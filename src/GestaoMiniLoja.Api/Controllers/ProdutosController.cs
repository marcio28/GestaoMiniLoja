using GestaoMiniLoja.Core;
using GestaoMiniLoja.Core.Exceptions;
using GestaoMiniLoja.Core.Models;
using GestaoMiniLoja.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoMiniLoja.Api.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosController(AppDbContext context) : ControllerBase
    {
        private readonly ProdutosService _produtosService = new(context);

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            if (!_produtosService.EstaConfigurado()) return Problem("Erro ao obter produtos. Contate o suporte!");

            return await _produtosService.ObterTodosAsync();
        }

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

            return produto;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            if (!_produtosService.EstaConfigurado()) return Problem("Erro ao criar um produto. Contate o suporte!");

            if (!ModelState.IsValid) return ValidationProblem(new ValidationProblemDetails(ModelState)
            {
                Title = "Um ou mais erros de validação ocorreram!"
            });

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
            if (!_produtosService.EstaConfigurado()) return Problem("Erro ao atualizar um produto. Contate o suporte!");

            if (id != produto.Id) return BadRequest();

            if (!ModelState.IsValid) return ValidationProblem(ModelState);

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

            if (!await _produtosService.ExisteAsync(id)) return NotFound();

            await _produtosService.ExcluirAsync(id);
            return NoContent();
        }
    }
}
