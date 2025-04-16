using GestaoMiniLoja.Core;
using GestaoMiniLoja.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoMiniLoja.Api.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosController(AppDbContext dbContext) : ControllerBase
    {
        private readonly AppDbContext _dbContext = dbContext;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            return await _dbContext.Produtos.ToListAsync();
        }
    }
}
