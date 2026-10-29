using Microsoft.AspNetCore.Mvc;
using PataFacil.Models;
using PataFacil.BLL;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace PataFacil.Controllers
{
    [Route("api/[controller]")] // Rota base: /api/historicoclinico
    [ApiController]
    [Authorize] // Protege o controller inteiro, exigindo login
    public class HistoricoClinicoController : ControllerBase
    {
        private readonly HistoricoClinicoService _historicoService;

        public HistoricoClinicoController(HistoricoClinicoService historicoService)
        {
            _historicoService = historicoService;
        }

        // POST: api/historicoclinico (RF05 - Usado pelo Veterinário)
        [HttpPost]
        [Authorize(Roles = "Veterinario,Gerente")]
        public async Task<IActionResult> RegistrarAtendimento([FromBody] HistoricoClinico historico)
        {
            try
            {
                var id = await _historicoService.RegistrarHistoricoAsync(historico, User.ObterId());
                return StatusCode(201, new { Mensagem = "Histórico clínico registrado com sucesso!", Id = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        // GET: api/historicoclinico/pet/{idPet} (UC08 - Usado pelo Tutor)
        [HttpGet("pet/{idPet}")]
        [Authorize(Roles = "Tutor,Veterinario,Gerente")]
        public async Task<IActionResult> GetHistoricoPorPet(int idPet)
        {
            try
            {
                // Validação de Segurança: O Tutor só pode ver o histórico do seu próprio pet.
                var historicos = await _historicoService.ObterHistoricoPorPetAsync(idPet, User.ObterIdSeTutor());
                return Ok(historicos);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Erro = ex.Message });
            }
        }
    }
}
