using System.ComponentModel.DataAnnotations;

namespace GestaoMiniLoja.Data.Models
{
    public class CategoriaDeProduto
    {
        public required int Id { get; set; }

        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O campo {0} deve conter {2} a {1} caracter(es).")]
        public required string Descricao { get; set; }
    }
}
