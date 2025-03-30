using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoMiniLoja.Data.Data;
using GestaoMiniLoja.Data.Models;

namespace GestaoMiniLoja.Web.Controllers
{
    [Route("categorias-de-produto")]
    public class CategoriasDeProdutoController(ApplicationDbContext context) : Controller
    {
        private const string AccessNotConfiguredMessage = "Não configurado o acesso às categorias de produto.";
        private readonly ApplicationDbContext _context = context;

        public async Task<IActionResult> Index()
        {
            if (_context.CategoriasDeProduto == null)
            {
                return Problem(AccessNotConfiguredMessage);
            }

            return View(await _context.CategoriasDeProduto.ToListAsync());
        }

        [Route("detalhes/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            if (_context.CategoriasDeProduto == null)
            {
                return Problem(AccessNotConfiguredMessage);
            }

            var categoriaDeProduto = await _context.CategoriasDeProduto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoriaDeProduto == null)
            {
                return NotFound();
            }

            return View(categoriaDeProduto);
        }

        [Route("nova")]
        public IActionResult Create()
        {
            return View();
        }

   
        [HttpPost("nova")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descricao")] CategoriaDeProduto categoriaDeProduto)
        {
            if (_context.CategoriasDeProduto == null)
            {
                return Problem(AccessNotConfiguredMessage);
            }

            if (ModelState.IsValid)
            {
                _context.Add(categoriaDeProduto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaDeProduto);
        }

        [Route("editar/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (_context.CategoriasDeProduto == null)
            {
                return Problem(AccessNotConfiguredMessage);
            }

            var categoriaDeProduto = await _context.CategoriasDeProduto.FindAsync(id);
            if (categoriaDeProduto == null)
            {
                return NotFound();
            }
            return View(categoriaDeProduto);
        }

        [HttpPost("editar/{id:int}"), ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descricao")] CategoriaDeProduto categoriaDeProduto)
        {
            if (_context.CategoriasDeProduto == null)
            {
                return Problem(AccessNotConfiguredMessage);
            }

            if (id != categoriaDeProduto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriaDeProduto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaDeProdutoExists(categoriaDeProduto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaDeProduto);
        }

        [Route("excluir/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.CategoriasDeProduto == null)
            {
                return Problem(AccessNotConfiguredMessage);
            }

            var categoriaDeProduto = await _context.CategoriasDeProduto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoriaDeProduto == null)
            {
                return NotFound();
            }

            return View(categoriaDeProduto);
        }

        // POST: CategoriaDeProdutos/Delete/5
        [HttpPost("excluir/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CategoriasDeProduto == null)
            {
                return Problem(AccessNotConfiguredMessage);
            }

            var categoriaDeProduto = await _context.CategoriasDeProduto.FindAsync(id);
            if (categoriaDeProduto != null)
            {
                _context.CategoriasDeProduto.Remove(categoriaDeProduto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaDeProdutoExists(int id)
        {
            return _context.CategoriasDeProduto.Any(e => e.Id == id);
        }
    }
}
