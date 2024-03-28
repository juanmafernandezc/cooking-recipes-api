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
                    Ingredient = new IngredientDto
                    {
                        IngredientID = ri.Ingredient!.IngredientID,
                        Name = ri.Ingredient.Name,
                        Description = ri.Ingredient.Description
                    }
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
    }
}
