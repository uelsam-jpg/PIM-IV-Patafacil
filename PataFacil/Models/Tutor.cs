using System;

namespace PataFacil.Models
{
    // Herança: Tutor ganha Id, Nome, Cpf, Email e Ativo da classe Pessoa
    public class Tutor : Pessoa
    {
        public string Telefone { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public string SenhaHash { get; set; } = string.Empty;

        // Polimorfismo: Tutor se identifica no sistema apenas como "Tutor"
        public override string GetPerfil()
        {
            return "Tutor";
        }
    }
}