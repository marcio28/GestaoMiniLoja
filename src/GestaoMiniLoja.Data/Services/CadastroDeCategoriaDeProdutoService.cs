using GestaoMiniLoja.Data.Data;
using GestaoMiniLoja.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoMiniLoja.Data.Services
{
    public class CadastroDeCategoriaDeProdutoService(ApplicationDbContext dbContext)
    {
        public static readonly string MensagemAcessoNaoConfigurado = "Não configurado o acesso aos dados de categorias de produto.";
        public static readonly string MensagemInclusaoBemSucedida = "Categoria de produto incluída com sucesso.";
        public static readonly string MensagemEdicaoBemSucedida = "Categoria de produto editada com sucesso.";
        public static readonly string MensagemExclusaoBemSucedida = "Categoria de produto excluída com sucesso.";
        public static readonly string MensagemExclusaoProibidaPorAssociacaoAProduto = "Categoria de produto não pode ser excluída, porque há produtos associados a ela.";

        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<List<CategoriaDeProduto>> ObterTodos() => await _dbContext.CategoriasDeProduto.ToListAsync();

        public CategoriaDeProduto? Obter(int id) => _dbContext.CategoriasDeProduto.Find(id);

        public async ValueTask<CategoriaDeProduto?> ObterAsync(int id) => await _dbContext.CategoriasDeProduto.FindAsync(id);

        public async Task<CategoriaDeProduto?> ObterOuDefaultAsync(int id) => await _dbContext.CategoriasDeProduto.FirstOrDefaultAsync(m => m.Id == id);

        public async Task Incluir(CategoriaDeProduto categoriaDeProduto)
        {
            _dbContext.Add(categoriaDeProduto);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Atualizar(CategoriaDeProduto categoriaDeProduto)
        {
            _dbContext.Update(categoriaDeProduto);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Excluir(int id)
        {
            if (!EstaConfigurado()) 
                return;

            if (await ContemAssociacoes(id)) 
                return;

            var categoriaDeProduto = await _dbContext.CategoriasDeProduto.FindAsync(id);

            if (categoriaDeProduto == null) 
                return;

            _dbContext.CategoriasDeProduto.Remove(categoriaDeProduto);
            await _dbContext.SaveChangesAsync();
        }

        public bool EstaConfigurado()
        {
            return _dbContext.CategoriasDeProduto != null;
        }

        public async Task<bool> Existe(int id)
        {
            return await _dbContext.CategoriasDeProduto.AnyAsync(e => e.Id == id);
        }

        public Task<bool> ContemAssociacoes(int id)
        {
            return _dbContext.Produtos.Where(p => p.CategoriaDeProdutoId == id).AnyAsync();
        }
    }
}
