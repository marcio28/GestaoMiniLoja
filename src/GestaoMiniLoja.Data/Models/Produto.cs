using System.ComponentModel.DataAnnotations;

namespace GestaoMiniLoja.Data.Models
{
    public class Produto
    {
        public required int Id { get; set; }

        [Display(Name = "Nome do produto")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo {0} deve conter {2} a {1} caracter(es).")]
        public required string Nome { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(500, ErrorMessage = "O campo {0} não pode conter mais de {1} caracteres.")]
        public string? Descricao { get; set; }

        [Display(Name = "Caminho da imagem")]
        [StringLength(200, ErrorMessage = "O campo {0} não pode conter mais de {1} caracteres.")]
        public string? CaminhoDaImagem { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Preço unitário")]
        [Range(0.01, 9999999.99, ErrorMessage = "O {0} deve estar entre {1} e {2}.")]
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
