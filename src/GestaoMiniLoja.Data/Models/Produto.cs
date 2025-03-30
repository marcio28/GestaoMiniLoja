namespace GestaoMiniLoja.Data.Models
{
    public class Produto
    {
        public required int Id { get; set; }
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
        public string? CaminhoDaImagem { get; set; }
        public required decimal PrecoUnitario { get; set; }
        public required int QuantidadeEmEstoque { get; set; }
        public required int CategoriaDeProdutoId { get; set; }
        public required CategoriaDeProduto CategoriaDeProduto { get; set; }
        public required Guid VendedorId { get; set; }
        public required Vendedor Vendedor { get; set; }
    }
}
