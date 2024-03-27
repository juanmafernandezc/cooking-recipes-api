using CommunityToolkit.Diagnostics;
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
        public async Task<ActionResult> AddIngredient(Ingredient ingredient)
        {
            try
            {
                var result = await _ingredientService.CreateIngredientAsync(ingredient).ConfigureAwait(false);

                return result ? Ok(result) : BadRequest("The ingredient could not be created");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing AddIngredient: {ex}", ex);
                throw;
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetAllIngredients()
        {
            try
            {
                var ingredients = await _ingredientService.GetAllIngredientsAsync().ConfigureAwait(false);

                return Ok(ingredients);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing GetAllIngredients: {ex}", ex);
                throw;
            }
        }
    }
}
