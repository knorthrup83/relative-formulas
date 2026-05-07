using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RelativeFormulas.Application.Services;
using RelativeFormulas.Presentation.ViewModels;

namespace RelativeFormulas.Presentation.Controllers;

[Route("recipes")]
public class RecipesController : Controller
{
    private readonly RecipeService _recipeService;
    private readonly FavoriteService _favoriteService;

    public RecipesController(RecipeService recipeService, FavoriteService favoriteService)
    {
        _recipeService = recipeService;
        _favoriteService = favoriteService;
    }

    [HttpGet("/recipes")]
    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        var recipes = await _recipeService.GetAllAsync();
        return View(recipes);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string? q, string? tagSlug)
    {
        var recipes = await _recipeService.SearchAsync(q, tagSlug);
        ViewBag.Query = q;
        ViewBag.TagSlug = tagSlug;

        if (Request.Headers.ContainsKey("HX-Request"))
            return PartialView("_RecipeList", recipes);

        return View(recipes);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        if (string.IsNullOrEmpty(slug))
            return NotFound();

        var recipe = await _recipeService.GetBySlugAsync(slug);
        if (recipe is null)
            return NotFound();

        var isLoggedIn = User.Identity?.IsAuthenticated == true;
        var isFavorited = false;

        if (isLoggedIn)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            isFavorited = await _favoriteService.IsFavoritedAsync(userId, recipe.Id);
        }

        ViewBag.FavoriteButton = new FavoriteButtonViewModel(slug, isFavorited, isLoggedIn);
        return View(recipe);
    }

    [HttpGet("{slug}/print")]
    public async Task<IActionResult> Print(string slug)
    {
        var recipe = await _recipeService.GetBySlugAsync(slug);
        if (recipe is null)
            return NotFound();
        return View(recipe);
    }

    [HttpPost("{slug}/favorite")]
    public async Task<IActionResult> Favorite(string slug) => await HandleToggle(slug);

    [HttpPost("{slug}/unfavorite")]
    public async Task<IActionResult> Unfavorite(string slug) => await HandleToggle(slug);

    private async Task<IActionResult> HandleToggle(string slug)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            if (Request.Headers.ContainsKey("HX-Request"))
            {
                Response.Headers["HX-Redirect"] = "/login";
                return Ok();
            }
            return RedirectToAction("Login", "Account");
        }

        var recipe = await _recipeService.GetBySlugAsync(slug);
        if (recipe is null)
            return NotFound();

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _favoriteService.ToggleAsync(userId, recipe.Id);
        var isFavorited = await _favoriteService.IsFavoritedAsync(userId, recipe.Id);

        var model = new FavoriteButtonViewModel(slug, isFavorited, true);

        if (Request.Headers.ContainsKey("HX-Request"))
            return PartialView("_FavoriteButton", model);

        return RedirectToAction("Detail", new { slug });
    }
}
