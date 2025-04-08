using System.Diagnostics;
using GestaoMiniLoja.Data;
using GestaoMiniLoja.Data.Services;
using GestaoMiniLoja.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoMiniLoja.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly CadastroDeProdutoService _cadastroDeProduto;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _cadastroDeProduto = new CadastroDeProdutoService(context);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var produtos = await _cadastroDeProduto.ObterDisponiveisEmEstoqueAsync();
                if (produtos.Count == 0)
                    TempData["Falha"] = "Produtos esgotados! Retorne mais tarde.";
                return View(produtos);
            }
            catch (Exception e)
            {
                TempData["Falha"] = "Estamos fechados no momento. Em breve, muitos produtos para você!";
                _logger.LogError(e, e.Message);
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
