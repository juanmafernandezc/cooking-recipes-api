namespace CookingRecipes.Api.Domain.Entities
{
    public class Ingredient
    {
        public int IngredientID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<RecipeIngredient>? RecipeIngredients { get; set; }
    }
}
