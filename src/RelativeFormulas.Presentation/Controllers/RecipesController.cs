using Microsoft.AspNetCore.Mvc;
using RelativeFormulas.Application.Services;

namespace RelativeFormulas.Presentation.Controllers;

public class RecipesController : Controller
{
    private readonly RecipeService _recipeService;

    public RecipesController(RecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public async Task<IActionResult> Index()
    {
        var recipes = await _recipeService.GetAllAsync();
        return View(recipes);
    }

    public async Task<IActionResult> Detail(string slug)
    {
        if (string.IsNullOrEmpty(slug))
            return NotFound();

        var recipe = await _recipeService.GetBySlugAsync(slug);
        if (recipe is null)
            return NotFound();

        return View(recipe);
    }
}
