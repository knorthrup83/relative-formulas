namespace RelativeFormulas.Domain.Entities;

public class Favorite
{
    public int UserId { get; set; }
    public int RecipeId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Recipe Recipe { get; set; } = null!;
}
