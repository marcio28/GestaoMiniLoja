using System.ComponentModel.DataAnnotations;

namespace GestaoMiniLoja.Core.Models
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

        [Display(Name = "Preço")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public required string Preco { get; set; }

        [Display(Name = "Qtd. em estoque")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public required int Estoque { get; set; }

        [Display(Name = "Id categoria")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public required int CategoriaId { get; set; }

        [Display(Name = "Categoria")]
        public Categoria? Categoria { get; set; }

        [Display(Name = "Id vendedor")]
        public required Guid VendedorId { get; set; }

        [Display(Name = "Vendedor")]
        public Vendedor? Vendedor { get; set; }

        public String DescricaoDaCategoria => (Categoria == null) ? string.Empty : Categoria.Descricao;
    }
}
