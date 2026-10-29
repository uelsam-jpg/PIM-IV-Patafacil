using System;

namespace PataFacil.Models
{
    // Abstração: Classe base que centraliza os atributos comuns. Não pode ser instanciada diretamente.
    public abstract class Pessoa
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;

        // Polimorfismo: Assinatura de método que obrigará as filhas a implementarem comportamentos diferentes
        public abstract string GetPerfil();
    }
}