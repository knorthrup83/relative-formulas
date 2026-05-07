using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using RelativeFormulas.Domain.Entities;
using RelativeFormulas.Infrastructure.Data;

namespace RelativeFormulas.Application.Services;

public class AdminService
{
    public record IngredientEntry(string Name, string Quantity, string Unit, string? PrepNote, int SortOrder);

    private readonly AppDbContext _context;

    public AdminService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Recipe>> GetAllRecipesAsync()
    {
        return await _context.Recipes
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Recipe?> GetRecipeForEditAsync(int id)
    {
        return await _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeTags)
                .ThenInclude(rt => rt.Tag)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<int> CreateRecipeAsync(
        string title, string slug, string? notes,
        string instructionsText, string transcriptionText,
        string? imagePath, IEnumerable<IngredientEntry> ingredients, IEnumerable<int> tagIds)
    {
        var recipe = new Recipe
        {
            Title = title,
            Slug = string.IsNullOrWhiteSpace(slug) ? GenerateSlug(title) : slug,
            Notes = notes,
            InstructionsText = instructionsText,
            TranscriptionText = transcriptionText,
            ImagePath = imagePath,
            CreatedAt = DateTime.UtcNow
        };

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        await SetIngredientsAsync(recipe.Id, ingredients);
        SetTags(recipe.Id, tagIds);
        await _context.SaveChangesAsync();

        return recipe.Id;
    }

    public async Task UpdateRecipeAsync(
        int id, string title, string slug, string? notes,
        string instructionsText, string transcriptionText,
        string? imagePath, IEnumerable<IngredientEntry> ingredients, IEnumerable<int> tagIds)
    {
        var recipe = await _context.Recipes.FindAsync(id)
            ?? throw new InvalidOperationException($"Recipe {id} not found.");

        recipe.Title = title;
        recipe.Slug = string.IsNullOrWhiteSpace(slug) ? GenerateSlug(title) : slug;
        recipe.Notes = notes;
        recipe.InstructionsText = instructionsText;
        recipe.TranscriptionText = transcriptionText;
        if (imagePath is not null)
            recipe.ImagePath = imagePath;

        await _context.RecipeIngredients.Where(ri => ri.RecipeId == id).ExecuteDeleteAsync();
        await _context.RecipeTags.Where(rt => rt.RecipeId == id).ExecuteDeleteAsync();

        await SetIngredientsAsync(id, ingredients);
        SetTags(id, tagIds);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRecipeAsync(int id)
    {
        await _context.Recipes.Where(r => r.Id == id).ExecuteDeleteAsync();
    }

    public async Task<List<Ingredient>> SearchIngredientsAsync(string query)
    {
        return await _context.Ingredients
            .Where(i => EF.Functions.ILike(i.Name, $"%{query}%"))
            .OrderBy(i => i.Name)
            .Take(10)
            .ToListAsync();
    }

    public async Task<List<Tag>> GetAllTagsAsync()
    {
        return await _context.Tags.OrderBy(t => t.Name).ToListAsync();
    }

    public async Task<Ingredient> GetOrCreateIngredientAsync(string name)
    {
        var normalized = name.Trim();
        var existing = await _context.Ingredients
            .FirstOrDefaultAsync(i => i.Name.ToLower() == normalized.ToLower());

        if (existing is not null)
            return existing;

        var ingredient = new Ingredient { Name = normalized };
        _context.Ingredients.Add(ingredient);
        await _context.SaveChangesAsync();
        return ingredient;
    }

    private async Task SetIngredientsAsync(int recipeId, IEnumerable<IngredientEntry> entries)
    {
        var sorted = entries
            .Where(e => !string.IsNullOrWhiteSpace(e.Name))
            .Select((e, i) => (entry: e, index: i));

        foreach (var (entry, index) in sorted)
        {
            var ingredient = await GetOrCreateIngredientAsync(entry.Name);
            _context.RecipeIngredients.Add(new RecipeIngredient
            {
                RecipeId = recipeId,
                IngredientId = ingredient.Id,
                Quantity = entry.Quantity ?? "",
                Unit = entry.Unit ?? "",
                PreparationNote = entry.PrepNote,
                SortOrder = index
            });
        }
    }

    private void SetTags(int recipeId, IEnumerable<int> tagIds)
    {
        foreach (var tagId in tagIds)
            _context.RecipeTags.Add(new RecipeTag { RecipeId = recipeId, TagId = tagId });
    }

    private static string GenerateSlug(string title)
    {
        var lower = title.ToLower().Trim().Replace(" ", "-");
        return Regex.Replace(lower, @"[^a-z0-9\-]", "");
    }
}
