using System;

namespace PataFacil.Models
{
    public enum FormaPagamentoEnum { Dinheiro, CartaoCredito, CartaoDebito, Pix }

    public class Venda
    {
        public int Id { get; set; }
        public DateTime DataVenda { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public FormaPagamentoEnum FormaPagamento { get; set; }
        public string? Observacoes { get; set; }

        // Chaves Estrangeiras
        public int? IdTutor { get; set; } // Opcional: venda de balcão para cliente sem cadastro
        public int IdFuncionario { get; set; }
    }
}