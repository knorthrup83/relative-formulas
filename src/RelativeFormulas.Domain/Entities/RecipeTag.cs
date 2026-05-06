namespace RelativeFormulas.Domain.Entities;

public class RecipeTag
{
    public int RecipeId { get; set; }
    public int TagId { get; set; }

    // Navigation properties
    public Recipe Recipe { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
