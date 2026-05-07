using RelativeFormulas.Domain.Entities;

namespace RelativeFormulas.Presentation.ViewModels;

public class RecipeFormViewModel
{
    public int? Id { get; set; }
    public string Title { get; set; } = "";
    public string Slug { get; set; } = "";
    public string Notes { get; set; } = "";
    public string InstructionsText { get; set; } = "";
    public string TranscriptionText { get; set; } = "";
    public List<int> SelectedTagIds { get; set; } = [];
    public List<IngredientRowViewModel> Ingredients { get; set; } = [];

    // Populated before rendering the form view
    public List<Tag> AllTags { get; set; } = [];
    public List<RecipeImageViewModel> ExistingImages { get; set; } = [];

    public static RecipeFormViewModel FromRecipe(Recipe recipe, List<Tag> allTags) => new()
    {
        Id = recipe.Id,
        Title = recipe.Title,
        Slug = recipe.Slug,
        Notes = recipe.Notes ?? "",
        InstructionsText = recipe.InstructionsText,
        TranscriptionText = recipe.TranscriptionText,
        ExistingImages = recipe.Images
            .OrderBy(i => i.SortOrder)
            .Select(i => new RecipeImageViewModel
            {
                Id = i.Id,
                FilePath = i.FilePath,
                Caption = i.Caption,
                SortOrder = i.SortOrder
            }).ToList(),
        SelectedTagIds = recipe.RecipeTags.Select(rt => rt.TagId).ToList(),
        AllTags = allTags,
        Ingredients = recipe.RecipeIngredients
            .OrderBy(ri => ri.SortOrder)
            .Select((ri, i) => new IngredientRowViewModel
            {
                Index = i,
                IngredientId = ri.IngredientId,
                IngredientName = ri.Ingredient.Name,
                Quantity = ri.Quantity,
                Unit = ri.Unit,
                PreparationNote = ri.PreparationNote,
                SortOrder = ri.SortOrder
            }).ToList()
    };
}
