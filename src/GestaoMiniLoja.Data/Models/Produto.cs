using System.ComponentModel.DataAnnotations;

namespace GestaoMiniLoja.Data.Models
{
    public class Produto
    {
        public required int Id { get; set; }

        [Display(Name = "Nome do produto")] 
        public required string Nome { get; set; }

        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Display(Name = "Caminho da imagem")]
        public string? CaminhoDaImagem { get; set; }

        [Display(Name = "Preço unitário")]
        public required decimal PrecoUnitario { get; set; }

        [Display(Name = "Qtd. em estoque")]
        public required int QuantidadeEmEstoque { get; set; }

        [Display(Name = "Id categoria")]
        public required int CategoriaDeProdutoId { get; set; }

        [Display(Name = "Categoria")]
        public required CategoriaDeProduto CategoriaDeProduto { get; set; }

        [Display(Name = "Id vendedor")]
        public required Guid VendedorId { get; set; }

        [Display(Name = "Vendedor")]
        public required Vendedor Vendedor { get; set; }
    }
}
