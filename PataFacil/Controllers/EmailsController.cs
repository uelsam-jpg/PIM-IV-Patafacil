using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PataFacil.DAL;
using PataFacil.Models;

namespace PataFacil.Controllers
{
    // Caixa de E-mail: cada usuário só enxerga os e-mails enviados para o próprio endereço
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmailsController : ControllerBase
    {
        private readonly IRepositorio<EmailEnviado> _emailRepo;

        public EmailsController(IRepositorio<EmailEnviado> emailRepo)
        {
            _emailRepo = emailRepo;
        }

        private string EmailDoUsuario => (User.FindFirstValue(ClaimTypes.Email) ?? string.Empty).Trim().ToLower();

        // GET: api/emails
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var email = EmailDoUsuario;
            var emails = await _emailRepo.BuscarAsync(e => e.Destinatario == email);
            return Ok(emails.OrderByDescending(e => e.DataEnvio).Take(200));
        }

        // GET: api/emails/nao-lidos (contador do menu)
        [HttpGet("nao-lidos")]
        public async Task<IActionResult> ContarNaoLidos()
        {
            var email = EmailDoUsuario;
            var emails = await _emailRepo.BuscarAsync(e => e.Destinatario == email && !e.Lida);
            return Ok(new { total = emails.Count() });
        }

        // PATCH: api/emails/{id}/lida
        [HttpPatch("{id}/lida")]
        public async Task<IActionResult> MarcarComoLida(int id, [FromQuery] bool lida = true)
        {
            var mensagem = await _emailRepo.ObterPorIdAsync(id);
            if (mensagem == null || mensagem.Destinatario != EmailDoUsuario) return NotFound();

            mensagem.Lida = lida;
            await _emailRepo.AtualizarAsync(mensagem);
            return NoContent();
        }

        // DELETE: api/emails/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var mensagem = await _emailRepo.ObterPorIdAsync(id);
            if (mensagem == null || mensagem.Destinatario != EmailDoUsuario) return NotFound();

            await _emailRepo.RemoverAsync(id);
            return NoContent();
        }
    }
}
