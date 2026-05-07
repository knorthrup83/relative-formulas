namespace RelativeFormulas.Domain.Entities;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string InstructionsText { get; set; } = string.Empty;
    public string TranscriptionText { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    public ICollection<RecipeTag> RecipeTags { get; set; } = new List<RecipeTag>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<RecipeImage> Images { get; set; } = new List<RecipeImage>();
}
