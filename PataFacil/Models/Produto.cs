namespace PataFacil.Models
{
    public enum CategoriaProdutoEnum { Alimento, Medicamento, Acessorio, Higiene, Outro }

    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public CategoriaProdutoEnum Categoria { get; set; }
        public string Marca { get; set; } = string.Empty;
        public decimal PrecoCusto { get; set; }
        public decimal PrecoVenda { get; set; }
        public string UnidadeMedida { get; set; } = string.Empty;
        public string CodigoBarras { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
    }
}