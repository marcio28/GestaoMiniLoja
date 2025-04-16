using GestaoMiniLoja.Data;
using GestaoMiniLoja.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoMiniLoja.Api.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosController(ApplicationDbContext dbContext) : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            return await _dbContext.Produtos.ToListAsync();
        }
    }
}
