using System;

namespace PataFacil.Models
{
    // Recebimento de mercadoria do fornecedor: guarda a nota, o custo e a validade de cada lote
    public class EntradaEstoque
    {
        public int Id { get; set; }
        public int IdProduto { get; set; }
        public int Quantidade { get; set; }
        public string NumeroNota { get; set; } = string.Empty;
        public string Fornecedor { get; set; } = string.Empty;
        public DateTime? Validade { get; set; }          // nem todo produto tem validade (ex.: brinquedos)
        public decimal CustoUnitario { get; set; }
        public decimal PrecoVendaAplicado { get; set; }   // preço de venda definido nesta entrada
        public DateTime DataEntrada { get; set; } = DateTime.Now;
        public int IdFuncionario { get; set; }
    }
}
