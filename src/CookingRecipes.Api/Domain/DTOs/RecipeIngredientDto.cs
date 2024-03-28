namespace CookingRecipes.Api.Domain.DTOs
{
    public class RecipeIngredientDto
    {
        public int IngredientID { get; set; }
        public decimal Quantity { get; set; }
        public string MeasureUnit { get; set; } = string.Empty;
        public IngredientDto Ingredient { get; set; } = new();
    }
}
