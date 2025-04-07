using GestaoMiniLoja.Data.Data;
using GestaoMiniLoja.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoMiniLoja.Data.Services
{
    public class CadastroDeProdutoService(ApplicationDbContext dbContext)
    {
        public static readonly string MensagemAcessoNaoConfigurado = "Não configurado o acesso aos dados de produtos.";
        public static readonly string MensagemAcessoNaoPermitido = "Sem permissão para acessar esse produto.";
        public static readonly string MensagemEntidadeJaExistente = "Já existe um produto com esse nome.";
        public static readonly string MensagemEntidadeNaoEncontrada = "Produto não encontrado.";
        public static readonly string MensagemInclusaoBemSucedida = "Produto incluído com sucesso.";
        public static readonly string MensagemAtualizacaoBemSucedida = "Produto atualizado com sucesso.";
        public static readonly string MensagemAtualizacaoNaoPermitida = "Sem permissão para alterar esse produto.";
        public static readonly string MensagemAtualizacaoMalSucedidaPorConcorrencia = "Produto atualizado por outro usuário. Tente novamente.";
        public static readonly string MensagemExclusaoBemSucedida = "Produto excluído com sucesso.";
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<List<Produto>> ObterDisponiveisEmEstoqueAsync()
        {
            var queryable = _dbContext.Produtos.Include(p => p.CategoriaDeProduto)
                                               .Include(p => p.Vendedor)
                                               .Where(p => p.QuantidadeEmEstoque > 0);
            return await queryable.ToListAsync();
        }

        public async Task<List<Produto>> ObterPorVendedorAsync(string vendedorIdString)
        {
            Guid usuarioGuid = Guid.Parse(vendedorIdString);
            var queryable = _dbContext.Produtos.Include(p => p.CategoriaDeProduto)
                                             .Include(p => p.Vendedor)
                                             .Where(p => p.VendedorId == usuarioGuid);
            return await queryable.ToListAsync();
        }

        public async ValueTask<Produto?> ObterAsync(int id) => await _dbContext.Produtos.FindAsync(id);

        public async Task<Produto?> ObterOuDefaultAsync(int id) => await _dbContext.Produtos.Include(p => p.CategoriaDeProduto)
                                                                                            .Include(p => p.Vendedor)
                                                                                            .FirstOrDefaultAsync(m => m.Id == id);

        public async Task IncluirAsync(Produto produto)
        {
            if (!EstaConfigurado())
                throw new RegraDeNegocioException(MensagemAcessoNaoConfigurado);

            var produtoExistente = await ObterPorNomeAsync(produto.Nome);
            if (produtoExistente != null)
                throw new RegraDeNegocioException(MensagemEntidadeJaExistente);

            _dbContext.Add(produto);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Produto produto)
        {
            if (!EstaConfigurado())
                throw new RegraDeNegocioException(MensagemAcessoNaoConfigurado);

            var produtoExistente = await ObterPorNomeAsync(produto.Nome);
            if (produtoExistente != null && produtoExistente.Id != produto.Id)
                throw new RegraDeNegocioException(MensagemEntidadeJaExistente);


            _dbContext.Update(produto);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            if (!EstaConfigurado())
                throw new RegraDeNegocioException(MensagemAcessoNaoConfigurado);

            var produto = await _dbContext.Produtos.FindAsync(id);
            if (produto == null)
                throw new RegraDeNegocioException(MensagemEntidadeNaoEncontrada);

            _dbContext.Produtos.Remove(produto);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<Produto?> ObterPorNomeAsync(string nome)
        {
            return await _dbContext.Produtos.AsNoTracking().FirstOrDefaultAsync(c => c.Nome == nome);
        }

        private bool EstaConfigurado()
        {
            return _dbContext.Produtos != null;
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _dbContext.Produtos.AnyAsync(e => e.Id == id);
        }
    }
}
