using CommunityToolkit.Diagnostics;
using CookingRecipes.Api.Application.Extensions;
using CookingRecipes.Api.Common;
using CookingRecipes.Api.Domain.DTOs;
using CookingRecipes.Api.Domain.Interfaces;
using CookingRecipes.Api.Domain.Models.Responses;
using CookingRecipes.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipes.Api.Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly DataContext _context;

        private const string IngredientExistsMessage = "Ingredient already exists.";
        private const string IngredientCreatedMessage = "The ingredient has been successfully created.";
        private const string IngredientUpdatedMessage = "The ingredient has been successfully updated.";
        private const string IngredientDeletedMessage = "The ingredient has been successfully deleted.";
        private const string NoChangesDatabaseMessage = "No changes were made to the database.";
        private const string IngredientNotExistMessage = "The ingredient does not exist.";
        private const string IngredientAssociatedMessage = "The ingredient is associated with the recipe, it cannot be deleted.";


        public IngredientService(DataContext context)
        {
            Guard.IsNotNull(context);

            _context = context;
        }

        public async Task<ApiResponse<string?>> CreateIngredientAsync(IngredientDto ingredient)
        {
            var ingredientExists = await _context.Ingredients.AnyAsync(i => i.Name.ToLower() == ingredient.Name.ToLower());

            if (ingredientExists) return new ApiResponse<string?>(null, false, IngredientExistsMessage, HttpStatusCodes.Conflict);

            var ingredientEntity = ingredient.ToEntity();

            _context.Ingredients.Add(ingredientEntity);

            var result = await _context.SaveChangesAsync().ConfigureAwait(false);

            if (result != 0) return new ApiResponse<string?>(IngredientCreatedMessage);

            return new ApiResponse<string?>(null, false, NoChangesDatabaseMessage, HttpStatusCodes.InternalServerError);
        }

        public async Task<ApiResponse<IEnumerable<IngredientDto>?>> GetAllIngredientsAsync()
        {
            var ingredients = await _context.Ingredients.ToListAsync().ConfigureAwait(false);

            if (ingredients.Count == 0) return new ApiResponse<IEnumerable<IngredientDto>?>(null);

            return new ApiResponse<IEnumerable<IngredientDto>?>(ingredients.Select(r => r.ToDto()));
        }

        public async Task<ApiResponse<IngredientDto?>> GetIngredientByIdAsync(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id).ConfigureAwait(false);

            if (ingredient == null) return new ApiResponse<IngredientDto?>(null, false, IngredientNotExistMessage, HttpStatusCodes.NotFound);

            return new ApiResponse<IngredientDto?>(ingredient.ToDto());
        }

        public async Task<ApiResponse<string?>> UpdateIngredientAsync(int id, IngredientDto ingredientUpdated)
        {
            var ingredient = await _context.Ingredients.FindAsync(id).ConfigureAwait(false);
            if (ingredient == null) return new ApiResponse<string?>(null, false, IngredientNotExistMessage, HttpStatusCodes.NotFound);

            ingredient.Name = ingredientUpdated.Name;
            ingredient.Description = ingredientUpdated.Description;

            _context.Ingredients.Update(ingredient);

            var result = await _context.SaveChangesAsync().ConfigureAwait(false);

            if (result != 0) return new ApiResponse<string?>(IngredientUpdatedMessage, statusCode: HttpStatusCodes.Ok);

            return new ApiResponse<string?>(null, false, NoChangesDatabaseMessage, HttpStatusCodes.InternalServerError);
        }

        public async Task<ApiResponse<string?>> DeleteIngredientAsync(int ingredientId)
        {
            var ingredient = await _context.Ingredients
                .Include(i => i.RecipeIngredients)
                .FirstOrDefaultAsync(i => i.IngredientID == ingredientId);

            if (ingredient == null) return new ApiResponse<string?>(null, false, IngredientNotExistMessage, HttpStatusCodes.NotFound);

            if (ingredient.RecipeIngredients?.Any() == true) return new ApiResponse<string?>(null, false, IngredientAssociatedMessage, HttpStatusCodes.Conflict);

            _context.Ingredients.Remove(ingredient);

            var result = await _context.SaveChangesAsync().ConfigureAwait(false);

            if (result != 0) return new ApiResponse<string?>(IngredientDeletedMessage, statusCode: HttpStatusCodes.Ok);

            return new ApiResponse<string?>(null, false, NoChangesDatabaseMessage, HttpStatusCodes.Conflict);
        }
    }
}
