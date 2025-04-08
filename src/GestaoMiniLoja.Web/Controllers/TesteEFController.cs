using GestaoMiniLoja.Data;
using GestaoMiniLoja.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoMiniLoja.Web.Controllers
{
    public class TesteEFController(ApplicationDbContext db) : Controller
    {
        public ApplicationDbContext Db { get; } = db;

        public IActionResult Index()
        {
            CriarVendedor();
            return View();
        }

        private void CriarVendedor()
        {
            Console.WriteLine("Criando e salvando vendedor...");
            Vendedor vendedor = new() { Id = Guid.NewGuid() };
            Db.Vendedores.Add(vendedor);
            Db.SaveChanges();
            Console.WriteLine("Vendedor salvo.");
        }
    }
}
