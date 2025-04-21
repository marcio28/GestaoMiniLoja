using GestaoMiniLoja.Core;
using GestaoMiniLoja.Core.Exceptions;
using GestaoMiniLoja.Core.Models;
using GestaoMiniLoja.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestaoMiniLoja.Web.Controllers
{
    [Authorize]
    [Route("meus-produtos")]
    public class MeusProdutosController(AppDbContext context, UserManager<IdentityUser> userManager) : Controller
    {
        private readonly CategoriasService _categoriasService = new(context);
        private readonly ProdutosService _produtosService = new(context);
        private readonly VendedoresService _vendedoresService = new(context);
        readonly UserManager<IdentityUser> _userManager = userManager;
        string? _usuarioIdString;

        public async Task<IActionResult> Index()
        {
            try
            {
                _usuarioIdString = await ObterUsuarioIdAsync();
                if (_usuarioIdString == null) return NotFound();

                return View(await _produtosService.ObterPorVendedorAsync(_usuarioIdString));
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
            }
            return View();

        }

        [Route("detalhes/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                _usuarioIdString = await ObterUsuarioIdAsync();
                if (_usuarioIdString == null) return NotFound();

                var produto = await _produtosService.ObterOuDefaultAsync(id);
                if (produto == null) return NotFound();

                if (produto.VendedorId.ToString() != _usuarioIdString) return BadRequest();

                return View(produto);
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
            }
            return View();
        }

        [Route("novo")]
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_categoriasService.ObterTodosAsync().Result, "Id", "Descricao");
            return View();
        }

        [HttpPost("novo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,CaminhoDaImagem,Preco,Estoque,CategoriaId")] Produto produto)
        {
            try
            {
                _usuarioIdString = await ObterUsuarioIdAsync();
                if (_usuarioIdString == null) return NotFound();

                ModelState.Remove("Vendedor");
                ModelState.Remove("VendedorId");
                ModelState.Remove("Categoria");

                if (!ModelState.IsValid)
                {
                    ViewData["CategoriaId"] = new SelectList(_categoriasService.ObterTodosAsync().Result, "Id", "Descricao", produto.CategoriaId);
                    return View(produto);
                }

                await _vendedoresService.IncluirSeNaoExisteAsync(new Guid(_usuarioIdString));
                await _produtosService.IncluirAsync(produto);
                TempData["Sucesso"] = "Produto incluído.";
                return RedirectToAction("Index");
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
            }
            return View(produto);
        }

        [Route("editar/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var produto = await _produtosService.ObterAsync(id);
                if (produto == null) return NotFound();

                ViewData["CategoriaId"] = new SelectList(_categoriasService.ObterTodosAsync().Result, "Id", "Descricao", produto.CategoriaId);
                return View(produto);
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
            }
            return View();
        }

        [HttpPost("editar/{id:int}"), ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,CaminhoDaImagem,Preco,Estoque,CategoriaId,VendedorId")] Produto produto)
        {
            ModelState.Remove("Vendedor");
            ModelState.Remove("Categoria");

            if (ModelState.IsValid)
            {
                if (id != produto.Id) return BadRequest();

                try
                {
                    await _produtosService.AtualizarAsync(produto);
                    TempData["Sucesso"] = "Produto atualizado.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _produtosService.ExisteAsync(produto.Id)) return NotFound();
                    TempData["Falha"] = "Produto atualizado por outro usuário. Verifique.";
                }
                catch (RegraDeNegocioException rne)
                {
                    TempData["Falha"] = rne.Message;
                }
            }
            ViewData["CategoriaId"] = new SelectList(_categoriasService.ObterTodosAsync().Result, "Id", "Descricao", produto.CategoriaId);
            return View(produto);
        }

        [Route("excluir/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var produto = await _produtosService.ObterOuDefaultAsync(id);
                if (produto == null) return NotFound();

                return View(produto);
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost("excluir/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var produto = await _produtosService.ObterAsync(id);
                if (produto == null) return NotFound();

                await _produtosService.ExcluirAsync(id);
                TempData["Sucesso"] = "Produto excluído.";
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
            }
            return RedirectToAction("Index");
        }

        private async Task<string?> ObterUsuarioIdAsync()
        {
            IdentityUser? user = await _userManager.GetUserAsync(HttpContext.User);
            return user?.Id.ToString();
        }
    }
}
