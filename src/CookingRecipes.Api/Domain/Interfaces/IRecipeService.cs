using CookingRecipes.Api.Domain.DTOs;
using CookingRecipes.Api.Domain.Models.Responses;

namespace CookingRecipes.Api.Domain.Interfaces
{
    public interface IRecipeService
    {
        Task<ApiResponse<string?>> CreateRecipeAsync(RecipeDto recipe);
        Task<ApiResponse<IEnumerable<RecipeDto>?>> GetAllRecipesAsync();
        Task<ApiResponse<RecipeDto?>> GetRecipeByIdAsync(int id);
        Task<ApiResponse<string?>> UpdateRecipeAsync(int id, RecipeDto recipe);
        Task<ApiResponse<string?>> DeleteRecipeAsync(int id);
    }
}
