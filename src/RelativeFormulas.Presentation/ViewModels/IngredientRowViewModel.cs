namespace RelativeFormulas.Presentation.ViewModels;

public class IngredientRowViewModel
{
    public int Index { get; set; }
    public int? IngredientId { get; set; }
    public string IngredientName { get; set; } = "";
    public string Quantity { get; set; } = "";
    public string Unit { get; set; } = "";
    public string? PreparationNote { get; set; }
    public int SortOrder { get; set; }
}
