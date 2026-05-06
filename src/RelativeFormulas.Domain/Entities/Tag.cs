namespace RelativeFormulas.Domain.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<RecipeTag> RecipeTags { get; set; } = new List<RecipeTag>();
}
