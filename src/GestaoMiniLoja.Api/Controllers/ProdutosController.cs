using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GestaoMiniLoja.Core;
using GestaoMiniLoja.Core.Exceptions;
using GestaoMiniLoja.Core.Models;
using GestaoMiniLoja.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoMiniLoja.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosController(AppDbContext context, IHttpContextAccessor accessor) : ControllerBase
    {
        readonly CategoriasService _categoriasService = new(context);
        readonly ProdutosService _produtosService = new(context);
        readonly IHttpContextAccessor _accessor = accessor;

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            if (!_produtosService.EstaConfigurado()) return Problem("Erro ao obter produtos. Contate o suporte!");

            return await _produtosService.ObterTodosAsync();
        }

        [AllowAnonymous]
        [HttpGet("por-categoria/{categoriaId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutosByCategoria(int categoriaId)
        {
            if (!_produtosService.EstaConfigurado()) return Problem("Erro ao obter produtos por categoria. Contate o suporte!");

            return await _produtosService.ObterPorCategoriaAsync(categoriaId); ;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            if (!_produtosService.EstaConfigurado()) return Problem("Erro ao obter um produto. Contate o suporte!");

            var produto = await _produtosService.ObterAsync(id);
            if (produto == null) return NotFound();

            if (!ProdutoEhDoVendedor(produto)) return Forbid();

            return produto;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            if (!_categoriasService.EstaConfigurado()) return Problem("Cadastro de categorias inacessível. Contate o suporte!");

            if (!_produtosService.EstaConfigurado()) return Problem("Cadastro de produtos inacessível. Contate o suporte!");

            if (!ModelState.IsValid) return ValidationProblem(new ValidationProblemDetails(ModelState)
            {
                Title = "Um ou mais erros de validação ocorreram!"
            });

            if (!await _categoriasService.ExisteAsync(produto.CategoriaId)) return NotFound($"Categoria não encontrada.");

            try
            {
                await _produtosService.IncluirAsync(produto);
                return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);

            }
            catch (RegraDeNegocioException e)
            {
                return ValidationProblem(new ValidationProblemDetails()
                {
                    Title = $"Regra de negócio violada na inclusão de produto: {e.Message}"
                });
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PutProduto(int id, Produto produto)
        {
            if (!_categoriasService.EstaConfigurado()) return Problem("Cadastro de categorias inacessível. Contate o suporte!");

            if (!_produtosService.EstaConfigurado()) return Problem("Cadastro de produtos inacessível. Contate o suporte!");

            if (id != produto.Id) return BadRequest();

            if (!ProdutoEhDoVendedor(produto)) return Forbid();

            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            if (!await _categoriasService.ExisteAsync(produto.CategoriaId)) return NotFound($"Categoria não encontrada.");

            try
            {
                await _produtosService.AtualizarAsync(produto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _produtosService.ExisteAsync(id)) return NotFound();
                throw;
            }
            catch (RegraDeNegocioException e)
            {
                return ValidationProblem(new ValidationProblemDetails()
                {
                    Title = $"Regra de negócio violada na atualização de produto: {e.Message}"
                });
            }
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            if (!_produtosService.EstaConfigurado()) return Problem("Erro ao excluir um produto. Contate o suporte!");

            var produtoExistente = await _produtosService.ObterAsync(id);
            if (produtoExistente == null) return NotFound();

            if (!ProdutoEhDoVendedor(produtoExistente)) return Forbid();

            await _produtosService.ExcluirAsync(id);
            return NoContent();
        }

        Guid GetUserId()
        {
            // TODO: Pesquisar por que está sempre retornando null
            var claim = _accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (claim == null)
            {
                claim = _accessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            }

            var userId = claim is null ? Guid.Empty : Guid.Parse(claim);

            userId = Guid.Parse("13be6992-66bc-46b2-a682-c5abca6a4d02"); // TODO: Remover quando funcionar o código acima, que está sempre retornando Guid.Empty
            
            return userId;
        }

        bool ProdutoEhDoVendedor(Produto produto)
        {
            bool ehDoVendedor;
            ehDoVendedor = produto.VendedorId == GetUserId();
            ehDoVendedor = true; // TODO: Remover quando funcionar o método GetUserId, que está sempre retornando Guid.Empty
            return ehDoVendedor;
        }
    }
}
