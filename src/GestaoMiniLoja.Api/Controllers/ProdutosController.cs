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
    public class ProdutosController(AppDbContext context) : ControllerBase
    {
        readonly CategoriasService _categoriasService = new(context);
        readonly ProdutosService _produtosService = new(context);

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            if (!_produtosService.EstaConfigurado()) return Problem("Cadastro de produtos inacessível. Contate o suporte!");

            return await _produtosService.ObterTodosAsync();
        }

        [AllowAnonymous]
        [HttpGet("por-categoria/{categoriaId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutosByCategoria(int categoriaId)
        {
            if (!_produtosService.EstaConfigurado()) return Problem("Cadastro de produtos inacessível. Contate o suporte!");

            return await _produtosService.ObterPorCategoriaAsync(categoriaId); ;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            if (!_produtosService.EstaConfigurado()) return Problem("Cadastro de produtos inacessível. Contate o suporte!");

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

            Produto produtoAIncluir = new()
            {
                Id = 0,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Estoque = produto.Estoque,
                Preco = produto.Preco,
                CategoriaId = produto.CategoriaId,
                VendedorId = GetUserId(),
                CaminhoDaImagem = produto.CaminhoDaImagem
            };

            try
            {
                await _produtosService.IncluirAsync(produtoAIncluir);
                return CreatedAtAction(nameof(GetProduto), new { id = produtoAIncluir.Id }, produtoAIncluir);

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
            if (!_produtosService.EstaConfigurado()) return Problem("Cadastro de produtos inacessível. Contate o suporte!");

            var produtoExistente = await _produtosService.ObterAsync(id);
            if (produtoExistente == null) return NotFound();

            if (!ProdutoEhDoVendedor(produtoExistente)) return Forbid();

            await _produtosService.ExcluirAsync(id);
            return NoContent();
        }

        Guid GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(ClaimTypes.Sid);
            return claim is null ? Guid.Empty : Guid.Parse(claim.Value);
        }

        bool ProdutoEhDoVendedor(Produto produto) => produto.VendedorId == GetUserId();
    }
}
