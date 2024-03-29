using CommunityToolkit.Diagnostics;
using CookingRecipes.Api.Application.Extensions;
using CookingRecipes.Api.Common;
using CookingRecipes.Api.Domain.DTOs;
using CookingRecipes.Api.Domain.Entities;
using CookingRecipes.Api.Domain.Interfaces;
using CookingRecipes.Api.Domain.Models.Responses;
using CookingRecipes.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace CookingRecipes.Api.Application.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly DataContext _context;

        private const string UserNotExistsMessage = "User not exists.";
        private const string NotValidIngredientsMessage = "Not valid ingredients.";
        private const string RecipeCreatedMessage = "The recipe has been successfully created.";
        private const string NoChangesDatabaseMessage = "No changes were made to the database.";
        private const string RecipeNotFoundMessage = "Recipe not found.";
        private const string RecipeNotExistMessage = "The recipe does not exist.";
        private const string RecipeUpdatedMessage = "The recipe has been successfully updated.";
        private const string RecipeDeletedMessage = "The recipe has been successfully deleted.";
        private const string RecipeMismatchMessage = "Recipe ID mismatch.";
        
        public RecipeService(DataContext context)
        {
            Guard.IsNotNull(context);

            _context = context;
        }

        public async Task<ApiResponse<string?>> CreateRecipeAsync(RecipeDto recipe)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == recipe.UserID).ConfigureAwait(false);
            if (user == null) return new ApiResponse<string?>(null, false, UserNotExistsMessage, HttpStatusCodes.NotFound);

            if (!await IsValidIngredients(recipe).ConfigureAwait(false)) return new ApiResponse<string?>(null, false, NotValidIngredientsMessage, HttpStatusCodes.BadRequest);

            var recipeEntity = recipe.ToEntity();

            _context.Recipes.Add(recipeEntity);

            var result = await _context.SaveChangesAsync().ConfigureAwait(false);

            if (result != 0) return new ApiResponse<string?>(RecipeCreatedMessage);

            return new ApiResponse<string?>(null, false, NoChangesDatabaseMessage, HttpStatusCodes.InternalServerError);
        }

        public async Task<ApiResponse<IEnumerable<RecipeDto>?>> GetAllRecipesAsync()
        {
            var recipes = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ToListAsync().ConfigureAwait(false);

            if (recipes.Count == 0) return new ApiResponse<IEnumerable<RecipeDto>?>(null);

            return new ApiResponse<IEnumerable<RecipeDto>?>(recipes.Select(r => r.ToDto()));
        }

        public async Task<ApiResponse<RecipeDto?>> GetRecipeByIdAsync(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.RecipeID == id).ConfigureAwait(false);

            if (recipe == null) return new ApiResponse<RecipeDto?>(null, false, RecipeNotFoundMessage, HttpStatusCodes.NotFound);

            return new ApiResponse<RecipeDto?>(recipe?.ToDto());
        }

        public async Task<ApiResponse<string?>> UpdateRecipeAsync(int id, RecipeDto recipeDto)
        {
            if (id != recipeDto.RecipeID) return new ApiResponse<string?>(null, false, RecipeMismatchMessage, HttpStatusCodes.BadRequest);

            var existingRecipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .FirstOrDefaultAsync(r => r.RecipeID == id).ConfigureAwait(false);

            if (existingRecipe == null) return new ApiResponse<string?>(null, false, RecipeNotExistMessage, HttpStatusCodes.NotFound);

            var newIngredientIds = recipeDto.RecipeIngredients.Select(i => i.IngredientID).Distinct().ToList();
            foreach (var ingredientId in newIngredientIds)
            {
                var ingredientExists = await _context.Ingredients.AnyAsync(i => i.IngredientID == ingredientId);
                if (!ingredientExists) return new ApiResponse<string?>(null, false, $"Ingredient with ID {ingredientId} not found.", HttpStatusCodes.NotFound);
            }

            existingRecipe.Title = recipeDto.Title;
            existingRecipe.Description = recipeDto.Description;
            existingRecipe.Instructions = recipeDto.Instructions;
            existingRecipe.PrepTime = recipeDto.PrepTime;
            existingRecipe.CookTime = recipeDto.CookTime;
            existingRecipe.Servings = recipeDto.Servings;

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

            if (result != 0) return new ApiResponse<string?>(RecipeUpdatedMessage, statusCode: HttpStatusCodes.Ok);

            return new ApiResponse<string?>(null, false, NoChangesDatabaseMessage, HttpStatusCodes.InternalServerError);
        }


        public async Task<ApiResponse<string?>> DeleteRecipeAsync(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.RecipeID == id).ConfigureAwait(false);

            if (recipe == null) return new ApiResponse<string?>(null, false, RecipeNotFoundMessage, HttpStatusCodes.NotFound);

            _context.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);
            _context.Recipes.Remove(recipe);

            var removed = await _context.SaveChangesAsync().ConfigureAwait(false);

            if (removed != 0) return new ApiResponse<string?>(RecipeDeletedMessage, statusCode: HttpStatusCodes.Ok);

            return new ApiResponse<string?>(null, false, NoChangesDatabaseMessage, HttpStatusCodes.InternalServerError);
        }

        private async Task<bool> IsValidIngredients(RecipeDto recipeDto)
        => !(HasDuplicateIngredients(recipeDto) || !await AllIngredientsExist(recipeDto).ConfigureAwait(false) || !AllQuantitiesAreValid(recipeDto));

        private static bool HasDuplicateIngredients(RecipeDto recipeDto)
        => recipeDto.RecipeIngredients.GroupBy(ri => ri.IngredientID).Any(g => g.Count() > 1);

        private async Task<bool> AllIngredientsExist(RecipeDto recipeDto)
        {
            var ingredientIds = recipeDto.RecipeIngredients.Select(ri => ri.IngredientID).Distinct();
            foreach (var ingredientId in ingredientIds)
            {
                var ingredientExists = await _context.Ingredients.AnyAsync(i => i.IngredientID == ingredientId).ConfigureAwait(false);
                if (!ingredientExists) return false;
            }

            return true;
        }

        private static bool AllQuantitiesAreValid(RecipeDto recipeDto)
        => recipeDto.RecipeIngredients.All(ri => ri.Quantity > 0);
    }
}
