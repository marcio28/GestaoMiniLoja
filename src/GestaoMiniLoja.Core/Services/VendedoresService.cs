using GestaoMiniLoja.Core.Models;

namespace GestaoMiniLoja.Core.Services
{
    public class VendedoresService(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        public async Task IncluirSeNaoExisteAsync(Guid id)
        {
            if (!_context.Vendedores.Any(v => v.Id == id))
            {
                _context.Vendedores.Add(new Vendedor() { Id = id });
                await _context.SaveChangesAsync();
            }
        }
    }
}
