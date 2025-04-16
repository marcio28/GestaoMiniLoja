using GestaoMiniLoja.Core.Models;

namespace GestaoMiniLoja.Core.Services
{
    public class VendedoresService(AppDbContext dbContext)
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task IncluirSeNaoExiste(Guid id)
        {
            if (!_dbContext.Vendedores.Any(v => v.Id == id))
            {
                _dbContext.Vendedores.Add(new Vendedor() { Id = id });
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
