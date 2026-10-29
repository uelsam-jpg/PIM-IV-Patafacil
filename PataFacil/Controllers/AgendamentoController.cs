using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using PataFacil.Models;
using PataFacil.BLL;
using PataFacil.Controllers;

namespace PataFacil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Rota base: /api/agendamento
    [Authorize] // RF10: Garante que todo agendamento exige um usuário autenticado (Token JWT)
    public class AgendamentoController : ControllerBase
    {
        private readonly AgendamentoService _agendamentoService;

        public AgendamentoController(AgendamentoService agendamentoService)
        {
            _agendamentoService = agendamentoService;
        }

        [HttpPost]
        [Authorize(Roles = "Tutor,Atendente")]
        public async Task<IActionResult> CriarAgendamento([FromBody] Agendamento agendamento)
        {
            try
            {
                var id = await _agendamentoService.CriarAgendamentoAsync(agendamento, User.ObterIdSeTutor());
                return Ok(new { Mensagem = "Agendamento criado com sucesso!", Id = id });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex) // Captura nossos erros de RN01, RN03, etc.
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Tutor,Atendente,Gerente")]
        public async Task<IActionResult> CancelarAgendamento(int id)
        {
            try
            {
                await _agendamentoService.CancelarAgendamentoAsync(id, User.ObterIdSeTutor());
                return Ok(new { Mensagem = "Agendamento cancelado com sucesso!" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex) // Captura o erro da RN02 (Antecedência de 2h)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpGet("dia")]
        [Authorize(Roles = "Atendente,Veterinario,Gerente,Tosador")] // Agenda da clínica: somente a equipe
        public async Task<IActionResult> ObterAgendamentosDoDia([FromQuery] string? data)
        {
            try
            {
                if (!DateTime.TryParse(data, out DateTime dataBusca))
                    dataBusca = DateTime.Today;

                var agendamentos = await _agendamentoService.ObterAgendamentosDoDiaAsync(dataBusca);
                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpGet("semana")]
        [Authorize(Roles = "Atendente,Veterinario,Gerente,Tosador")] // Agenda da clínica: somente a equipe
        public async Task<IActionResult> ObterAgendamentosDaSemana([FromQuery] string? data)
        {
            try
            {
                if (!DateTime.TryParse(data, out DateTime dataBusca))
                    dataBusca = DateTime.Today;

                var agendamentos = await _agendamentoService.ObterAgendamentosDaSemanaAsync(dataBusca);
                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        // Agenda do balcão por período: ex. /api/agendamento/periodo?inicio=2026-10-13&fim=2026-10-31
        [HttpGet("periodo")]
        [Authorize(Roles = "Atendente,Veterinario,Gerente,Tosador")]
        public async Task<IActionResult> ObterAgendamentosDoPeriodo([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
        {
            try
            {
                return Ok(await _agendamentoService.ObterAgendamentosDoPeriodoAsync(inicio, fim));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        // Clientes que já chegaram e aguardam a finalização ("Em processo")
        [HttpGet("em-atendimento")]
        [Authorize(Roles = "Atendente,Veterinario,Gerente,Tosador")]
        public async Task<IActionResult> ObterEmAtendimento()
        {
            return Ok(await _agendamentoService.ObterEmAtendimentoAsync());
        }

        public class ObservacaoDTO
        {
            public string? Texto { get; set; }
        }

        [HttpPatch("{id}/observacoes")]
        [Authorize(Roles = "Atendente,Veterinario,Gerente,Tosador")]
        public async Task<IActionResult> AtualizarObservacoes(int id, [FromBody] ObservacaoDTO observacao)
        {
            try
            {
                await _agendamentoService.AtualizarObservacoesAtendimentoAsync(id, observacao.Texto);
                return Ok(new { Mensagem = "Observação salva." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpPatch("{id}/chegada")]
        [Authorize(Roles = "Atendente,Gerente")]
        public async Task<IActionResult> RegistrarChegada(int id)
        {
            try
            {
                await _agendamentoService.RegistrarChegadaAsync(id);
                return Ok(new { Mensagem = "Chegada registrada. O atendimento está em processo." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpPatch("{id}/pagamento")]
        [Authorize(Roles = "Atendente,Gerente")]
        public async Task<IActionResult> RegistrarPagamento(int id, [FromBody] PagamentoDTO pagamento)
        {
            try
            {
                await _agendamentoService.RegistrarPagamentoAsync(id, pagamento);
                return Ok(new { Mensagem = "Pagamento registrado e atendimento finalizado." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpGet("tutor/{idTutor}")]
        [Authorize(Roles = "Tutor,Atendente,Gerente")]
        public async Task<IActionResult> ObterAgendamentosDoTutor(int idTutor)
        {
            // O Tutor só enxerga os próprios agendamentos
            var idSeTutor = User.ObterIdSeTutor();
            if (idSeTutor.HasValue && idSeTutor.Value != idTutor) return Forbid();

            try
            {
                var agendamentos = await _agendamentoService.ObterAgendamentosDoTutorAsync(idTutor);
                return Ok(agendamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpPatch("{id}/confirmar")]
        [Authorize(Roles = "Atendente,Gerente")]
        public async Task<IActionResult> Confirmar(int id)
        {
            try
            {
                await _agendamentoService.ConfirmarAgendamentoAsync(id);
                return Ok(new { Mensagem = "Agendamento confirmado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpPatch("{id}/concluir")]
        [Authorize(Roles = "Veterinario,Gerente,Atendente")]
        public async Task<IActionResult> Concluir(int id)
        {
            try
            {
                await _agendamentoService.ConcluirAgendamentoAsync(id);
                return Ok(new { Mensagem = "Agendamento concluído com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpGet("horarios-disponiveis")]
        [AllowAnonymous] // Permite que tutores vejam os horários mesmo antes de logar totalmente
        public async Task<IActionResult> ObterHorariosDisponiveis([FromQuery] DateTime data, [FromQuery] int idServico, [FromQuery] int idFuncionario)
        {
            try
            {
                var horarios = await _agendamentoService.ObterHorariosDisponiveisAsync(data, idServico, idFuncionario);
                return Ok(horarios);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }
    }
}
