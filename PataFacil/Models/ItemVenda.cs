namespace PataFacil.Models
{
    public class ItemVenda
    {
        public int Id { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        
        public decimal Subtotal { get; set; } // Valor gerado (Quantidade x PrecoUnitario)

        // Chaves Estrangeiras
        public int IdVenda { get; set; }
        public int IdProduto { get; set; }
    }
}