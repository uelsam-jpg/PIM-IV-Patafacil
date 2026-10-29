using Microsoft.AspNetCore.Mvc;
using PataFacil.Models;
using PataFacil.DAL;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace PataFacil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuncionariosController : ControllerBase
    {
        private readonly IRepositorio<Funcionario> _funcRepo;

        public FuncionariosController(IRepositorio<Funcionario> funcRepo)
        {
            _funcRepo = funcRepo;
        }

        // GET: api/funcionarios
        [HttpGet]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> ListarTodos()
        {
            var funcionarios = await _funcRepo.ObterTodosAsync();
            var funcionariosSeguros = funcionarios.Select(f => new { f.Id, f.Nome, f.Email, f.Cpf, f.Cargo, f.Ativo });
            return Ok(funcionariosSeguros);
        }

        // GET: api/funcionarios/profissionais (Para o wizard de agendamento)
        [HttpGet("profissionais")]
        [AllowAnonymous]
        public async Task<IActionResult> ListarProfissionais()
        {
            var profissionais = (await _funcRepo.ObterTodosAsync())
                .Where(f => f.Ativo && (f.Cargo == Cargo.Veterinario || f.Cargo == Cargo.Tosador))
                .Select(f => new { f.Id, f.Nome, f.Cargo });
            return Ok(profissionais);
        }

        // POST: api/funcionarios (RF13)
        [HttpPost]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Cadastrar([FromBody] Funcionario funcionario)
        {
            if (string.IsNullOrEmpty(funcionario.SenhaHash)) return BadRequest("A senha provisória é obrigatória.");

            var funcionarios = await _funcRepo.ObterTodosAsync();
            if (funcionarios.Any(f => f.Cpf == funcionario.Cpf || f.Email == funcionario.Email))
                return Conflict(new { Erro = "CPF ou E-mail já cadastrado." });

            funcionario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(funcionario.SenhaHash, 12);
            funcionario.DataAdmissao = System.DateTime.Now;
            funcionario.Ativo = true;

            var id = await _funcRepo.AdicionarAsync(funcionario);
            return Ok(new { Mensagem = "Funcionário cadastrado com sucesso!", Id = id });
        }

        // PUT: api/funcionarios/{id} (RF13)
        [HttpPut("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> Editar(int id, [FromBody] Funcionario funcAtualizado)
        {
            var funcExistente = await _funcRepo.ObterPorIdAsync(id);
            if (funcExistente == null) return NotFound();

            funcExistente.Nome = funcAtualizado.Nome;
            funcExistente.Email = funcAtualizado.Email;
            funcExistente.Cargo = funcAtualizado.Cargo;

            await _funcRepo.AtualizarAsync(funcExistente);
            return NoContent();
        }

        // DELETE: api/funcionarios/{id} (RF13 - Soft Delete)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> InativarReativar(int id)
        {
            var func = await _funcRepo.ObterPorIdAsync(id);
            if (func == null) return NotFound();

            func.Ativo = !func.Ativo; // Inverte o status
            await _funcRepo.AtualizarAsync(func);
            return Ok(new { mensagem = $"Funcionário {(func.Ativo ? "reativado" : "inativado")}." });
        }
    }
}