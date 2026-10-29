using Microsoft.AspNetCore.Mvc;
using PataFacil.Models;
using PataFacil.DAL;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System;

namespace PataFacil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRepositorio<Tutor> _tutorRepo;
        private readonly IRepositorio<Funcionario> _funcionarioRepo;
        private readonly IConfiguration _configuration;

        public AuthController(IRepositorio<Tutor> tutorRepo, IRepositorio<Funcionario> funcionarioRepo, IConfiguration configuration)
        {
            _tutorRepo = tutorRepo;
            _funcionarioRepo = funcionarioRepo;
            _configuration = configuration;
        }

        public class LoginDTO
        {
            public string Email { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO credenciais)
        {
            var emailInformado = (credenciais.Email ?? string.Empty).Trim().ToLower();
            if (emailInformado.Length == 0 || string.IsNullOrEmpty(credenciais.Senha))
                return BadRequest(new { mensagem = "Informe e-mail e senha." });

            // Polimorfismo: Tutor e Funcionario são tratados como Pessoa a partir daqui
            Pessoa? pessoa = (await _tutorRepo.BuscarAsync(t => t.Email.ToLower() == emailInformado)).FirstOrDefault();
            string senhaHashBanco = (pessoa as Tutor)?.SenhaHash ?? string.Empty;

            if (pessoa == null)
            {
                var funcionario = (await _funcionarioRepo.BuscarAsync(f => f.Email.ToLower() == emailInformado)).FirstOrDefault();
                pessoa = funcionario;
                senhaHashBanco = funcionario?.SenhaHash ?? string.Empty;
            }

            // A mesma mensagem para e-mail inexistente e senha errada: não revela quais e-mails têm cadastro
            if (pessoa == null || !SenhaConfere(credenciais.Senha, senhaHashBanco))
                return Unauthorized(new { mensagem = "E-mail ou senha incorretos." });

            if (!pessoa.Ativo)
                return Unauthorized(new { mensagem = "Usuário inativo. Procure a gerência." });

            string perfil = pessoa.GetPerfil();

            // Geração do Token JWT Real
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "MinhaChaveSuperSecretaPataFacil2025!");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, pessoa.Id.ToString()),
                    new Claim(ClaimTypes.Email, pessoa.Email),
                    new Claim(ClaimTypes.Role, perfil) // Insere a Role (Tutor, Atendente, etc.)
                }),
                Expires = DateTime.UtcNow.AddHours(8), // Exigência RNF02: Expiração de 8 horas
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                usuario = new { id = pessoa.Id, nome = pessoa.Nome, email = pessoa.Email, perfil = perfil }
            });
        }

        // As senhas ficam sempre em hash BCrypt; um valor que não seja hash válido nunca autentica
        private static bool SenhaConfere(string senha, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(senha, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}
