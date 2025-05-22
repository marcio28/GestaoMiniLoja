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
    [Route("api/categorias")]
    public class CategoriasController(AppDbContext context) : ControllerBase
    {
        readonly CategoriasService _categoriasService = new(context);

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            if (!_categoriasService.EstaConfigurado()) return Problem("Cadastro de categorias inacessível. Contate o suporte!");

            return await _categoriasService.ObterTodosAsync();
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            if (!_categoriasService.EstaConfigurado()) return Problem("Cadastro de categorias inacessível. Contate o suporte!");

            var categoria = await _categoriasService.ObterAsync(id);
            if (categoria == null) return NotFound();

            return categoria;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Categoria), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Categoria>> PostCategoria(Categoria categoria)
        {
            if (!_categoriasService.EstaConfigurado()) return Problem("Cadastro de categorias inacessível. Contate o suporte!");

            if (!ModelState.IsValid) return ValidationProblem(new ValidationProblemDetails(ModelState)
            {
                Title = "Um ou mais erros de validação ocorreram!"
            });

            try
            {
                await _categoriasService.IncluirAsync(categoria);
                return CreatedAtAction(nameof(PostCategoria), new { id = categoria.Id }, categoria);

            }
            catch (RegraDeNegocioException e)
            {
                return ValidationProblem(new ValidationProblemDetails()
                {
                    Title = $"Regra de negócio violada na inclusão de categoria: {e.Message}"
                });
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PutCategoria(int id, Categoria categoria)
        {
            if (!_categoriasService.EstaConfigurado()) return Problem("Cadastro de categorias inacessível. Contate o suporte!");

            if (id != categoria.Id) return BadRequest();

            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                await _categoriasService.AtualizarAsync(categoria);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _categoriasService.ExisteAsync(id)) return NotFound();
                throw;
            }
            catch (RegraDeNegocioException e)
            {
                return ValidationProblem(new ValidationProblemDetails()
                {
                    Title = $"Regra de negócio violada na atualização de categoria: {e.Message}"
                });
            }
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            if (!_categoriasService.EstaConfigurado()) return Problem("Cadastro de categorias inacessível. Contate o suporte!");

            var categoriaExistente = await _categoriasService.ObterAsync(id);
            if (categoriaExistente == null) return NotFound();

            try
            {
                await _categoriasService.ExcluirAsync(id);
            }
            catch (RegraDeNegocioException e)
            {
                return ValidationProblem(new ValidationProblemDetails()
                {
                    Title = $"Regra de negócio violada na exclusão de categoria: {e.Message}"
                });
            }
            return NoContent();
        }
    }
}
