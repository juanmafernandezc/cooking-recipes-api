namespace CookingRecipes.Api.Domain.DTOs
{
    public class IngredientDto
    {
        public int IngredientID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
