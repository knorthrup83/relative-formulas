namespace RelativeFormulas.Domain.Entities;

public class RecipeIngredient
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public int IngredientId { get; set; }
    public string Quantity { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;

    // Navigation properties
    public Recipe Recipe { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;
}
