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
    public class RecipesController : ControllerBase
    {
        private readonly ILogger<RecipesController> _logger;
        private readonly IRecipeService _recipeService;

        public RecipesController(ILogger<RecipesController> logger, IRecipeService recipeService)
        {
            Guard.IsNotNull(logger);
            Guard.IsNotNull(recipeService);

            _logger = logger;
            _recipeService = recipeService;
        }

        [HttpPost("Add")]
        [Authorize]
        public async Task<IActionResult> CreateRecipe([FromBody] RecipeDto recipe)
        {
            try
            {
                var result = await _recipeService.CreateRecipeAsync(recipe).ConfigureAwait(false);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing CreateRecipe: {ex}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRecipes()
        {
            try
            {
                var result = await _recipeService.GetAllRecipesAsync().ConfigureAwait(false);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing GetRecipes: {ex}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetRecipeById(int id)
        {
            try
            {
                var result = await _recipeService.GetRecipeByIdAsync(id).ConfigureAwait(false);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing GetRecipeById: {ex}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateRecipe(int id, [FromBody] RecipeDto recipe)
        {
            try
            {
                var result = await _recipeService.UpdateRecipeAsync(id, recipe).ConfigureAwait(false);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing UpdateRecipe: {ex}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteRecipe(int id)
        {
            try
            {
                var result = await _recipeService.DeleteRecipeAsync(id).ConfigureAwait(false);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing DeleteRecipe: {ex}", ex);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
