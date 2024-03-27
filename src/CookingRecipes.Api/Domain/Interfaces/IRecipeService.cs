using CookingRecipes.Api.Domain.Entities;

namespace CookingRecipes.Api.Domain.Interfaces
{
    public interface IRecipeService
    {
        Task<Recipe> CreateRecipeAsync(Recipe recipe);
        Task<IEnumerable<Recipe>> GetAllRecipesAsync();
        Task<Recipe?> GetRecipeByIdAsync(int id);
        Task UpdateRecipeAsync(int id, Recipe recipe);
        Task DeleteRecipeAsync(int id);
    }
}
