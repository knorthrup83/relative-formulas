namespace RelativeFormulas.Presentation.ViewModels;

public class RecipeImageViewModel
{
    public int Id { get; set; }
    public string FilePath { get; set; } = "";
    public string? Caption { get; set; }
    public int SortOrder { get; set; }
}
