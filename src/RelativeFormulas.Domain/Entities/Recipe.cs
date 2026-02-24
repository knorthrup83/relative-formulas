namespace RelativeFormulas.Domain.Entities;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string InstructionsText { get; set; } = string.Empty;
    public string TranscriptionText { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; }
}
