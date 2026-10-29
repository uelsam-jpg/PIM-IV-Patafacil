using System;

namespace PataFacil.Models
{
    public class Estoque
    {
        public int Id { get; set; }
        public int QuantidadeAtual { get; set; }
        public int PontoReposicao { get; set; }
        public int QuantidadeReposicao { get; set; }
        public DateTime UltimaAtualizacao { get; set; } = DateTime.Now;

        // Chave Estrangeira
        public int IdProduto { get; set; }
    }
}