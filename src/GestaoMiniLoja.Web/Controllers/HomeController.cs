using System.Diagnostics;
using GestaoMiniLoja.Core;
using GestaoMiniLoja.Core.Services;
using GestaoMiniLoja.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoMiniLoja.Web.Controllers
{
    public class HomeController(ILogger<HomeController> logger, AppDbContext dbContext) : Controller
    {
        private readonly ProdutosService _produtosService = new(dbContext);
        private readonly ILogger<HomeController> _logger = logger;

        public async Task<IActionResult> Index()
        {
            try
            {
                var produtos = await _produtosService.ObterDisponiveisAsync();
                return View(produtos);
            }
            catch (Exception e)
            {
                TempData["Falha"] = "Estamos fechados no momento.";
                _logger.LogError(e, e.Message);
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
