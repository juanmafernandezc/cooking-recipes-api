using CommunityToolkit.Diagnostics;
using CookingRecipes.Api.Application.Extensions;
using CookingRecipes.Api.Domain.DTOs;
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

        public async Task<bool> CreateRecipeAsync(RecipeDto recipe)
        {
            var recipeEntity = recipe.ToEntity();

            _context.Recipes.Add(recipeEntity);

            var result = await _context.SaveChangesAsync().ConfigureAwait(false);

            return result != 0;
        }

        public async Task<IEnumerable<RecipeDto>?> GetAllRecipesAsync()
        {
            var recipes = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ToListAsync().ConfigureAwait(false);

            if (recipes.Count == 0) return null;

            return recipes.Select(r => r.ToDto());
        }

        public async Task<RecipeDto?> GetRecipeByIdAsync(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.RecipeID == id).ConfigureAwait(false);

            if (recipe == null) return null;

            return recipe?.ToDto();
        }
        public async Task<bool> UpdateRecipeAsync(int id, RecipeDto recipeDto)
        {
            var existingRecipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .FirstOrDefaultAsync(r => r.RecipeID == id).ConfigureAwait(false);

            if (existingRecipe == null) return false;

            existingRecipe.Title = recipeDto.Title;
            existingRecipe.Description = recipeDto.Description;
            existingRecipe.Instructions = recipeDto.Instructions;
            existingRecipe.PrepTime = recipeDto.PrepTime;
            existingRecipe.CookTime = recipeDto.CookTime;
            existingRecipe.Servings = recipeDto.Servings;

            var newIngredientIds = recipeDto.RecipeIngredients.Select(i => i.IngredientID).ToList();

            var ingredientsToRemove = existingRecipe.RecipeIngredients
                .Where(ri => !newIngredientIds.Contains(ri.IngredientID))
                .ToList();

            foreach (var ingredient in ingredientsToRemove)
            {
                _context.RecipeIngredients.Remove(ingredient);
            }

            foreach (var ingredientDto in recipeDto.RecipeIngredients)
            {
                var existingIngredient = existingRecipe.RecipeIngredients
                    .FirstOrDefault(ri => ri.IngredientID == ingredientDto.IngredientID);

                if (existingIngredient != null)
                {
                    existingIngredient.Quantity = ingredientDto.Quantity;
                    existingIngredient.MeasureUnit = ingredientDto.MeasureUnit;
                }
                else
                {
                    existingRecipe.RecipeIngredients.Add(new RecipeIngredient
                    {
                        RecipeID = id,
                        IngredientID = ingredientDto.IngredientID,
                        Quantity = ingredientDto.Quantity,
                        MeasureUnit = ingredientDto.MeasureUnit
                    });
                }
            }

            var result = await _context.SaveChangesAsync().ConfigureAwait(false);

            return result != 0;
        }


        public async Task<bool> DeleteRecipeAsync(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.RecipeID == id).ConfigureAwait(false);

            if (recipe == null) return false;

            _context.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);
            _context.Recipes.Remove(recipe);

            var removed = await _context.SaveChangesAsync().ConfigureAwait(false);

            return removed != 0;
        }
    }
}
