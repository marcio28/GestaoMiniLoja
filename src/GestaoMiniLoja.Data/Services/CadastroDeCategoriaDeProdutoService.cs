using GestaoMiniLoja.Data.Data;
using GestaoMiniLoja.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoMiniLoja.Data.Services
{
    public class CadastroDeCategoriaDeProdutoService(ApplicationDbContext dbContext)
    {
        public static readonly string MensagemAcessoNaoConfigurado = "Não configurado o acesso aos dados de categorias de produto.";
        public static readonly string MensagemEntidadeJaExistente = "Já existe uma categoria de produto com essa descrição.";
        public static readonly string MensagemEntidadeNaoEncontrada = "Categoria de produto não encontrada.";
        public static readonly string MensagemInclusaoBemSucedida = "Categoria de produto incluída com sucesso.";
        public static readonly string MensagemAtualizacaoBemSucedida = "Categoria de produto atualizada com sucesso.";
        public static readonly string MensagemAtualizacaoMalSucedidaPorConcorrencia = "Categoria de produto atualizada por outro usuário. Tente novamente.";
        public static readonly string MensagemExclusaoBemSucedida = "Categoria de produto excluída com sucesso.";

        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<List<CategoriaDeProduto>> ObterTodosAsync() => await _dbContext.CategoriasDeProduto.ToListAsync();

        public CategoriaDeProduto? Obter(int id) => _dbContext.CategoriasDeProduto.Find(id);

        public async ValueTask<CategoriaDeProduto?> ObterAsync(int id) => await _dbContext.CategoriasDeProduto.FindAsync(id);

        public async Task<CategoriaDeProduto?> ObterOuDefaultAsync(int id) => await _dbContext.CategoriasDeProduto.FirstOrDefaultAsync(m => m.Id == id);

        public async Task IncluirAsync(CategoriaDeProduto categoriaDeProduto)
        {
            if (!EstaConfigurado())
                throw new RegraDeNegocioException(MensagemAcessoNaoConfigurado);

            var categoriaExistente = await ObterPorDescricaoAsync(categoriaDeProduto.Descricao);
            if (categoriaExistente != null)
                throw new RegraDeNegocioException(MensagemEntidadeJaExistente);

            _dbContext.Add(categoriaDeProduto);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<CategoriaDeProduto?> ObterPorDescricaoAsync(string descricao)
        {
            return await _dbContext.CategoriasDeProduto.FirstOrDefaultAsync(c => c.Descricao == descricao);
        }

        public async Task AtualizarAsync(CategoriaDeProduto categoriaDeProduto)
        {
            if (!EstaConfigurado())
                throw new RegraDeNegocioException(MensagemAcessoNaoConfigurado);

            var categoriaExistente = await ObterPorDescricaoAsync(categoriaDeProduto.Descricao);
            if (categoriaExistente != null && categoriaExistente.Id != categoriaDeProduto.Id)
                throw new RegraDeNegocioException(MensagemEntidadeJaExistente);


            _dbContext.Update(categoriaDeProduto);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            if (!EstaConfigurado())
                throw new RegraDeNegocioException(MensagemAcessoNaoConfigurado);

            var categoriaDeProduto = await _dbContext.CategoriasDeProduto.FindAsync(id);
            if (categoriaDeProduto == null)
                throw new RegraDeNegocioException(MensagemEntidadeNaoEncontrada);

            if (await ContemAssociacoesAsync(id))
                throw new RegraDeNegocioException($"A categoria de produto '{categoriaDeProduto.Descricao}' não pode ser excluída, porque há produtos associados a ela.");

            _dbContext.CategoriasDeProduto.Remove(categoriaDeProduto);
            await _dbContext.SaveChangesAsync();
        }

        private bool EstaConfigurado()
        {
            return _dbContext.CategoriasDeProduto != null;
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _dbContext.CategoriasDeProduto.AnyAsync(e => e.Id == id);
        }

        private Task<bool> ContemAssociacoesAsync(int id)
        {
            return _dbContext.Produtos.Where(p => p.CategoriaDeProdutoId == id).AnyAsync();
        }
    }
}
