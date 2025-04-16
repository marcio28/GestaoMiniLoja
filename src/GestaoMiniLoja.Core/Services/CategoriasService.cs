using GestaoMiniLoja.Core.Exceptions;
using GestaoMiniLoja.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoMiniLoja.Core.Services
{
    public class CategoriasService(AppDbContext dbContext)
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<List<Categoria>> ObterTodosAsync() => await _dbContext.Categorias.ToListAsync();

        public Categoria? Obter(int id) => _dbContext.Categorias.Find(id);

        public async ValueTask<Categoria?> ObterAsync(int id) => await _dbContext.Categorias.FindAsync(id);

        public async Task<Categoria?> ObterOuDefaultAsync(int id) => await _dbContext.Categorias.FirstOrDefaultAsync(m => m.Id == id);

        public async Task IncluirAsync(Categoria categoria)
        {
            if (!EstaConfigurado()) throw AcessoNaoConfigurado();

            _ = await ObterPorDescricaoAsync(categoria.Descricao) ?? throw EntidadeJaExistente();

            _dbContext.Add(categoria);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<Categoria?> ObterPorDescricaoAsync(string descricao) => await _dbContext.Categorias.FirstOrDefaultAsync(c => c.Descricao == descricao);

        public async Task AtualizarAsync(Categoria categoria)
        {
            if (!EstaConfigurado()) throw AcessoNaoConfigurado();

            var categoriaExistente = await ObterPorDescricaoAsync(categoria.Descricao);
            if (categoriaExistente != null && categoriaExistente.Id != categoria.Id) throw EntidadeJaExistente();

            _dbContext.Update(categoria);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            if (!EstaConfigurado()) throw AcessoNaoConfigurado();

            var categoria = await _dbContext.Categorias.FindAsync(id) ?? throw EntidadeNaoEncontrada();
            if (await ContemAssociacoesAsync(id)) throw EntidadeComAssociaces();

            _dbContext.Categorias.Remove(categoria);
            await _dbContext.SaveChangesAsync();
        }

        private bool EstaConfigurado() => _dbContext.Categorias != null;

        public async Task<bool> ExisteAsync(int id) => await _dbContext.Categorias.AnyAsync(e => e.Id == id);

        private Task<bool> ContemAssociacoesAsync(int id) => _dbContext.Produtos.Where(p => p.CategoriaId == id).AnyAsync();

        private static RegraDeNegocioException AcessoNaoConfigurado() => RegraDeNegocio("Acesso às categorias não configurado.");

        private static RegraDeNegocioException EntidadeNaoEncontrada() => RegraDeNegocio("Categoria não encontrada.");

        private static RegraDeNegocioException EntidadeJaExistente() => RegraDeNegocio("Categoria já cadastrada.");

        private static RegraDeNegocioException EntidadeComAssociaces() => RegraDeNegocio("Categoria não pode ser excluída, porque há produtos associações a ela.");

        private static RegraDeNegocioException RegraDeNegocio(string mensagem) => new(mensagem);
    }
}
