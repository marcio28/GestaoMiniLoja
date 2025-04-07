using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoMiniLoja.Data.Data;
using GestaoMiniLoja.Data.Models;
using GestaoMiniLoja.Data.Services;
using Microsoft.AspNetCore.Authorization;

namespace GestaoMiniLoja.Web.Controllers
{
    [Authorize]
    [Route("categorias-de-produto")]
    public class CategoriasDeProdutoController(ApplicationDbContext context) : Controller
    {
        private readonly CadastroDeCategoriaDeProdutoService _cadastroDeCategoriaDeProduto = new(context);

        public async Task<IActionResult> Index()
        {
            try
            {
                var categoriasDeProduto = await _cadastroDeCategoriaDeProduto.ObterTodosAsync();
                return View(categoriasDeProduto);
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
                return View();
            }
        }

        [Route("detalhes/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var categoriaDeProduto = await _cadastroDeCategoriaDeProduto.ObterOuDefaultAsync(id);
                return View(categoriaDeProduto);
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
                return View();
            }
        }

        [Route("nova")]
        public IActionResult Create() => View();
   
        [HttpPost("nova")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descricao")] CategoriaDeProduto categoriaDeProduto)
        {
            if (!ModelState.IsValid)
                return View(categoriaDeProduto);

            try
            {
                await _cadastroDeCategoriaDeProduto.IncluirAsync(categoriaDeProduto);
                TempData["Sucesso"] = CadastroDeCategoriaDeProdutoService.MensagemInclusaoBemSucedida;
                return RedirectToAction(nameof(Index));
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
                return View(categoriaDeProduto);
            }
        }

        [Route("editar/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var categoriaDeProduto = await _cadastroDeCategoriaDeProduto.ObterAsync(id);
                return View(categoriaDeProduto);
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
                return View();
            }
        }

        [HttpPost("editar/{id:int}"), ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descricao")] CategoriaDeProduto categoriaDeProduto)
        {
            if (!ModelState.IsValid)
                return View(categoriaDeProduto);

            if (id != categoriaDeProduto.Id)
            {
                TempData["Falha"] = "Solicitação inapropriada.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _cadastroDeCategoriaDeProduto.AtualizarAsync(categoriaDeProduto);
                TempData["Sucesso"] = CadastroDeCategoriaDeProdutoService.MensagemAtualizacaoBemSucedida;
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _cadastroDeCategoriaDeProduto.ExisteAsync(categoriaDeProduto.Id))
                {
                    TempData["Falha"] = CadastroDeCategoriaDeProdutoService.MensagemEntidadeNaoEncontrada;
                    return View(categoriaDeProduto);
                }
                TempData["Falha"] = CadastroDeCategoriaDeProdutoService.MensagemAtualizacaoMalSucedidaPorConcorrencia;
                return RedirectToAction(nameof(Index));
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
                return View(categoriaDeProduto);
            }
        }

        [Route("excluir/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var categoriaDeProduto = await _cadastroDeCategoriaDeProduto.ObterOuDefaultAsync(id);

                if (categoriaDeProduto == null)
                {
                    TempData["Falha"] = CadastroDeCategoriaDeProdutoService.MensagemEntidadeNaoEncontrada;
                    return RedirectToAction(nameof(Index));
                }

                return View(categoriaDeProduto);
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("excluir/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var categoriaDeProduto = await _cadastroDeCategoriaDeProduto.ObterAsync(id);

                if (categoriaDeProduto == null)
                {
                    TempData["Falha"] = CadastroDeCategoriaDeProdutoService.MensagemEntidadeNaoEncontrada;
                }

                await _cadastroDeCategoriaDeProduto.ExcluirAsync(id);
                TempData["Sucesso"] = CadastroDeCategoriaDeProdutoService.MensagemExclusaoBemSucedida;
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
