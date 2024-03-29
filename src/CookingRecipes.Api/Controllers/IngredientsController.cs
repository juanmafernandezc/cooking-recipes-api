using CommunityToolkit.Diagnostics;
using CookingRecipes.Api.Domain.DTOs;
using CookingRecipes.Api.Domain.Entities;
using CookingRecipes.Api.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CookingRecipes.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly ILogger<IngredientsController> _logger;
        private readonly IIngredientService _ingredientService;

        public IngredientsController(ILogger<IngredientsController> logger, IIngredientService ingredientService)
        {
            Guard.IsNotNull(logger);
            Guard.IsNotNull(ingredientService);

            _logger = logger;
            _ingredientService = ingredientService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddIngredient(IngredientDto ingredient)
        {
            try
            {
                var result = await _ingredientService.CreateIngredientAsync(ingredient).ConfigureAwait(false);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing AddIngredient: {ex}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetAllIngredients()
        {
            try
            {
                var result = await _ingredientService.GetAllIngredientsAsync().ConfigureAwait(false);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing GetAllIngredients: {ex}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IngredientDto>> GetIngredientById(int id)
        {
            try
            {
                var result = await _ingredientService.GetIngredientByIdAsync(id).ConfigureAwait(false);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing GetIngredientById: {ex}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateIngredient(int id, [FromBody] IngredientDto ingredientDto)
        {
            try
            {
                var result = await _ingredientService.UpdateIngredientAsync(id, ingredientDto).ConfigureAwait(false);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing UpdateIngredient: {ex}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteIngredient(int id)
        {
            try
            {
                var result = await _ingredientService.DeleteIngredientAsync(id).ConfigureAwait(false);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing DeleteIngredient: {ex}", ex);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
