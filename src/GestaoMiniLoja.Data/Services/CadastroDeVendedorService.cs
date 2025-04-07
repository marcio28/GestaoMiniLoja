using GestaoMiniLoja.Data.Data;
using GestaoMiniLoja.Data.Models;

namespace GestaoMiniLoja.Data.Services
{
    public class CadastroDeVendedorService(ApplicationDbContext dbContext)
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task IncluirSeNaoExiste(Guid id)
        {
            if (!_dbContext.Vendedores.Any(v => v.Id == id))
            {
                Vendedor vendedor = new() { Id = id };
                _dbContext.Vendedores.Add(vendedor);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
