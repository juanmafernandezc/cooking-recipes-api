using CookingRecipes.Api.Domain.Entities;

namespace CookingRecipes.Api.Domain.Interfaces
{
    public interface IIngredientService
    {
        Task<IEnumerable<Ingredient>> GetAllIngredientsAsync();
        Task<bool> CreateIngredientAsync(Ingredient ingredient);
    }
}
