using Microsoft.EntityFrameworkCore;
using RelativeFormulas.Domain.Entities;
using RelativeFormulas.Infrastructure.Data;

namespace RelativeFormulas.Application.Services;

public class RecipeService
{
    private readonly AppDbContext _context;

    public RecipeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Recipe>> GetAllAsync()
    {
        return await _context.Recipes
            .Include(r => r.RecipeTags)
                .ThenInclude(rt => rt.Tag)
            .Include(r => r.Images)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Recipe?> GetBySlugAsync(string slug)
    {
        return await _context.Recipes
            .Include(r => r.RecipeTags)
                .ThenInclude(rt => rt.Tag)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.Images)
            .FirstOrDefaultAsync(r => r.Slug == slug);
    }

    public async Task<List<Recipe>> SearchAsync(string? q, string? tagSlug)
    {
        var query = _context.Recipes
            .Include(r => r.RecipeTags)
                .ThenInclude(rt => rt.Tag)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.Images)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(tagSlug))
            query = query.Where(r => r.RecipeTags.Any(rt => rt.Tag.Slug == tagSlug));

        if (!string.IsNullOrWhiteSpace(q))
        {
            var pattern = $"%{q}%";
            query = query.Where(r =>
                EF.Functions.ILike(r.Title, pattern) ||
                EF.Functions.ILike(r.InstructionsText, pattern) ||
                EF.Functions.ILike(r.TranscriptionText, pattern) ||
                r.RecipeIngredients.Any(ri => EF.Functions.ILike(ri.Ingredient.Name, pattern)) ||
                r.RecipeTags.Any(rt => EF.Functions.ILike(rt.Tag.Name, pattern)));
        }

        return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
    }

    public async Task<List<Tag>> GetAllTagsAsync()
    {
        return await _context.Tags
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<Tag?> GetTagBySlugAsync(string slug)
    {
        return await _context.Tags.FirstOrDefaultAsync(t => t.Slug == slug);
    }
}
