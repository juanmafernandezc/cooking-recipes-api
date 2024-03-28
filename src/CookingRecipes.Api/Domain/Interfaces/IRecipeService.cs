using CookingRecipes.Api.Domain.DTOs;
using CookingRecipes.Api.Domain.Entities;

namespace CookingRecipes.Api.Domain.Interfaces
{
    public interface IRecipeService
    {
        Task<bool> CreateRecipeAsync(RecipeDto recipe);
        Task<IEnumerable<RecipeDto>?> GetAllRecipesAsync();
        Task<RecipeDto?> GetRecipeByIdAsync(int id);
        Task<bool> UpdateRecipeAsync(int id, RecipeDto recipe);
        Task<bool> DeleteRecipeAsync(int id);
    }
}
