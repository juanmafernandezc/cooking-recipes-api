namespace CookingRecipes.Api.Domain.Entities
{
    public class RecipeIngredient
    {
        public int RecipeID { get; set; }
        public int IngredientID { get; set; }
        public decimal Quantity { get; set; }
        public string MeasureUnit { get; set; } = string.Empty;
        public Recipe? Recipe { get; set; }
        public Ingredient? Ingredient { get; set; }
    }
}
