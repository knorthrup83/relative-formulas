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
            .FirstOrDefaultAsync(r => r.Slug == slug);
    }
}
