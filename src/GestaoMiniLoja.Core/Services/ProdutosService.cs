using GestaoMiniLoja.Core.Exceptions;
using GestaoMiniLoja.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoMiniLoja.Core.Services
{
    public class ProdutosService(AppDbContext dbContext)
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<List<Produto>> ObterDisponiveisAsync() => await _dbContext.Produtos.Include(p => p.Categoria)
                                                                                             .Include(p => p.Vendedor)
                                                                                             .Where(p => p.Estoque > 0)
                                                                                             .ToListAsync();

        public async Task<List<Produto>> ObterPorVendedorAsync(string vendedorIdString) => await _dbContext.Produtos.Include(p => p.Categoria)
                                                                                                                    .Include(p => p.Vendedor)
                                                                                                                    .Where(p => p.VendedorId == Guid.Parse(vendedorIdString))
                                                                                                                    .ToListAsync();

        public async ValueTask<Produto?> ObterAsync(int id) => await _dbContext.Produtos.FindAsync(id);

        public async Task<Produto?> ObterOuDefaultAsync(int id) => await _dbContext.Produtos.Include(p => p.Categoria)
                                                                                            .Include(p => p.Vendedor)
                                                                                            .FirstOrDefaultAsync(m => m.Id == id);

        public async Task IncluirAsync(Produto produto)
        {
            if (!EstaConfigurado()) throw AcessoNaoConfigurado();

            var produtoExistente = await ObterPorNomeAsync(produto.Nome);
            if (produtoExistente != null) throw EntidadeJaExistente();

            _dbContext.Add(produto);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Produto produto)
        {
            if (!EstaConfigurado()) throw AcessoNaoConfigurado();

            var produtoExistente = await ObterPorNomeAsync(produto.Nome);
            if (produtoExistente != null && produtoExistente.Id != produto.Id) throw EntidadeJaExistente();

            _dbContext.Update(produto);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            if (!EstaConfigurado()) throw AcessoNaoConfigurado();

            var produto = await _dbContext.Produtos.FindAsync(id) ?? throw EntidadeNaoEncontrada();
            _dbContext.Produtos.Remove(produto);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<Produto?> ObterPorNomeAsync(string nome) => await _dbContext.Produtos.AsNoTracking().FirstOrDefaultAsync(c => c.Nome == nome);

        private bool EstaConfigurado() => _dbContext.Produtos != null;

        public async Task<bool> ExisteAsync(int id) => await _dbContext.Produtos.AnyAsync(e => e.Id == id);

        private static RegraDeNegocioException AcessoNaoConfigurado() => RegraDeNegocio("Acesso aos produtos não configurado.");

        private static RegraDeNegocioException EntidadeNaoEncontrada() => RegraDeNegocio("Produto não encontrado.");

        private static RegraDeNegocioException EntidadeJaExistente() => RegraDeNegocio("Produto já cadastrado.");

        private static RegraDeNegocioException RegraDeNegocio(string mensagem) => new(mensagem);
    }
}
