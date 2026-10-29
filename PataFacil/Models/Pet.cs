using System;

namespace PataFacil.Models
{
    public enum Especie { Cachorro, Gato, Ave, Roedor, Outro }
    public enum Sexo { Macho, Femea }

    public class Pet
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public Especie Especie { get; set; }
        public string Raca { get; set; } = string.Empty;
        public Sexo Sexo { get; set; }
        public DateTime DataNascimento { get; set; }
        public decimal PesoKg { get; set; }
        public string FotoUrl { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;

        // Relacionamento (Chave Estrangeira): A qual tutor este pet pertence?
        public int TutorId { get; set; }
        
        // Propriedade de navegação (Opcional, mas útil para o Entity Framework)
        // public Tutor? Tutor { get; set; } 
    }
}