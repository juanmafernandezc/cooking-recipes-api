using CommunityToolkit.Diagnostics;
using CookingRecipes.Api.Domain.Entities;
using CookingRecipes.Api.Domain.Interfaces;
using CookingRecipes.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipes.Api.Application.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly DataContext _context;

        public RecipeService(DataContext context)
        {
            Guard.IsNotNull(context);

            _context = context;
        }

        public async Task<Recipe> CreateRecipeAsync(Recipe recipe)
        {
            _context.Recipes.Add(recipe);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return recipe;
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
        => await _context.Recipes.ToListAsync().ConfigureAwait(false);

        public async Task<Recipe?> GetRecipeByIdAsync(int id)
        => await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.RecipeID == id).ConfigureAwait(false);

        public Task DeleteRecipeAsync(int id)
        {
            throw new NotImplementedException();
        }


        public Task UpdateRecipeAsync(int id, Recipe recipe)
        {
            throw new NotImplementedException();
        }
    }
}
