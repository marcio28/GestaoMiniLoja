namespace GestaoMiniLoja.Api.Models
{
    public class JwtSettings
    {
        public string? Segredo { get; set; }
        public int ExpiracaoEmHoras { get; set; }
        public string? Emissor { get; set; }
        public string? Audiencia { get; set; }
    }
}
