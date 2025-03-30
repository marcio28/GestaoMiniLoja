using System.ComponentModel.DataAnnotations;

namespace GestaoMiniLoja.Data.Models
{
    public class CategoriaDeProduto
    {
        public required int Id { get; set; }
        [Display(Name = "Descrição")]
        public required string Descricao { get; set; }
    }
}
