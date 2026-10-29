using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net.Http;
using Xunit;
using PataFacil.Models;
using PataFacil.DAL;
using PataFacil.BLL;

namespace PataFacil.Tests
{
    public class AgendamentoServiceTests
    {
        private readonly Mock<IRepositorio<Agendamento>> _agendamentoRepoMock;
        private readonly Mock<IRepositorio<Servico>> _servicoRepoMock;
        private readonly Mock<IRepositorio<Pet>> _petRepoMock;
        private readonly AgendamentoService _agendamentoService;

        // Agendamentos que o "banco de dados falso" devolve nas buscas filtradas
        private readonly List<Agendamento> _agendamentosNoBanco = new List<Agendamento>();

        public AgendamentoServiceTests()
        {
            // Prepara os Mocks ("Banco de dados falso") para os testes
            _agendamentoRepoMock = new Mock<IRepositorio<Agendamento>>();
            _servicoRepoMock = new Mock<IRepositorio<Servico>>();
            _petRepoMock = new Mock<IRepositorio<Pet>>();
            var tutorRepoMock = new Mock<IRepositorio<Tutor>>();
            var httpClientMock = new Mock<IHttpClientFactory>();
            var emailServiceMock = new Mock<IEmailService>();
            var funcRepoMock = new Mock<IRepositorio<Funcionario>>();

            // O filtro (WHERE) é aplicado de verdade sobre a lista em memória
            _agendamentoRepoMock
                .Setup(repo => repo.BuscarAsync(It.IsAny<Expression<Func<Agendamento, bool>>>()))
                .ReturnsAsync((Expression<Func<Agendamento, bool>> filtro) => _agendamentosNoBanco.FindAll(new Predicate<Agendamento>(filtro.Compile())));

            _servicoRepoMock.Setup(repo => repo.ObterPorIdAsync(1)).ReturnsAsync(new Servico { Id = 1, DuracaoMinutos = 60 });

            _agendamentoService = new AgendamentoService(
                _agendamentoRepoMock.Object, _servicoRepoMock.Object,
                httpClientMock.Object, _petRepoMock.Object, tutorRepoMock.Object, emailServiceMock.Object, funcRepoMock.Object,
                new ConfigurationBuilder().Build());
        }

        // Uma data futura que nunca cai no domingo, às 10h
        private static DateTime ProximoDiaUtilAs10h()
        {
            DateTime dataFutura = DateTime.Now.AddDays(5);
            if (dataFutura.DayOfWeek == DayOfWeek.Sunday) dataFutura = dataFutura.AddDays(1);
            return new DateTime(dataFutura.Year, dataFutura.Month, dataFutura.Day, 10, 0, 0);
        }

        [Fact]
        public async Task T01_CriarAgendamento_DiaValidoEHorarioSemConflito_RetornaId()
        {
            // Arrange (Preparação)
            _agendamentoRepoMock.Setup(repo => repo.AdicionarAsync(It.IsAny<Agendamento>())).ReturnsAsync(10); // Simula retorno ID 10
            var agendamento = new Agendamento { IdServico = 1, IdFuncionario = 1, DataHoraInicio = ProximoDiaUtilAs10h() };

            // Act (Ação)
            var resultadoId = await _agendamentoService.CriarAgendamentoAsync(agendamento);

            // Assert (Verificação)
            Assert.Equal(10, resultadoId);
            // RN10: o fim foi calculado pelo sistema (início + 60 minutos do serviço)
            Assert.Equal(agendamento.DataHoraInicio.AddMinutes(60), agendamento.DataHoraFim);
        }

        [Fact]
        public async Task T02_CriarAgendamento_ComConflitoDeHorario_LancaExcecao()
        {
            // Arrange
            var dataInicio = ProximoDiaUtilAs10h();

            // Simula que já existe um agendamento no banco que cruza com o nosso (das 09:30 às 10:30)
            var existente = new Agendamento
            {
                IdFuncionario = 1,
                DataHoraInicio = dataInicio.AddMinutes(-30),
                DataHoraFim = dataInicio.AddMinutes(30)
            };
            existente.Confirmar();
            _agendamentosNoBanco.Add(existente);

            var novoAgendamento = new Agendamento { IdServico = 1, IdFuncionario = 1, DataHoraInicio = dataInicio };

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => _agendamentoService.CriarAgendamentoAsync(novoAgendamento));
            Assert.Contains("já possui um agendamento", excecao.Message);
            _agendamentoRepoMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Agendamento>()), Times.Never);
        }

        [Fact]
        public async Task T03_CriarAgendamento_DataNoPassado_LancaExcecao()
        {
            // Arrange
            var agendamento = new Agendamento { IdServico = 1, IdFuncionario = 1, DataHoraInicio = DateTime.Now.AddDays(-1) }; // Ontem

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => _agendamentoService.CriarAgendamentoAsync(agendamento));
            Assert.Contains("futura", excecao.Message);
        }

        [Fact]
        public async Task T04_CriarAgendamento_NoDomingo_LancaExcecao()
        {
            // Arrange
            DateTime proximoDomingo = DateTime.Now.AddDays(1);
            while (proximoDomingo.DayOfWeek != DayOfWeek.Sunday) proximoDomingo = proximoDomingo.AddDays(1);
            var dataInicio = new DateTime(proximoDomingo.Year, proximoDomingo.Month, proximoDomingo.Day, 10, 0, 0);
            var agendamento = new Agendamento { IdServico = 1, IdFuncionario = 1, DataHoraInicio = dataInicio };

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => _agendamentoService.CriarAgendamentoAsync(agendamento));
            Assert.Contains("domingos", excecao.Message);
        }

        [Fact]
        public async Task T05_CancelarAgendamento_JaConcluido_LancaExcecao()
        {
            // Arrange
            var agendamentoConcluido = new Agendamento { Id = 1, DataHoraInicio = DateTime.Now.AddDays(1) };
            agendamentoConcluido.Confirmar();
            agendamentoConcluido.Concluir();
            _agendamentoRepoMock.Setup(repo => repo.ObterPorIdAsync(1)).ReturnsAsync(agendamentoConcluido);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => _agendamentoService.CancelarAgendamentoAsync(1));
            Assert.Contains("concluídos não podem ser cancelados", excecao.Message);
        }

        [Fact]
        public async Task T06_CancelarAgendamento_TutorComMenosDe2Horas_LancaExcecao()
        {
            // Arrange
            var agendamento = new Agendamento { Id = 1, IdPet = 7, DataHoraInicio = DateTime.Now.AddHours(1) }; // Falta só 1h
            _agendamentoRepoMock.Setup(repo => repo.ObterPorIdAsync(1)).ReturnsAsync(agendamento);
            _petRepoMock.Setup(repo => repo.ObterPorIdAsync(7)).ReturnsAsync(new Pet { Id = 7, TutorId = 3 });

            // Act & Assert (o tutor 3, dono do pet, tenta cancelar online)
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => _agendamentoService.CancelarAgendamentoAsync(1, idTutorSolicitante: 3));
            Assert.Contains("2 horas", excecao.Message);
        }

        // ================= T09 E T10 (Teste do Model) =================
        [Fact]
        public void T09_ConfirmarAgendamento_APartirDePendente_AlteraStatusParaConfirmado()
        {
            // Arrange
            var agendamento = new Agendamento();
            // Act
            agendamento.Confirmar();
            // Assert
            Assert.Equal(StatusAgendamento.CONFIRMADO, agendamento.Status);
        }

        [Fact]
        public void T10_ConcluirAgendamento_APartirDePendente_LancaExcecao()
        {
            var agendamento = new Agendamento(); // Nasce PENDENTE
            Assert.Throws<InvalidOperationException>(() => agendamento.Concluir());
        }

        // ================= Regras complementares =================
        [Fact]
        public async Task T11_CriarAgendamento_TerminandoAposAs19h_LancaExcecao()
        {
            // Arrange: serviço de 60 min começando às 18h30 terminaria às 19h30 (RN03)
            var data = ProximoDiaUtilAs10h();
            var agendamento = new Agendamento { IdServico = 1, IdFuncionario = 1, DataHoraInicio = data.Date.AddHours(18).AddMinutes(30) };

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => _agendamentoService.CriarAgendamentoAsync(agendamento));
            Assert.Contains("horário de funcionamento", excecao.Message);
        }

        [Fact]
        public async Task T12_CriarAgendamento_HorarioDeAgendamentoCancelado_FicaLivre()
        {
            // Arrange: o único agendamento que cruza o horário está CANCELADO (RN01 ignora cancelados)
            var dataInicio = ProximoDiaUtilAs10h();
            var cancelado = new Agendamento { IdFuncionario = 1, DataHoraInicio = dataInicio, DataHoraFim = dataInicio.AddMinutes(60) };
            cancelado.Cancelar();
            _agendamentosNoBanco.Add(cancelado);
            _agendamentoRepoMock.Setup(repo => repo.AdicionarAsync(It.IsAny<Agendamento>())).ReturnsAsync(11);

            // Act
            var id = await _agendamentoService.CriarAgendamentoAsync(new Agendamento { IdServico = 1, IdFuncionario = 1, DataHoraInicio = dataInicio });

            // Assert
            Assert.Equal(11, id);
        }

        [Fact]
        public async Task T13_CriarAgendamento_TutorComPetDeOutroTutor_LancaExcecao()
        {
            // Arrange: o pet 7 pertence ao tutor 3, mas quem agenda é o tutor 99
            _petRepoMock.Setup(repo => repo.ObterPorIdAsync(7)).ReturnsAsync(new Pet { Id = 7, TutorId = 3 });
            var agendamento = new Agendamento { IdPet = 7, IdServico = 1, IdFuncionario = 1, DataHoraInicio = ProximoDiaUtilAs10h() };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _agendamentoService.CriarAgendamentoAsync(agendamento, idTutorSolicitante: 99));
        }

        [Fact]
        public async Task T14_CancelarAgendamento_TutorQueNaoEDono_LancaExcecao()
        {
            // Arrange
            var agendamento = new Agendamento { Id = 1, IdPet = 7, DataHoraInicio = DateTime.Now.AddDays(2) };
            _agendamentoRepoMock.Setup(repo => repo.ObterPorIdAsync(1)).ReturnsAsync(agendamento);
            _petRepoMock.Setup(repo => repo.ObterPorIdAsync(7)).ReturnsAsync(new Pet { Id = 7, TutorId = 3 });

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _agendamentoService.CancelarAgendamentoAsync(1, idTutorSolicitante: 99));
            Assert.Equal(StatusAgendamento.PENDENTE, agendamento.Status);
        }

        [Fact]
        public async Task T15_CancelarAgendamento_PelaEquipeComMenosDe2Horas_Cancela()
        {
            // Arrange: a antecedência de 2h (RN02) vale para o cancelamento online do tutor, não para a equipe
            var agendamento = new Agendamento { Id = 1, IdPet = 7, DataHoraInicio = DateTime.Now.AddHours(1) };
            _agendamentoRepoMock.Setup(repo => repo.ObterPorIdAsync(1)).ReturnsAsync(agendamento);

            // Act
            await _agendamentoService.CancelarAgendamentoAsync(1);

            // Assert
            Assert.Equal(StatusAgendamento.CANCELADO, agendamento.Status);
            _agendamentoRepoMock.Verify(repo => repo.AtualizarAsync(agendamento), Times.Once);
        }

        // ================= Balcão: chegada e pagamento =================
        [Fact]
        public void T17_RegistrarChegada_DePendente_ConfirmaEMarcaEmProcesso()
        {
            var agendamento = new Agendamento(); // Nasce PENDENTE

            agendamento.RegistrarChegada();

            Assert.Equal(StatusAgendamento.CONFIRMADO, agendamento.Status);
            Assert.NotNull(agendamento.DataChegada);
        }

        [Fact]
        public void T18_RegistrarPagamento_SemChegada_LancaExcecao()
        {
            var agendamento = new Agendamento();
            agendamento.Confirmar();

            var excecao = Assert.Throws<InvalidOperationException>(() => agendamento.RegistrarPagamento(80m, FormaPagamentoEnum.Pix));
            Assert.Contains("chegada", excecao.Message);
        }

        [Fact]
        public void T19_RegistrarPagamento_AposChegada_ConcluiEGuardaValorEForma()
        {
            var agendamento = new Agendamento();
            agendamento.RegistrarChegada();

            agendamento.RegistrarPagamento(95.50m, FormaPagamentoEnum.CartaoCredito);

            Assert.Equal(StatusAgendamento.CONCLUIDO, agendamento.Status);
            Assert.Equal(95.50m, agendamento.ValorPago);
            Assert.Equal(FormaPagamentoEnum.CartaoCredito, agendamento.FormaPagamento);
            // Não dá para receber duas vezes o mesmo atendimento
            Assert.Throws<InvalidOperationException>(() => agendamento.RegistrarPagamento(10m, FormaPagamentoEnum.Dinheiro));
        }

        [Fact]
        public void T20_RegistrarChegada_AgendamentoCancelado_LancaExcecao()
        {
            var agendamento = new Agendamento();
            agendamento.Cancelar();

            Assert.Throws<InvalidOperationException>(() => agendamento.RegistrarChegada());
        }

        [Fact]
        public async Task T16_HorariosDisponiveis_NaoOfereceHorarioOcupado()
        {
            // Arrange: profissional ocupado das 10h às 11h
            var dia = ProximoDiaUtilAs10h();
            _agendamentosNoBanco.Add(new Agendamento { IdFuncionario = 1, DataHoraInicio = dia, DataHoraFim = dia.AddMinutes(60) });

            // Act
            var horarios = await _agendamentoService.ObterHorariosDisponiveisAsync(dia.Date, 1, 1);

            // Assert: 09h00 e 11h00 estão livres; 09h15 a 10h45 cruzariam o atendimento existente
            Assert.Contains(dia.AddHours(-1), horarios);
            Assert.Contains(dia.AddHours(1), horarios);
            Assert.DoesNotContain(dia.AddMinutes(-45), horarios);
            Assert.DoesNotContain(dia, horarios);
            Assert.DoesNotContain(dia.AddMinutes(45), horarios);
            // O último horário do dia termina exatamente no fechamento (18h00 + 60 min = 19h00)
            Assert.Equal(dia.Date.AddHours(18), horarios[horarios.Count - 1]);
        }
    }
}
