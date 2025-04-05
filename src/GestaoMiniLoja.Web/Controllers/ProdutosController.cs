using GestaoMiniLoja.Data.Data;
using GestaoMiniLoja.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestaoMiniLoja.Web.Controllers
{
    [Route("produtos")]
    public class ProdutosController(ApplicationDbContext context, UserManager<IdentityUser> userManager) : Controller
    {
        const string MensagemAcessoNaoConfigurado = "Não configurado o acesso aos dados de produtos.";
        const string MensagemUsuarioNaoIdentificado = "Usuário não identificado.";
        readonly ApplicationDbContext _context = context;
        readonly UserManager<IdentityUser> _userManager = userManager;
        string? _usuarioIdString;

        public async Task<IActionResult> Index()
        {
            _usuarioIdString = await ObterUsuarioIdAsync();

            if (_usuarioIdString == null) 
                return Problem(MensagemUsuarioNaoIdentificado);

            var applicationDbContext = _context.Produtos.Include(p => p.CategoriaDeProduto)
                                                        .Include(p => p.Vendedor)
                                                        .Where(p => p.VendedorId.ToString() == _usuarioIdString);

            return View(await applicationDbContext.ToListAsync());
        }

        [Route("detalhes/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            if (_context.Produtos == null) 
                return Problem(MensagemAcessoNaoConfigurado);

            _usuarioIdString = await ObterUsuarioIdAsync();

            if (_usuarioIdString == null) 
                return Problem(MensagemUsuarioNaoIdentificado);

            var produto = await _context.Produtos.Include(p => p.CategoriaDeProduto)
                                                 .Include(p => p.Vendedor)
                                                 .FirstOrDefaultAsync(m => m.Id == id);
            
            if (produto == null) 
                return NotFound();

            if (produto.VendedorId.ToString() != _usuarioIdString) return BadRequest();

            return View(produto);
        }

        [Route("novo")]
        public IActionResult Create()
        {
            ViewData["CategoriaDeProdutoId"] = new SelectList(_context.CategoriasDeProduto, "Id", "Descricao");

            return View();
        }

        [HttpPost("novo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,CaminhoDaImagem,Preco,QuantidadeEmEstoque,CategoriaDeProdutoId")] Produto produto)
        {
            if (_context.Produtos == null) 
                return Problem(MensagemAcessoNaoConfigurado);

            _usuarioIdString = await ObterUsuarioIdAsync();

            if (_usuarioIdString == null) 
                return Problem(MensagemUsuarioNaoIdentificado);

            _ = ModelState.Remove("Vendedor");
            _ = ModelState.Remove("VendedorId");
            _ = ModelState.Remove("CategoriaDeProduto");

            if (ModelState.IsValid)
            {

                produto.VendedorId = new Guid(_usuarioIdString);
                CriarVendedorSeNaoExiste(produto.VendedorId);

                _context.Add(produto);

                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Produto incluído com sucesso.";

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaDeProdutoId"] = new SelectList(_context.CategoriasDeProduto, "Id", "Descricao", produto.CategoriaDeProdutoId);

            return View(produto);
        }

        [Route("editar/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (_context.Produtos == null) 
                return Problem(MensagemAcessoNaoConfigurado);

            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
                return NotFound();

            ViewData["CategoriaDeProdutoId"] = new SelectList(_context.CategoriasDeProduto, "Id", "Descricao", produto.CategoriaDeProdutoId);

            return View(produto);
        }

        [HttpPost("editar/{id:int}"), ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,CaminhoDaImagem,Preco,QuantidadeEmEstoque,CategoriaDeProdutoId,VendedorId")] Produto produto)
        {
            if (_context.Produtos == null)
                return Problem(MensagemAcessoNaoConfigurado);

            if (id != produto.Id)
                return BadRequest();

            _ = ModelState.Remove("Vendedor");
            _ = ModelState.Remove("CategoriaDeProduto");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.Id))
                        return NotFound();
                    else
                        throw;
                }

                TempData["Sucesso"] = "Produto editado com sucesso.";

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaDeProdutoId"] = new SelectList(_context.CategoriasDeProduto, "Id", "Descricao", produto.CategoriaDeProdutoId);

            return View(produto);
        }

        [Route("excluir/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Produtos == null)
                return Problem(MensagemAcessoNaoConfigurado);

            var produto = await _context.Produtos.Include(p => p.CategoriaDeProduto)
                                                 .Include(p => p.Vendedor)
                                                 .FirstOrDefaultAsync(m => m.Id == id);

            if (produto == null)
                return NotFound();

            return View(produto);
        }

        [HttpPost("excluir/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Produtos == null)
                return Problem(MensagemAcessoNaoConfigurado);

            var produto = await _context.Produtos.FindAsync(id);

            if (produto != null)
                _context.Produtos.Remove(produto);

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Produto excluído com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }

        private async Task<string?> ObterUsuarioIdAsync()
        {
            IdentityUser? user = await _userManager.GetUserAsync(HttpContext.User);
            return user?.Id.ToString();
        }

        // TODO: Extrair método, para ser usado pela API também
        private void CriarVendedorSeNaoExiste(Guid id)
        {
            if (!_context.Vendedores.Any(v => v.Id == id))
            {
                Vendedor vendedor = new() { Id = id };
                _context.Vendedores.Add(vendedor);
            }
        }
    }
}
