namespace RelativeFormulas.Domain.Entities;

public class RecipeImage
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public int SortOrder { get; set; }

    public Recipe Recipe { get; set; } = null!;
}
