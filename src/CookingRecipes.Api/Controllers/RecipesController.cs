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

        // Add a new recipe
        [HttpPost("Add")]
        [Authorize]
        public async Task<ActionResult> CreateRecipe([FromBody] RecipeDto recipe)
        {
            try
            {
                var isCreated = await _recipeService.CreateRecipeAsync(recipe).ConfigureAwait(false);

                return isCreated ? Ok(isCreated) : BadRequest("The recipe could not be created");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing CreateRecipe: {ex}", ex);
                throw;
            }
        }

        // Get all recipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipes()
        {
            try
            {
                var recipes = await _recipeService.GetAllRecipesAsync().ConfigureAwait(false);

                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing GetRecipes: {ex}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        // Get recipe by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipeById(int id)
        {
            try
            {
                var recipe = await _recipeService.GetRecipeByIdAsync(id).ConfigureAwait(false);

                if (recipe == null) return NotFound();

                return Ok(recipe);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing GetRecipeById: {ex}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        // Update an existing recipe
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateRecipe(int id, [FromBody] RecipeDto recipe)
        {
            try
            {
                if (id != recipe.RecipeID) return BadRequest("Recipe ID mismatch");

                await _recipeService.UpdateRecipeAsync(id, recipe).ConfigureAwait(false);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing UpdateRecipe: {ex}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        // Delete a recipe
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteRecipe(int id)
        {
            try
            {
                var isDeleted = await _recipeService.DeleteRecipeAsync(id).ConfigureAwait(false);

                return isDeleted ? NoContent() : BadRequest("Recipe could not be deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing DeleteRecipe: {ex}", ex);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
