using System;
using System.Linq;
using PataFacil.Models;

namespace PataFacil.DAL
{
    // Clientes, pets e agendamentos de exemplo (semana passada, esta semana e a próxima),
    // para a agenda do balcão já ter o que mostrar. Roda uma única vez: só se o cliente de
    // demonstração ainda não existir.
    public static class DadosDemonstracao
    {
        private const string EmailCliente = "cliente@patafacil.com";

        public static void Criar(PataFacilContext db)
        {
            if (db.Tutores.Any(t => t.Email == EmailCliente)) return;

            var senha = BCrypt.Net.BCrypt.HashPassword("123456", 12);
            var maria = new Tutor { Nome = "Maria Souza", Cpf = "66666666666", Email = EmailCliente, Telefone = "(11) 98765-4321", SenhaHash = senha };
            var joao = new Tutor { Nome = "João Pereira", Cpf = "77777777777", Email = "joao.pereira@exemplo.com", Telefone = "(11) 91234-5678", SenhaHash = senha };
            db.Tutores.AddRange(maria, joao);
            db.SaveChanges();

            var thor = new Pet { Nome = "Thor", Especie = Especie.Cachorro, Raca = "Golden Retriever", Sexo = Sexo.Macho, DataNascimento = DateTime.Today.AddYears(-4), PesoKg = 32, TutorId = maria.Id };
            var mel = new Pet { Nome = "Mel", Especie = Especie.Gato, Raca = "SRD", Sexo = Sexo.Femea, DataNascimento = DateTime.Today.AddYears(-2), PesoKg = 4.2m, TutorId = maria.Id };
            var bob = new Pet { Nome = "Bob", Especie = Especie.Cachorro, Raca = "Shih-tzu", Sexo = Sexo.Macho, DataNascimento = DateTime.Today.AddYears(-6), PesoKg = 7.5m, TutorId = joao.Id };
            db.Pets.AddRange(thor, mel, bob);
            db.SaveChanges();

            var banho = db.Servicos.First(s => s.Nome.Contains("Banho"));
            var consulta = db.Servicos.First(s => s.Nome.Contains("Consulta"));
            var tosadores = db.Funcionarios.Where(f => f.Cargo == Cargo.Tosador && f.Ativo).OrderBy(f => f.Id).ToList();
            var veterinario = db.Funcionarios.First(f => f.Cargo == Cargo.Veterinario && f.Ativo);
            if (tosadores.Count == 0) return;

            // Segunda-feira da semana atual; domingo não tem expediente (RN03)
            var hoje = DateTime.Today.DayOfWeek == DayOfWeek.Sunday ? DateTime.Today.AddDays(1) : DateTime.Today;
            var segunda = hoje.AddDays(-(((int)hoje.DayOfWeek + 6) % 7));

            Agendamento Novo(DateTime dia, int hora, Servico servico, Funcionario profissional, Pet pet, string? obsCliente = null)
            {
                var inicio = dia.Date.AddHours(hora);
                return new Agendamento
                {
                    IdPet = pet.Id, IdServico = servico.Id, IdFuncionario = profissional.Id,
                    DataHoraInicio = inicio, DataHoraFim = inicio.AddMinutes(servico.DuracaoMinutos),
                    Observacoes = obsCliente, DataCriacao = inicio.AddDays(-3)
                };
            }

            // Semana passada: atendimentos finalizados e pagos
            var passados = new[]
            {
                Novo(segunda.AddDays(-7), 9, banho, tosadores[0], thor),
                Novo(segunda.AddDays(-5), 14, banho, tosadores[1 % tosadores.Count], bob),
                Novo(segunda.AddDays(-3), 10, consulta, veterinario, mel, "Está espirrando bastante.")
            };
            var formas = new[] { FormaPagamentoEnum.Pix, FormaPagamentoEnum.CartaoDebito, FormaPagamentoEnum.Dinheiro };
            for (int i = 0; i < passados.Length; i++)
            {
                passados[i].RegistrarChegada();
                passados[i].RegistrarPagamento(i == 2 ? consulta.Preco : banho.Preco, formas[i]);
            }

            // Hoje: um cliente já no balcão, um confirmado e um pendente
            var emProcesso = Novo(hoje, 9, banho, tosadores[0], bob);
            emProcesso.RegistrarChegada();
            emProcesso.ObservacoesAtendimento = "Pelo com nós atrás das orelhas. Tutor pediu perfume suave.";
            var confirmado = Novo(hoje, 15, banho, tosadores[2 % tosadores.Count], thor);
            confirmado.Confirmar();
            var pendente = Novo(hoje, 16, consulta, veterinario, mel, "Revisão do tratamento.");

            // Próxima semana
            var proximos = new[]
            {
                Novo(segunda.AddDays(8), 10, banho, tosadores[1 % tosadores.Count], thor),
                Novo(segunda.AddDays(10), 11, banho, tosadores[0], bob)
            };

            db.Agendamentos.AddRange(passados);
            db.Agendamentos.AddRange(emProcesso, confirmado, pendente);
            db.Agendamentos.AddRange(proximos);
            db.SaveChanges();
        }
    }
}
