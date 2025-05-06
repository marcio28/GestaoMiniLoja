using GestaoMiniLoja.Core.Exceptions;
using GestaoMiniLoja.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoMiniLoja.Core.Services
{
    public class CategoriasService(AppDbContext context)
    {
        readonly AppDbContext _context = context;

        public async Task<List<Categoria>> ObterTodosAsync() => await _context.Categorias.ToListAsync();

        public Categoria? Obter(int id) => _context.Categorias.Find(id);

        public async ValueTask<Categoria?> ObterAsync(int id) => await _context.Categorias.FindAsync(id);

        public async Task<Categoria?> ObterOuDefaultAsync(int id) => await _context.Categorias.FirstOrDefaultAsync(m => m.Id == id);

        public async Task IncluirAsync(Categoria categoria)
        {
            if (!EstaConfigurado()) throw AcessoNaoConfigurado();

            if (await ExisteDescricaoAsync(categoria.Descricao)) throw EntidadeJaExistente();

            _context.Add(categoria);
            await _context.SaveChangesAsync();
        }

        async Task<Categoria?> ObterPorDescricaoAsync(string descricao) => await _context.Categorias.FirstOrDefaultAsync(c => c.Descricao == descricao);

        public async Task AtualizarAsync(Categoria categoria)
        {
            if (!EstaConfigurado()) throw AcessoNaoConfigurado();

            var categoriaExistente = await ObterPorDescricaoAsync(categoria.Descricao);
            if (categoriaExistente != null && categoriaExistente.Id != categoria.Id) throw EntidadeJaExistente();

            _context.Update(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            if (!EstaConfigurado()) throw AcessoNaoConfigurado();

            var categoria = await _context.Categorias.FindAsync(id) ?? throw EntidadeNaoEncontrada();
            if (await ContemAssociacoesAsync(id)) throw EntidadeComAssociaces();

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
        }

        public bool EstaConfigurado() => _context.Categorias != null;

        public async Task<bool> ExisteAsync(int id) => await _context.Categorias.AnyAsync(e => e.Id == id);

        public async Task<bool> ExisteDescricaoAsync(string descricao) => await _context.Categorias.AnyAsync(c => c.Descricao == descricao);

        Task<bool> ContemAssociacoesAsync(int id) => _context.Produtos.Where(p => p.CategoriaId == id).AnyAsync();

        static RegraDeNegocioException AcessoNaoConfigurado() => RegraDeNegocio("Acesso às categorias não configurado.");

        static RegraDeNegocioException EntidadeNaoEncontrada() => RegraDeNegocio("Categoria não encontrada.");

        static RegraDeNegocioException EntidadeJaExistente() => RegraDeNegocio("Categoria já cadastrada.");

        static RegraDeNegocioException EntidadeComAssociaces() => RegraDeNegocio("Categoria não pode ser excluída, porque há produtos associados a ela.");

        static RegraDeNegocioException RegraDeNegocio(string mensagem) => new(mensagem);
    }
}
