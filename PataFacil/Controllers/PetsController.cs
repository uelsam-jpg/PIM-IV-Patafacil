using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PataFacil.Models;
using PataFacil.DAL;
using System.Threading.Tasks;
using System.Linq;

namespace PataFacil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PetsController : ControllerBase
    {
        private readonly IRepositorio<Pet> _petRepo;

        public PetsController(IRepositorio<Pet> petRepo)
        {
            _petRepo = petRepo;
        }

        // POST: api/pets (Cadastrar um novo pet)
        [HttpPost]
        [Authorize(Roles = "Tutor,Atendente,Gerente")]
        public async Task<IActionResult> Cadastrar([FromBody] Pet pet)
        {
            if (string.IsNullOrWhiteSpace(pet.Nome))
                return BadRequest(new { mensagem = "O nome do pet é obrigatório." });

            // O Tutor só cadastra pets para si mesmo, qualquer que seja o tutorId enviado
            var idSeTutor = User.ObterIdSeTutor();
            if (idSeTutor.HasValue) pet.TutorId = idSeTutor.Value;

            pet.Id = 0;
            pet.Ativo = true;

            var id = await _petRepo.AdicionarAsync(pet);
            return CreatedAtAction(nameof(ListarPorTutor), new { tutorId = pet.TutorId }, new { id = id, mensagem = "Pet cadastrado com sucesso!" });
        }

        // GET: api/pets/tutor/{tutorId} (Listar pets de um tutor específico)
        [HttpGet("tutor/{tutorId}")]
        public async Task<IActionResult> ListarPorTutor(int tutorId)
        {
            // O Tutor só lista os próprios pets; a equipe pode consultar os de qualquer tutor
            var idSeTutor = User.ObterIdSeTutor();
            if (idSeTutor.HasValue && idSeTutor.Value != tutorId) return Forbid();

            var petsDoTutor = (await _petRepo.BuscarAsync(p => p.TutorId == tutorId && p.Ativo)).ToList();
            return Ok(petsDoTutor);
        }
    }
}
