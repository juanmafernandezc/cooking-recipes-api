using CookingRecipes.Api.Domain.DTOs;
using CookingRecipes.Api.Domain.Entities;

namespace CookingRecipes.Api.Application.Extensions
{
    public static class MappingExtensions
    {
        public static RecipeDto ToDto(this Recipe recipe)
        {
            return new RecipeDto
            {
                RecipeID = recipe.RecipeID,
                UserID = recipe.UserID,
                Title = recipe.Title,
                Description = recipe.Description,
                Instructions = recipe.Instructions,
                PrepTime = recipe.PrepTime,
                CookTime = recipe.CookTime,
                Servings = recipe.Servings,
                RecipeIngredients = recipe.RecipeIngredients.Select(ri => new RecipeIngredientDto
                {
                    IngredientID = ri.IngredientID,
                    Quantity = ri.Quantity,
                    MeasureUnit = ri.MeasureUnit,
                    Ingredient = ri.Ingredient!.ToDto(),
                }).ToList()
            };
        }

        public static Recipe ToEntity(this RecipeDto recipeDto)
        {
            return new Recipe
            {
                RecipeID = recipeDto.RecipeID,
                UserID = recipeDto.UserID,
                Title = recipeDto.Title,
                Description = recipeDto.Description,
                Instructions = recipeDto.Instructions,
                PrepTime = recipeDto.PrepTime,
                CookTime = recipeDto.CookTime,
                Servings = recipeDto.Servings,
                RecipeIngredients = recipeDto.RecipeIngredients.Select(ri => new RecipeIngredient
                {
                    IngredientID = ri.IngredientID,
                    Quantity = ri.Quantity,
                    MeasureUnit = ri.MeasureUnit,
                }).ToList()
            };
        }

        public static IngredientDto ToDto(this Ingredient ingredient)
        {
            return new IngredientDto
            {
                IngredientID = ingredient.IngredientID,
                Name = ingredient.Name,
                Description = ingredient.Description
            };
        }

        public static Ingredient ToEntity(this IngredientDto ingredientDto)
        {
            return new Ingredient
            {
                IngredientID = ingredientDto.IngredientID,
                Name = ingredientDto.Name,
                Description = ingredientDto.Description
            };
        }
    }
}
