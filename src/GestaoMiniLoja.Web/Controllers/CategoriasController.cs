using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoMiniLoja.Core;
using GestaoMiniLoja.Core.Models;
using GestaoMiniLoja.Core.Services;
using Microsoft.AspNetCore.Authorization;
using GestaoMiniLoja.Core.Exceptions;

namespace GestaoMiniLoja.Web.Controllers
{
    [Authorize]
    [Route("categorias")]
    public class CategoriasController(AppDbContext context) : Controller
    {
        private readonly CategoriasService _categoriasService = new(context);

        public async Task<IActionResult> Index()
        {
            try
            {
                var categorias = await _categoriasService.ObterTodosAsync();
                return View(categorias);
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
                var categoria = await _categoriasService.ObterOuDefaultAsync(id);
                if (categoria == null) return NotFound();

                return View(categoria);
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
            }
            return View();
        }

        [Route("nova")]
        public IActionResult Create() => View();
   
        [HttpPost("nova")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descricao")] Categoria categoria)
        {
            if (!ModelState.IsValid) return View(categoria);

            try
            {
                await _categoriasService.IncluirAsync(categoria);

                TempData["Sucesso"] = "Categoria incluída.";
                return RedirectToAction("Index");
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
            }
            return View(categoria);
        }

        [Route("editar/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var categoria = await _categoriasService.ObterAsync(id);
                return View(categoria);
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
            }
            return View();
        }

        [HttpPost("editar/{id:int}"), ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descricao")] Categoria categoria)
        {
            if (!ModelState.IsValid) return View(categoria);

            if (id != categoria.Id) return NotFound();

            try
            {
                await _categoriasService.AtualizarAsync(categoria);

                TempData["Sucesso"] = "Categoria atualizada.";
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _categoriasService.ExisteAsync(categoria.Id)) return NotFound();

                TempData["Falha"] = "Categoria atualizada por outro usuário. Verifique.";
                return RedirectToAction("Index");
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
            }
            return View(categoria);
        }

        [Route("excluir/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var categoria = await _categoriasService.ObterOuDefaultAsync(id);
                if (categoria == null) return NotFound();

                return View(categoria);
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
                var categoria = await _categoriasService.ObterAsync(id);
                if (categoria == null) return NotFound();

                await _categoriasService.ExcluirAsync(id);
                TempData["Sucesso"] = "Categoria excluída.";
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
