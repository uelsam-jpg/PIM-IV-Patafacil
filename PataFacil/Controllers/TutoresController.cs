using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PataFacil.Models;
using PataFacil.DAL;
using PataFacil.BLL;
using System.Linq;
using System.Threading.Tasks;

namespace PataFacil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutoresController : ControllerBase
    {
        private readonly IRepositorio<Tutor> _tutorRepo;
        private readonly IRepositorio<Funcionario> _funcionarioRepo;
        private readonly IEmailService _emailService;

        // Injeção de dependência do nosso Repositório genérico
        public TutoresController(IRepositorio<Tutor> tutorRepo, IRepositorio<Funcionario> funcionarioRepo, IEmailService emailService)
        {
            _tutorRepo = tutorRepo;
            _funcionarioRepo = funcionarioRepo;
            _emailService = emailService;
        }

        // POST: api/tutores (Cadastrar um novo tutor) — público: é a tela de cadastro
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Cadastrar([FromBody] Tutor tutor)
        {
            tutor.Nome = (tutor.Nome ?? string.Empty).Trim();
            tutor.Email = (tutor.Email ?? string.Empty).Trim().ToLower();
            tutor.Cpf = new string((tutor.Cpf ?? string.Empty).Where(char.IsDigit).ToArray());

            if (tutor.Nome.Length == 0 || !tutor.Email.Contains('@'))
                return BadRequest(new { mensagem = "Informe nome e um e-mail válido." });
            if (tutor.Cpf.Length != 11)
                return BadRequest(new { mensagem = "O CPF deve conter 11 dígitos." });
            if (string.IsNullOrEmpty(tutor.SenhaHash) || tutor.SenhaHash.Length < 6)
                return BadRequest(new { mensagem = "A senha deve ter pelo menos 6 caracteres." });

            // O telefone é o contato do balcão com o cliente: DDD + número (10 ou 11 dígitos)
            var telefone = FormatarTelefone(tutor.Telefone);
            if (telefone == null)
                return BadRequest(new { mensagem = "Informe um telefone com DDD, por exemplo (11) 98765-4321." });
            tutor.Telefone = telefone;

            // O e-mail é o login: não pode repetir nem entre tutores nem com a equipe
            var email = tutor.Email;
            var cpf = tutor.Cpf;
            bool jaExiste = (await _tutorRepo.BuscarAsync(t => t.Email == email || t.Cpf == cpf)).Any()
                         || (await _funcionarioRepo.BuscarAsync(f => f.Email == email)).Any();
            if (jaExiste)
                return Conflict(new { mensagem = "Já existe um cadastro com este e-mail ou CPF." });

            // Aplica a criptografia BCrypt na senha antes de salvar no banco (Requisito RNF02)
            tutor.SenhaHash = BCrypt.Net.BCrypt.HashPassword(tutor.SenhaHash, 12);
            tutor.Id = 0;
            tutor.Ativo = true;
            tutor.DataCadastro = System.DateTime.Now;

            var id = await _tutorRepo.AdicionarAsync(tutor);

            try
            {
                await _emailService.EnviarEmailAsync(tutor.Email, "Boas-vindas ao PataFácil! 🐾",
                    $"Olá, {tutor.Nome}!\n\nSeu cadastro foi concluído. Pelo portal você cadastra seus pets, agenda banho, tosa e consultas e acompanha tudo por aqui.\n\nAs confirmações dos seus agendamentos chegam nesta caixa de e-mail.\n\nEquipe PataFácil");
            }
            catch { /* O cadastro não depende do e-mail */ }

            return CreatedAtAction(nameof(ObterPorId), new { id = id }, new { id = id, mensagem = "Tutor cadastrado com sucesso!" });
        }

        public class CadastroBalcaoDTO
        {
            public string Nome { get; set; } = string.Empty;
            public string Cpf { get; set; } = string.Empty;
            public string Telefone { get; set; } = string.Empty;
            public string? Email { get; set; }
        }

        // POST: api/tutores/balcao — a atendente cadastra o cliente que chegou pessoalmente.
        // Não pede senha: o cliente pode criar a própria depois, se quiser usar o portal.
        [HttpPost("balcao")]
        [Authorize(Roles = "Atendente,Gerente")]
        public async Task<IActionResult> CadastrarNoBalcao([FromBody] CadastroBalcaoDTO dto)
        {
            var nome = (dto.Nome ?? string.Empty).Trim();
            var cpf = new string((dto.Cpf ?? string.Empty).Where(char.IsDigit).ToArray());
            var telefone = FormatarTelefone(dto.Telefone);
            var email = (dto.Email ?? string.Empty).Trim().ToLower();

            if (nome.Length < 3)
                return BadRequest(new { mensagem = "Informe o nome completo do cliente." });
            if (cpf.Length != 11)
                return BadRequest(new { mensagem = "O CPF deve conter 11 dígitos." });
            if (telefone == null)
                return BadRequest(new { mensagem = "Informe um telefone com DDD, por exemplo (11) 98765-4321." });
            if (email.Length > 0 && !email.Contains('@'))
                return BadRequest(new { mensagem = "O e-mail informado não é válido." });

            // Sem e-mail, o cadastro recebe um endereço interno único (o CPF já é único)
            if (email.Length == 0) email = $"cliente{cpf}@balcao.patafacil";

            bool jaExiste = (await _tutorRepo.BuscarAsync(t => t.Email == email || t.Cpf == cpf)).Any()
                         || (await _funcionarioRepo.BuscarAsync(f => f.Email == email)).Any();
            if (jaExiste)
                return Conflict(new { mensagem = "Já existe um cliente com este CPF ou e-mail. Procure-o pelo nome na busca." });

            var tutor = new Tutor
            {
                Nome = nome,
                Cpf = cpf,
                Email = email,
                Telefone = telefone,
                // Senha aleatória: o login pelo portal só passa a valer quando o cliente definir a dele
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(System.Guid.NewGuid().ToString(), 12),
                DataCadastro = System.DateTime.Now,
                Ativo = true
            };
            var id = await _tutorRepo.AdicionarAsync(tutor);
            return Ok(new { id, nome, telefone, email, cpf, mensagem = "Cliente cadastrado." });
        }

        // Devolve "(11) 98765-4321" ou null quando não há DDD + número (10 ou 11 dígitos)
        private static string? FormatarTelefone(string? telefone)
        {
            var d = new string((telefone ?? string.Empty).Where(char.IsDigit).ToArray());
            if (d.Length < 10 || d.Length > 11) return null;
            return d.Length == 11 ? $"({d[..2]}) {d[2..7]}-{d[7..]}" : $"({d[..2]}) {d[2..6]}-{d[6..]}";
        }

        // GET: api/tutores/{id} (Buscar tutor específico)
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> ObterPorId(int id)
        {
            // O Tutor só consulta o próprio cadastro
            var idSeTutor = User.ObterIdSeTutor();
            if (idSeTutor.HasValue && idSeTutor.Value != id) return Forbid();

            var tutor = await _tutorRepo.ObterPorIdAsync(id);
            if (tutor == null)
                return NotFound(new { mensagem = "Tutor não encontrado." });

            return Ok(new {
                tutor.Id,
                tutor.Nome,
                tutor.Cpf,
                tutor.Email,
                tutor.Telefone
            }); // Retorna apenas os dados seguros
        }

        // GET: api/tutores (Listar todos) — dados de clientes: somente a equipe
        [HttpGet]
        [Authorize(Roles = "Atendente,Veterinario,Gerente")]
        public async Task<IActionResult> ListarTodos()
        {
            var tutores = await _tutorRepo.ObterTodosAsync();

            // O CPF entra para a busca do balcão (digitar nome, CPF ou telefone)
            var tutoresSeguros = tutores.Where(t => t.Ativo).OrderBy(t => t.Nome).Select(t => new {
                t.Id, t.Nome, t.Email, t.Telefone, t.Cpf
            }); // Protege os hashes de senhas e dados sensíveis
            return Ok(tutoresSeguros);
        }
    }
}
