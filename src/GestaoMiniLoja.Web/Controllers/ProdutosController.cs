using GestaoMiniLoja.Data.Data;
using GestaoMiniLoja.Data.Models;
using GestaoMiniLoja.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestaoMiniLoja.Web.Controllers
{
    [Authorize]
    [Route("produtos")]
    public class ProdutosController(ApplicationDbContext context, UserManager<IdentityUser> userManager) : Controller
    {
        const string MensagemUsuarioNaoIdentificado = "Usuário não identificado.";
        private readonly CadastroDeCategoriaDeProdutoService _cadastroDeCategoriaProduto = new(context);
        private readonly CadastroDeProdutoService _cadastroDeProduto = new(context);
        private readonly CadastroDeVendedorService _cadastroDeVendedor = new(context);
        readonly UserManager<IdentityUser> _userManager = userManager;
        string? _usuarioIdString;

        public async Task<IActionResult> Index()
        {
            try
            {
                _usuarioIdString = await ObterUsuarioIdAsync();

                if (_usuarioIdString == null)
                {
                    TempData["Falha"] = MensagemUsuarioNaoIdentificado;
                    return View();
                }

                return View(await _cadastroDeProduto.ObterPorVendedorAsync(_usuarioIdString));
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
                _usuarioIdString = await ObterUsuarioIdAsync();

                if (_usuarioIdString == null)
                {
                    TempData["Falha"] = MensagemUsuarioNaoIdentificado;
                    return RedirectToAction(nameof(Index));
                }

                var produto = await _cadastroDeProduto.ObterOuDefaultAsync(id);

                if (produto == null)
                { 
                    TempData["Falha"] = CadastroDeProdutoService.MensagemEntidadeNaoEncontrada;
                    return RedirectToAction(nameof(Index));
                }

                if (produto.VendedorId.ToString() != _usuarioIdString)
                {
                    TempData["Falha"] = CadastroDeProdutoService.MensagemAcessoNaoPermitido;
                    return RedirectToAction(nameof(Index));
                }
                return View(produto);
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
                return View();
            }
        }

        [Route("novo")]
        public IActionResult Create()
        {
            ViewData["CategoriaDeProdutoId"] = new SelectList(_cadastroDeCategoriaProduto.ObterTodosAsync().Result, "Id", "Descricao");
            return View();
        }

        [HttpPost("novo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,CaminhoDaImagem,Preco,QuantidadeEmEstoque,CategoriaDeProdutoId")] Produto produto)
        {
            try
            {
                _usuarioIdString = await ObterUsuarioIdAsync();

                if (_usuarioIdString == null)
                {
                    TempData["Falha"] = MensagemUsuarioNaoIdentificado;
                    return RedirectToAction(nameof(Index));
                }

                ModelState.Remove("Vendedor");
                ModelState.Remove("VendedorId");
                ModelState.Remove("CategoriaDeProduto");

                if (!ModelState.IsValid)
                {
                    ViewData["CategoriaDeProdutoId"] = new SelectList(_cadastroDeCategoriaProduto.ObterTodosAsync().Result, "Id", "Descricao", produto.CategoriaDeProdutoId);
                    return View(produto);
                }

                produto.VendedorId = new Guid(_usuarioIdString);
                await _cadastroDeVendedor.IncluirSeNaoExiste(produto.VendedorId);
                await _cadastroDeProduto.IncluirAsync(produto);
                TempData["Sucesso"] = CadastroDeProdutoService.MensagemInclusaoBemSucedida;
                return RedirectToAction(nameof(Index));
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
                return View(produto);
            }
        }

        [Route("editar/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var produto = await _cadastroDeProduto.ObterAsync(id);

                if (produto == null)
                {
                    TempData["Falha"] = CadastroDeProdutoService.MensagemEntidadeNaoEncontrada;
                    return RedirectToAction(nameof(Index));
                }

                ViewData["CategoriaDeProdutoId"] = new SelectList(_cadastroDeCategoriaProduto.ObterTodosAsync().Result, "Id", "Descricao", produto.CategoriaDeProdutoId);
                return View(produto);
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
                return View();
            }
        }

        [HttpPost("editar/{id:int}"), ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,CaminhoDaImagem,Preco,QuantidadeEmEstoque,CategoriaDeProdutoId,VendedorId")] Produto produto)
        {
            ModelState.Remove("Vendedor");
            ModelState.Remove("CategoriaDeProduto");

            if (ModelState.IsValid)
            {
                if (id != produto.Id)
                {
                    TempData["Falha"] = "Solicitação inapropriada.";
                    return RedirectToAction(nameof(Index));
                }

                try
                {
                    await _cadastroDeProduto.AtualizarAsync(produto);
                    TempData["Sucesso"] = CadastroDeProdutoService.MensagemAtualizacaoBemSucedida;
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _cadastroDeProduto.ExisteAsync(produto.Id))
                    {
                        TempData["Falha"] = CadastroDeProdutoService.MensagemEntidadeNaoEncontrada;
                    }
                    TempData["Falha"] = CadastroDeProdutoService.MensagemAtualizacaoMalSucedidaPorConcorrencia;
                }
                catch (RegraDeNegocioException rne)
                {
                    TempData["Falha"] = rne.Message;
                }
            }
            ViewData["CategoriaDeProdutoId"] = new SelectList(_cadastroDeCategoriaProduto.ObterTodosAsync().Result, "Id", "Descricao", produto.CategoriaDeProdutoId);
            return View(produto);
        }

        [Route("excluir/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var produto = await _cadastroDeProduto.ObterOuDefaultAsync(id);

                if (produto == null)
                {
                    TempData["Falha"] = CadastroDeProdutoService.MensagemEntidadeNaoEncontrada;
                    return RedirectToAction(nameof(Index));
                }

                return View(produto);
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
                var produto = await _cadastroDeProduto.ObterAsync(id);

                if (produto == null)
                {
                    TempData["Falha"] = CadastroDeProdutoService.MensagemEntidadeNaoEncontrada;
                }

                await _cadastroDeProduto.ExcluirAsync(id);
                TempData["Sucesso"] = CadastroDeProdutoService.MensagemExclusaoBemSucedida;
            }
            catch (RegraDeNegocioException rne)
            {
                TempData["Falha"] = rne.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> ObterUsuarioIdAsync()
        {
            IdentityUser? user = await _userManager.GetUserAsync(HttpContext.User);
            return user?.Id.ToString();
        }
    }
}
