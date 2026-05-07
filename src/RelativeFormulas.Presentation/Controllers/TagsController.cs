using Microsoft.AspNetCore.Mvc;
using RelativeFormulas.Application.Services;

namespace RelativeFormulas.Presentation.Controllers;

public class TagsController : Controller
{
    private readonly RecipeService _recipeService;

    public TagsController(RecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public async Task<IActionResult> Index()
    {
        var tags = await _recipeService.GetAllTagsAsync();
        return View(tags);
    }

    public async Task<IActionResult> Detail(string slug)
    {
        var tag = await _recipeService.GetTagBySlugAsync(slug);
        if (tag is null)
            return NotFound();

        var recipes = await _recipeService.SearchAsync(null, slug);
        ViewBag.Tag = tag;
        return View(recipes);
    }
}
