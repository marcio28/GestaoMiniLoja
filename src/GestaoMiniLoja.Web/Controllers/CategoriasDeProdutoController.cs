using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoMiniLoja.Data.Data;
using GestaoMiniLoja.Data.Models;
using GestaoMiniLoja.Data.Services;

namespace GestaoMiniLoja.Web.Controllers
{
    [Route("categorias-de-produto")]
    public class CategoriasDeProdutoController(ApplicationDbContext context) : Controller
    {
        private readonly CadastroDeCategoriaDeProdutoService _cadastroDeCategoriaDeProduto = new(context);

        public async Task<IActionResult> Index()
        {
            if (!_cadastroDeCategoriaDeProduto.EstaConfigurado())
                return Problem(CadastroDeCategoriaDeProdutoService.MensagemAcessoNaoConfigurado);

            return View(await _cadastroDeCategoriaDeProduto.ObterTodos());
        }

        [Route("detalhes/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            if (!_cadastroDeCategoriaDeProduto.EstaConfigurado())
                return Problem(CadastroDeCategoriaDeProdutoService.MensagemAcessoNaoConfigurado);

            var categoriaDeProduto = await _cadastroDeCategoriaDeProduto.ObterOuDefaultAsync(id);

            if (categoriaDeProduto == null)
                return NotFound();

            return View(categoriaDeProduto);
        }

        [Route("nova")]
        public IActionResult Create() => View();
   
        [HttpPost("nova")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descricao")] CategoriaDeProduto categoriaDeProduto)
        {
            if (!_cadastroDeCategoriaDeProduto.EstaConfigurado())
                return Problem(CadastroDeCategoriaDeProdutoService.MensagemAcessoNaoConfigurado);

            if (ModelState.IsValid)
            {
                await _cadastroDeCategoriaDeProduto.Incluir(categoriaDeProduto);

                TempData["Sucesso"] = CadastroDeCategoriaDeProdutoService.MensagemInclusaoBemSucedida;

                return RedirectToAction(nameof(Index));
            }

            return View(categoriaDeProduto);
        }

        [Route("editar/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (!_cadastroDeCategoriaDeProduto.EstaConfigurado()) 
                return Problem(CadastroDeCategoriaDeProdutoService.MensagemAcessoNaoConfigurado);

            var categoriaDeProduto = await _cadastroDeCategoriaDeProduto.ObterAsync(id);

            if (categoriaDeProduto == null) 
                return NotFound();

            return View(categoriaDeProduto);
        }

        [HttpPost("editar/{id:int}"), ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descricao")] CategoriaDeProduto categoriaDeProduto)
        {
            if (!_cadastroDeCategoriaDeProduto.EstaConfigurado())
                return Problem(CadastroDeCategoriaDeProdutoService.MensagemAcessoNaoConfigurado);

            if (id != categoriaDeProduto.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await _cadastroDeCategoriaDeProduto.Atualizar(categoriaDeProduto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _cadastroDeCategoriaDeProduto.Existe(categoriaDeProduto.Id))
                        return NotFound();
                    else
                        throw;
                }

                TempData["Sucesso"] = CadastroDeCategoriaDeProdutoService.MensagemEdicaoBemSucedida;

                return RedirectToAction(nameof(Index));
            }

            return View(categoriaDeProduto);
        }

        [Route("excluir/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_cadastroDeCategoriaDeProduto.EstaConfigurado())
                return Problem(CadastroDeCategoriaDeProdutoService.MensagemAcessoNaoConfigurado);

            var categoriaDeProduto = await _cadastroDeCategoriaDeProduto.ObterOuDefaultAsync(id);

            if (categoriaDeProduto == null) 
                return NotFound();

            return View(categoriaDeProduto);
        }

        [HttpPost("excluir/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!_cadastroDeCategoriaDeProduto.EstaConfigurado())
                return Problem(CadastroDeCategoriaDeProdutoService.MensagemAcessoNaoConfigurado);

            var categoriaDeProduto = await _cadastroDeCategoriaDeProduto.ObterAsync(id);

            if (categoriaDeProduto == null)
                return NotFound();

            if (await _cadastroDeCategoriaDeProduto.ContemAssociacoes(id)) 
            { 
                TempData["Falha"] = CadastroDeCategoriaDeProdutoService.MensagemExclusaoProibidaPorAssociacaoAProduto;
                return View(categoriaDeProduto);
            }
            
            await _cadastroDeCategoriaDeProduto.Excluir(id);

            TempData["Sucesso"] = CadastroDeCategoriaDeProdutoService.MensagemExclusaoBemSucedida;

            return RedirectToAction(nameof(Index));
        }
    }
}
