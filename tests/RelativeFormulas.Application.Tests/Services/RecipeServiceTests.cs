using Microsoft.EntityFrameworkCore;
using RelativeFormulas.Application.Services;
using RelativeFormulas.Domain.Entities;
using RelativeFormulas.Infrastructure.Data;

namespace RelativeFormulas.Application.Tests.Services;

public class RecipeServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static void SeedRecipes(AppDbContext context)
    {
        context.Recipes.AddRange(
            new Recipe
            {
                Title = "Chocolate Cake",
                Slug = "chocolate-cake",
                InstructionsText = "Mix and bake.",
                TranscriptionText = "Original text.",
                CreatedAt = DateTime.UtcNow
            },
            new Recipe
            {
                Title = "Banana Bread",
                Slug = "banana-bread",
                InstructionsText = "Mash and bake.",
                TranscriptionText = "Original text.",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        );
        context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllRecipes()
    {
        using var context = CreateContext();
        SeedRecipes(context);
        var service = new RecipeService(context);

        var recipes = await service.GetAllAsync();

        Assert.Equal(2, recipes.Count);
    }

    [Fact]
    public async Task GetBySlugAsync_ReturnsCorrectRecipe()
    {
        using var context = CreateContext();
        SeedRecipes(context);
        var service = new RecipeService(context);

        var recipe = await service.GetBySlugAsync("chocolate-cake");

        Assert.NotNull(recipe);
        Assert.Equal("Chocolate Cake", recipe!.Title);
    }

    [Fact]
    public async Task GetBySlugAsync_ReturnsNullForUnknownSlug()
    {
        using var context = CreateContext();
        SeedRecipes(context);
        var service = new RecipeService(context);

        var recipe = await service.GetBySlugAsync("nonexistent");

        Assert.Null(recipe);
    }
}
