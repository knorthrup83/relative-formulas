using Microsoft.EntityFrameworkCore;
using RelativeFormulas.Domain.Entities;
using RelativeFormulas.Infrastructure.Data;

namespace RelativeFormulas.Application.Services;

public class FavoriteService
{
    private readonly AppDbContext _context;

    public FavoriteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsFavoritedAsync(int userId, int recipeId)
    {
        return await _context.Favorites
            .AnyAsync(f => f.UserId == userId && f.RecipeId == recipeId);
    }

    public async Task ToggleAsync(int userId, int recipeId)
    {
        var existing = await _context.Favorites
            .SingleOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId);

        if (existing is null)
        {
            _context.Favorites.Add(new Favorite
            {
                UserId = userId,
                RecipeId = recipeId,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            _context.Favorites.Remove(existing);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<Recipe>> GetUserFavoritesAsync(int userId)
    {
        return await _context.Recipes
            .Where(r => r.Favorites.Any(f => f.UserId == userId))
            .Include(r => r.RecipeTags)
                .ThenInclude(rt => rt.Tag)
            .ToListAsync();
    }
}
