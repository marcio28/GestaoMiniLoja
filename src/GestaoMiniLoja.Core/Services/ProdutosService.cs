using GestaoMiniLoja.Core.Exceptions;
using GestaoMiniLoja.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoMiniLoja.Core.Services
{
    public class ProdutosService(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        public async Task<List<Produto>> ObterTodosAsync() => await _context.Produtos.Include(p => p.Categoria)
                                                                                     .Include(p => p.Vendedor)
                                                                                     .ToListAsync();

        public async Task<List<Produto>> ObterPorCategoriaAsync(int categoriaId) => await _context.Produtos.Include(p => p.Categoria)
                                                                                                           .Include(p => p.Vendedor)
                                                                                                           .Where(p => p.CategoriaId == categoriaId)
                                                                                                           .ToListAsync();

        public async Task<List<Produto>> ObterPorVendedorAsync(Guid vendedorId) => await _context.Produtos.Include(p => p.Categoria)
                                                                                                                  .Include(p => p.Vendedor)
                                                                                                                  .Where(p => p.VendedorId == vendedorId)
                                                                                                                  .ToListAsync();

        public async ValueTask<Produto?> ObterAsync(int id) => await _context.Produtos.FindAsync(id);

        public async Task<Produto?> ObterOuDefaultAsync(int id) => await _context.Produtos.Include(p => p.Categoria)
                                                                                          .Include(p => p.Vendedor)
                                                                                          .FirstOrDefaultAsync(m => m.Id == id);

        public async Task IncluirAsync(Produto produto)
        {
            var produtoExistente = await ObterPorNomeAsync(produto.Nome);
            if (produtoExistente != null) throw EntidadeJaExistente();

            _context.Add(produto);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Produto produto)
        {
            var produtoExistente = await ObterPorNomeAsync(produto.Nome);
            if (produtoExistente != null && produtoExistente.Id != produto.Id) throw EntidadeJaExistente();

            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id) ?? throw EntidadeNaoEncontrada();

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
        }

        private async Task<Produto?> ObterPorNomeAsync(string nome) => await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(c => c.Nome == nome);

        public bool EstaConfigurado() => _context.Produtos != null;

        public async Task<bool> ExisteAsync(int id) => await _context.Produtos.AnyAsync(e => e.Id == id);

        private static RegraDeNegocioException EntidadeNaoEncontrada() => RegraDeNegocio("Produto não encontrado.");

        private static RegraDeNegocioException EntidadeJaExistente() => RegraDeNegocio("Produto já cadastrado.");

        private static RegraDeNegocioException RegraDeNegocio(string mensagem) => new(mensagem);
    }
}
