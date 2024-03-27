namespace CookingRecipes.Api.Domain.Entities
{
    public class Recipe
    {
        public int RecipeID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public int Servings { get; set; }
        public User? User { get; set; }
        public List<RecipeIngredient>? RecipeIngredients { get; set; }
    }
}
