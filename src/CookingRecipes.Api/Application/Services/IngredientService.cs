using CommunityToolkit.Diagnostics;
using CookingRecipes.Api.Domain.Entities;
using CookingRecipes.Api.Domain.Interfaces;
using CookingRecipes.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipes.Api.Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly DataContext _context;

        public IngredientService(DataContext context)
        {
            Guard.IsNotNull(context);

            _context = context;
        }

        public async Task<bool> CreateIngredientAsync(Ingredient ingredient)
        {
            var ingredientExists = await _context.Ingredients.AnyAsync(i => i.Name.ToLower() == ingredient.Name.ToLower());

            if (ingredientExists) return false;

            _context.Ingredients.Add(ingredient);

            var result = await _context.SaveChangesAsync().ConfigureAwait(false);

            return result > 0;
        }

        public async Task<IEnumerable<Ingredient>> GetAllIngredientsAsync()
        => await _context.Ingredients.ToListAsync().ConfigureAwait(false);
    }
}
