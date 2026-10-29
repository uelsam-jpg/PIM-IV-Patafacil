using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PataFacil.Models;
using PataFacil.DAL;

namespace PataFacil.BLL
{
    // Agendamento com os nomes já resolvidos, para as telas não exibirem apenas IDs
    public class AgendamentoDetalhadoDTO
    {
        public int Id { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public string? Observacoes { get; set; }
        public DateTime DataCriacao { get; set; }
        public int IdPet { get; set; }
        public int IdServico { get; set; }
        public int IdFuncionario { get; set; }
        public StatusAgendamento Status { get; set; }
        public string NomePet { get; set; } = string.Empty;
        public string NomeTutor { get; set; } = string.Empty;
        public string NomeServico { get; set; } = string.Empty;
        public string NomeProfissional { get; set; } = string.Empty;
        public decimal Preco { get; set; }

        // Dados de contato e do pet, para o balcão saber quem é o cliente
        public string TelefoneTutor { get; set; } = string.Empty;
        public string EmailTutor { get; set; } = string.Empty;
        public string CpfTutor { get; set; } = string.Empty;
        public Especie? EspeciePet { get; set; }
        public string RacaPet { get; set; } = string.Empty;
        public decimal? PesoPet { get; set; }

        // Fluxo do balcão
        public string? ObservacoesAtendimento { get; set; }
        public DateTime? DataChegada { get; set; }
        public decimal? ValorPago { get; set; }
        public FormaPagamentoEnum? FormaPagamento { get; set; }
        public DateTime? DataPagamento { get; set; }
    }

    // Dados enviados pelo balcão ao dar baixa no pagamento
    public class PagamentoDTO
    {
        public decimal ValorPago { get; set; }
        public FormaPagamentoEnum FormaPagamento { get; set; }
    }

    public class AgendamentoService
    {
        private static readonly TimeSpan HorarioAbertura = new TimeSpan(8, 0, 0);
        private static readonly TimeSpan HorarioFechamento = new TimeSpan(19, 0, 0);

        private readonly IRepositorio<Agendamento> _agendamentoRepo;
        private readonly IRepositorio<Servico> _servicoRepo;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRepositorio<Pet> _petRepo;
        private readonly IRepositorio<Tutor> _tutorRepo;
        private readonly IEmailService _emailService;
        private readonly IRepositorio<Funcionario> _funcionarioRepo;
        private readonly IConfiguration _configuration;

        // Injeção de dependência dos repositórios
        public AgendamentoService(
            IRepositorio<Agendamento> agendamentoRepo,
            IRepositorio<Servico> servicoRepo,
            IHttpClientFactory httpClientFactory,
            IRepositorio<Pet> petRepo,
            IRepositorio<Tutor> tutorRepo,
            IEmailService emailService,
            IRepositorio<Funcionario> funcionarioRepo,
            IConfiguration configuration)
        {
            _agendamentoRepo = agendamentoRepo;
            _servicoRepo = servicoRepo;
            _httpClientFactory = httpClientFactory;
            _petRepo = petRepo;
            _tutorRepo = tutorRepo;
            _emailService = emailService;
            _funcionarioRepo = funcionarioRepo;
            _configuration = configuration;
        }

        // idTutorSolicitante é informado quando quem chama é um Tutor: ele só agenda para os próprios pets
        public async Task<int> CriarAgendamentoAsync(Agendamento novoAgendamento, int? idTutorSolicitante = null)
        {
            // 1. Busca o serviço para obter a duração
            var servico = await _servicoRepo.ObterPorIdAsync(novoAgendamento.IdServico);
            if (servico == null || !servico.Ativo)
                throw new ArgumentException("Serviço selecionado não encontrado.");

            // RN02: Agendamentos só podem ser criados para data/hora futura.
            if (novoAgendamento.DataHoraInicio <= DateTime.Now)
                throw new InvalidOperationException("A data do agendamento deve ser futura.");

            // RN10: A data/hora de fim é sempre calculada pelo sistema somando a duração
            novoAgendamento.DataHoraFim = novoAgendamento.DataHoraInicio.AddMinutes(servico.DuracaoMinutos);

            // RN03: Horário de funcionamento (08h00 às 19h00, segunda a sábado)
            var diaSemana = novoAgendamento.DataHoraInicio.DayOfWeek;
            if (diaSemana == DayOfWeek.Sunday)
                throw new InvalidOperationException("Não é possível realizar agendamentos aos domingos.");

            if (novoAgendamento.DataHoraInicio.TimeOfDay < HorarioAbertura ||
                novoAgendamento.DataHoraFim.TimeOfDay > HorarioFechamento ||
                novoAgendamento.DataHoraFim.Date != novoAgendamento.DataHoraInicio.Date)
            {
                throw new InvalidOperationException("O agendamento deve estar contido no horário de funcionamento (08h00 às 19h00).");
            }

            if (idTutorSolicitante.HasValue)
            {
                var petDoTutor = await _petRepo.ObterPorIdAsync(novoAgendamento.IdPet);
                if (petDoTutor == null || petDoTutor.TutorId != idTutorSolicitante.Value)
                    throw new UnauthorizedAccessException("O pet informado não pertence ao tutor autenticado.");
            }

            // RN01: Conflito de horário (inicio_A < fim_B E fim_A > inicio_B)
            var inicio = novoAgendamento.DataHoraInicio;
            var fim = novoAgendamento.DataHoraFim;
            var idFuncionario = novoAgendamento.IdFuncionario;

            var conflitos = await _agendamentoRepo.BuscarAsync(a =>
                a.IdFuncionario == idFuncionario &&
                a.Status != StatusAgendamento.CANCELADO &&
                inicio < a.DataHoraFim &&
                fim > a.DataHoraInicio);

            if (conflitos.Any())
                throw new InvalidOperationException("O profissional selecionado já possui um agendamento neste horário.");

            // Histórico do pet: alimenta as variáveis comportamentais do modelo de IA
            var idPet = novoAgendamento.IdPet;
            var historicoDoPet = (await _agendamentoRepo.BuscarAsync(a => a.IdPet == idPet)).ToList();

            // 1. Salva o agendamento no banco via Repositório
            var idAgendamento = await _agendamentoRepo.AdicionarAsync(novoAgendamento);

            // 2. Integração com o Microsserviço de Inteligência Artificial (Python FastAPI)
            try
            {
                var antecedencia = (novoAgendamento.DataHoraInicio - DateTime.Now).Days;

                var mlPayload = new
                {
                    antecedencia_dias = antecedencia < 0 ? 0 : antecedencia,
                    historico_cancelamentos = historicoDoPet.Count(a => a.Status == StatusAgendamento.CANCELADO),
                    tipo_servico = novoAgendamento.IdServico,
                    dia_semana = (int)novoAgendamento.DataHoraInicio.DayOfWeek,
                    tutor_novo = historicoDoPet.Any() ? 0 : 1
                };

                // URL configurável: no Docker a IA é o serviço "ml-api", não o localhost
                var mlUrl = _configuration["MlApiUrl"] ?? "http://127.0.0.1:8000";
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(3); // A IA é um apoio: não pode segurar o agendamento
                var content = new StringContent(JsonSerializer.Serialize(mlPayload), Encoding.UTF8, "application/json");

                // Envia os dados para a nossa IA classificar
                var response = await client.PostAsync($"{mlUrl}/api/ml/prever-cancelamento", content);

                if (response.IsSuccessStatusCode)
                {
                    var resultString = await response.Content.ReadAsStringAsync();
                    var mlResult = JsonSerializer.Deserialize<JsonElement>(resultString);

                    // Captura a IA e adiciona uma etiqueta nas observações
                    novoAgendamento.Observacoes += $"\n[IA] Risco de Cancelamento: {mlResult.GetProperty("risco").GetString()} ({(mlResult.GetProperty("probabilidade").GetDouble() * 100):0}%)";
                    await _agendamentoRepo.AtualizarAsync(novoAgendamento);
                }
            }
            catch
            {
                // Ignora falhas de conexão para que o Python não impeça o funcionamento do Pet Shop caso esteja desligado
            }

            // 3. Envio de E-mail Automático de Confirmação (RF09)
            try
            {
                var pet = await _petRepo.ObterPorIdAsync(novoAgendamento.IdPet);
                if (pet != null)
                {
                    var tutor = await _tutorRepo.ObterPorIdAsync(pet.TutorId);
                    if (tutor != null)
                    {
                        string msgTutor = $"Olá {tutor.Nome}, o agendamento do(a) {pet.Nome} foi marcado para o dia {novoAgendamento.DataHoraInicio:dd/MM/yyyy 'às' HH:mm}.";
                        await _emailService.EnviarEmailAsync(tutor.Email, "Agendamento Confirmado - PataFácil", msgTutor);

                        // Avisa também o profissional responsável e o balcão (atendentes)
                        var servicoNome = servico.Nome;
                        var contato = TelefoneInformado(tutor.Telefone) ? tutor.Telefone : "telefone não informado";
                        var aviso = $"Novo agendamento: {servicoNome} para {pet.Nome} no dia {novoAgendamento.DataHoraInicio:dd/MM/yyyy 'às' HH:mm}.\n" +
                                    $"Cliente: {tutor.Nome} - {contato} - {tutor.Email}";

                        var profissional = await _funcionarioRepo.ObterPorIdAsync(novoAgendamento.IdFuncionario);
                        if (profissional != null && profissional.Ativo)
                            await _emailService.EnviarEmailAsync(profissional.Email, "Novo Agendamento Registrado", aviso);

                        var atendentes = await _funcionarioRepo.BuscarAsync(f => f.Ativo && f.Cargo == Cargo.Atendente);
                        foreach (var atendente in atendentes.Where(a => a.Id != novoAgendamento.IdFuncionario))
                            await _emailService.EnviarEmailAsync(atendente.Email, $"Novo agendamento: {pet.Nome} ({servicoNome})", aviso);
                    }
                }
            }
            catch { /* Ignora se o serviço de email não estiver configurado para não travar a API */ }

            return idAgendamento;
        }

        public async Task CancelarAgendamentoAsync(int idAgendamento, int? idTutorSolicitante = null)
        {
            var agendamento = await _agendamentoRepo.ObterPorIdAsync(idAgendamento);
            if (agendamento == null)
                throw new ArgumentException("Agendamento não encontrado.");

            var pet = await _petRepo.ObterPorIdAsync(agendamento.IdPet);

            if (idTutorSolicitante.HasValue)
            {
                if (pet == null || pet.TutorId != idTutorSolicitante.Value)
                    throw new UnauthorizedAccessException("Este agendamento não pertence ao tutor autenticado.");

                // RN02: o cancelamento online pelo tutor exige pelo menos 2 horas de antecedência.
                // A equipe pode cancelar a qualquer momento (ex.: o cliente avisa por telefone).
                var tempoAteInicio = agendamento.DataHoraInicio - DateTime.Now;
                if (tempoAteInicio.TotalHours < 2)
                    throw new InvalidOperationException("O cancelamento deve ser feito com pelo menos 2 horas de antecedência.");
            }

            // Utiliza o método próprio da entidade (Encapsulamento)
            agendamento.Cancelar();

            await _agendamentoRepo.AtualizarAsync(agendamento);

            // Dispara E-mail de Cancelamento (RF09)
            try
            {
                if (pet != null)
                {
                    var tutor = await _tutorRepo.ObterPorIdAsync(pet.TutorId);
                    if (tutor != null) {
                        await _emailService.EnviarEmailAsync(tutor.Email, "Agendamento Cancelado - PataFácil", $"Olá {tutor.Nome}, o agendamento do(a) {pet.Nome} foi cancelado conforme solicitado.");
                    }
                }
            }
            catch { /* Ignora falha de email */ }
        }

        public async Task<IEnumerable<AgendamentoDetalhadoDTO>> ObterAgendamentosDoDiaAsync(DateTime data)
        {
            var inicio = data.Date;
            var fim = inicio.AddDays(1);
            var agendamentos = await _agendamentoRepo.BuscarAsync(a => a.DataHoraInicio >= inicio && a.DataHoraInicio < fim);
            return await DetalharAsync(agendamentos);
        }

        public async Task<IEnumerable<AgendamentoDetalhadoDTO>> ObterAgendamentosDaSemanaAsync(DateTime dataInicio)
        {
            var inicio = dataInicio.Date;
            var fim = inicio.AddDays(8); // Hoje + os próximos 7 dias
            var agendamentos = await _agendamentoRepo.BuscarAsync(a => a.DataHoraInicio >= inicio && a.DataHoraInicio < fim);
            return await DetalharAsync(agendamentos);
        }

        // Agenda do balcão: qualquer intervalo de datas (semana passada, a atual, a próxima...)
        public async Task<IEnumerable<AgendamentoDetalhadoDTO>> ObterAgendamentosDoPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            var inicio = dataInicio.Date;
            var fim = dataFim.Date.AddDays(1); // o último dia entra inteiro
            if (fim <= inicio)
                throw new ArgumentException("A data final deve ser igual ou posterior à data inicial.");
            if ((fim - inicio).TotalDays > 62)
                throw new ArgumentException("Consulte no máximo 62 dias por vez.");

            var agendamentos = await _agendamentoRepo.BuscarAsync(a => a.DataHoraInicio >= inicio && a.DataHoraInicio < fim);
            return await DetalharAsync(agendamentos);
        }

        // Clientes que já chegaram e ainda não pagaram, qualquer que seja o dia do agendamento
        public async Task<IEnumerable<AgendamentoDetalhadoDTO>> ObterEmAtendimentoAsync()
        {
            var agendamentos = await _agendamentoRepo.BuscarAsync(a =>
                a.DataChegada != null && a.DataPagamento == null && a.Status != StatusAgendamento.CANCELADO);
            return await DetalharAsync(agendamentos);
        }

        public async Task AtualizarObservacoesAtendimentoAsync(int idAgendamento, string? texto)
        {
            var agendamento = await ObterOuFalharAsync(idAgendamento);
            var observacao = (texto ?? string.Empty).Trim();
            if (observacao.Length > 2000)
                throw new ArgumentException("A observação pode ter no máximo 2000 caracteres.");

            agendamento.ObservacoesAtendimento = observacao.Length == 0 ? null : observacao;
            await _agendamentoRepo.AtualizarAsync(agendamento);
        }

        public async Task RegistrarChegadaAsync(int idAgendamento)
        {
            var agendamento = await ObterOuFalharAsync(idAgendamento);
            agendamento.RegistrarChegada();
            await _agendamentoRepo.AtualizarAsync(agendamento);
        }

        public async Task RegistrarPagamentoAsync(int idAgendamento, PagamentoDTO pagamento)
        {
            var agendamento = await ObterOuFalharAsync(idAgendamento);
            agendamento.RegistrarPagamento(pagamento.ValorPago, pagamento.FormaPagamento);
            await _agendamentoRepo.AtualizarAsync(agendamento);
        }

        private async Task<Agendamento> ObterOuFalharAsync(int idAgendamento)
        {
            return await _agendamentoRepo.ObterPorIdAsync(idAgendamento)
                ?? throw new ArgumentException("Agendamento não encontrado.");
        }

        // Cadastros antigos gravavam "(00) 00000-0000" quando o telefone não era informado
        private static bool TelefoneInformado(string? telefone)
        {
            var digitos = new string((telefone ?? string.Empty).Where(char.IsDigit).ToArray());
            return digitos.Length >= 10 && digitos.Trim('0').Length > 0;
        }

        public async Task<IEnumerable<AgendamentoDetalhadoDTO>> ObterAgendamentosDoTutorAsync(int idTutor)
        {
            var petsDoTutor = (await _petRepo.BuscarAsync(p => p.TutorId == idTutor)).Select(p => p.Id).ToList();
            if (petsDoTutor.Count == 0)
                return Enumerable.Empty<AgendamentoDetalhadoDTO>();

            var agendamentos = await _agendamentoRepo.BuscarAsync(a => petsDoTutor.Contains(a.IdPet));
            return await DetalharAsync(agendamentos);
        }

        public async Task ConfirmarAgendamentoAsync(int idAgendamento)
        {
            var agendamento = await _agendamentoRepo.ObterPorIdAsync(idAgendamento);
            if (agendamento == null)
                throw new ArgumentException("Agendamento não encontrado.");

            agendamento.Confirmar();
            await _agendamentoRepo.AtualizarAsync(agendamento);
        }

        public async Task ConcluirAgendamentoAsync(int idAgendamento)
        {
            var agendamento = await _agendamentoRepo.ObterPorIdAsync(idAgendamento);
            if (agendamento == null)
                throw new ArgumentException("Agendamento não encontrado.");

            agendamento.Concluir();
            await _agendamentoRepo.AtualizarAsync(agendamento);
        }

        public async Task<List<DateTime>> ObterHorariosDisponiveisAsync(DateTime data, int idServico, int idFuncionario)
        {
            var servico = await _servicoRepo.ObterPorIdAsync(idServico);
            if (servico == null)
                throw new ArgumentException("Serviço não encontrado.");

            var horariosDisponiveis = new List<DateTime>();

            // RN03: não há expediente aos domingos
            if (data.DayOfWeek == DayOfWeek.Sunday)
                return horariosDisponiveis;

            // Pega todos os agendamentos do profissional naquele dia para evitar múltiplas buscas
            var inicioDia = data.Date;
            var fimDia = inicioDia.AddDays(1);
            var agendamentosDoDia = (await _agendamentoRepo.BuscarAsync(a =>
                    a.IdFuncionario == idFuncionario &&
                    a.Status != StatusAgendamento.CANCELADO &&
                    a.DataHoraInicio >= inicioDia && a.DataHoraInicio < fimDia))
                .ToList();

            // Itera pelo dia em intervalos de 15 minutos
            for (var horario = data.Date.Add(HorarioAbertura); horario.AddMinutes(servico.DuracaoMinutos) <= data.Date.Add(HorarioFechamento); horario = horario.AddMinutes(15))
            {
                // Ignora horários no passado
                if (horario <= DateTime.Now) continue;

                var horarioFimProposto = horario.AddMinutes(servico.DuracaoMinutos);

                // Verifica se o slot proposto conflita com algum agendamento existente
                bool temConflito = agendamentosDoDia.Any(a =>
                    horario < a.DataHoraFim && horarioFimProposto > a.DataHoraInicio
                );

                if (!temConflito)
                {
                    horariosDisponiveis.Add(horario);
                }
            }
            return horariosDisponiveis;
        }

        // Resolve os nomes de pet, tutor, serviço e profissional de uma lista de agendamentos
        private async Task<IEnumerable<AgendamentoDetalhadoDTO>> DetalharAsync(IEnumerable<Agendamento> agendamentos)
        {
            var lista = agendamentos.OrderBy(a => a.DataHoraInicio).ToList();
            if (lista.Count == 0)
                return Enumerable.Empty<AgendamentoDetalhadoDTO>();

            var idsPets = lista.Select(a => a.IdPet).Distinct().ToList();
            var pets = (await _petRepo.BuscarAsync(p => idsPets.Contains(p.Id))).ToDictionary(p => p.Id);

            var idsTutores = pets.Values.Select(p => p.TutorId).Distinct().ToList();
            var tutores = (await _tutorRepo.BuscarAsync(t => idsTutores.Contains(t.Id))).ToDictionary(t => t.Id);

            var servicos = (await _servicoRepo.ObterTodosAsync()).ToDictionary(s => s.Id);
            var funcionarios = (await _funcionarioRepo.ObterTodosAsync()).ToDictionary(f => f.Id);

            return lista.Select(a =>
            {
                pets.TryGetValue(a.IdPet, out var pet);
                Tutor? tutor = null;
                if (pet != null) tutores.TryGetValue(pet.TutorId, out tutor);
                servicos.TryGetValue(a.IdServico, out var servico);
                funcionarios.TryGetValue(a.IdFuncionario, out var funcionario);

                return new AgendamentoDetalhadoDTO
                {
                    Id = a.Id,
                    DataHoraInicio = a.DataHoraInicio,
                    DataHoraFim = a.DataHoraFim,
                    Observacoes = a.Observacoes,
                    DataCriacao = a.DataCriacao,
                    IdPet = a.IdPet,
                    IdServico = a.IdServico,
                    IdFuncionario = a.IdFuncionario,
                    Status = a.Status,
                    NomePet = pet?.Nome ?? string.Empty,
                    NomeTutor = tutor?.Nome ?? string.Empty,
                    NomeServico = servico?.Nome ?? string.Empty,
                    NomeProfissional = funcionario?.Nome ?? string.Empty,
                    Preco = servico?.Preco ?? 0,
                    TelefoneTutor = TelefoneInformado(tutor?.Telefone) ? tutor!.Telefone : string.Empty,
                    EmailTutor = tutor?.Email ?? string.Empty,
                    CpfTutor = tutor?.Cpf ?? string.Empty,
                    EspeciePet = pet?.Especie,
                    RacaPet = pet?.Raca ?? string.Empty,
                    PesoPet = pet?.PesoKg,
                    ObservacoesAtendimento = a.ObservacoesAtendimento,
                    DataChegada = a.DataChegada,
                    ValorPago = a.ValorPago,
                    FormaPagamento = a.FormaPagamento,
                    DataPagamento = a.DataPagamento
                };
            }).ToList();
        }
    }
}
