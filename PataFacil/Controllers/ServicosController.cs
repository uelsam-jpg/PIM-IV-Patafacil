using Microsoft.AspNetCore.Mvc;
using PataFacil.Models;
using PataFacil.DAL;
using System.Threading.Tasks;
using System.Linq;

namespace PataFacil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicosController : ControllerBase
    {
        private readonly IRepositorio<Servico> _repo;

        public ServicosController(IRepositorio<Servico> repo)
        {
            _repo = repo;
        }

        [HttpGet] // Público: é o catálogo de serviços exibido no agendamento
        public async Task<IActionResult> ListarTodos()
        {
            var servicos = (await _repo.ObterTodosAsync()).Where(s => s.Ativo);
            return Ok(servicos);
        }
    }
}