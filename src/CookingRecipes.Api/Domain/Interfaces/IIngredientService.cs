using CookingRecipes.Api.Domain.DTOs;
using CookingRecipes.Api.Domain.Models.Responses;

namespace CookingRecipes.Api.Domain.Interfaces
{
    public interface IIngredientService
    {
        Task<ApiResponse<string?>> CreateIngredientAsync(IngredientDto ingredient);
        Task<ApiResponse<IEnumerable<IngredientDto>?>> GetAllIngredientsAsync();
        Task<ApiResponse<IngredientDto?>> GetIngredientByIdAsync(int id);
        Task<ApiResponse<string?>> UpdateIngredientAsync(int id, IngredientDto ingredientUpdated);
        Task<ApiResponse<string?>> DeleteIngredientAsync(int id);
    }
}
