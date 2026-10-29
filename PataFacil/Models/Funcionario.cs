using System;

namespace PataFacil.Models
{
    // Herança: Funcionario ganha Id, Nome, Cpf, Email e Ativo da classe Pessoa
    public class Funcionario : Pessoa
    {
        public Cargo Cargo { get; set; }
        public string SenhaHash { get; set; } = string.Empty;
        public DateTime DataAdmissao { get; set; } = DateTime.Now;

        // Polimorfismo: O perfil do Funcionario depende do cargo que ele ocupa (Atendente, Gerente, etc.)
        public override string GetPerfil()
        {
            return Cargo.ToString();
        }
    }
}