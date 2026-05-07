using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelativeFormulas.Application.Services;

namespace RelativeFormulas.Presentation.Controllers;

[Authorize]
[Route("favorites")]
public class FavoritesController : Controller
{
    private readonly FavoriteService _favoriteService;

    public FavoritesController(FavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var recipes = await _favoriteService.GetUserFavoritesAsync(userId);
        return View(recipes);
    }
}
