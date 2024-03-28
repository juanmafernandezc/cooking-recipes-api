namespace CookingRecipes.Api.Domain.DTOs
{
    public class RecipeDto
    {
        public int RecipeID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public int Servings { get; set; }
        public List<RecipeIngredientDto> RecipeIngredients { get; set; } = new();
    }
}
