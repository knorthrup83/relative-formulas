using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelativeFormulas.Application.Services;
using RelativeFormulas.Domain.Entities;
using RelativeFormulas.Presentation.ViewModels;

namespace RelativeFormulas.Presentation.Controllers;

[Authorize(Policy = "AdminOnly")]
[Route("admin")]
public class AdminController : Controller
{
    private readonly AdminService _adminService;
    private readonly IWebHostEnvironment _env;

    public AdminController(AdminService adminService, IWebHostEnvironment env)
    {
        _adminService = adminService;
        _env = env;
    }

    [HttpGet("recipes")]
    public async Task<IActionResult> Index()
    {
        var recipes = await _adminService.GetAllRecipesAsync();
        return View(recipes);
    }

    [HttpGet("recipes/new")]
    public async Task<IActionResult> Create()
    {
        var vm = new RecipeFormViewModel { AllTags = await _adminService.GetAllTagsAsync() };
        return View("Form", vm);
    }

    [HttpPost("recipes")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RecipeFormViewModel vm, IFormFile? image)
    {
        var imagePath = await SaveImageAsync(image);
        var id = await _adminService.CreateRecipeAsync(
            vm.Title, vm.Slug, vm.Notes,
            vm.InstructionsText, vm.TranscriptionText,
            imagePath, ToEntries(vm.Ingredients), vm.SelectedTagIds);
        return RedirectToAction("Edit", new { id });
    }

    [HttpGet("recipes/{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var recipe = await _adminService.GetRecipeForEditAsync(id);
        if (recipe is null) return NotFound();
        var vm = RecipeFormViewModel.FromRecipe(recipe, await _adminService.GetAllTagsAsync());
        return View("Form", vm);
    }

    [HttpPost("recipes/{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RecipeFormViewModel vm, IFormFile? image)
    {
        var imagePath = await SaveImageAsync(image);
        await _adminService.UpdateRecipeAsync(
            id, vm.Title, vm.Slug, vm.Notes,
            vm.InstructionsText, vm.TranscriptionText,
            imagePath, ToEntries(vm.Ingredients), vm.SelectedTagIds);
        return RedirectToAction("Edit", new { id });
    }

    [HttpPost("recipes/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _adminService.DeleteRecipeAsync(id);
        return RedirectToAction("Index");
    }

    [HttpGet("recipes/ingredient-row")]
    public IActionResult IngredientRow(int index)
    {
        return PartialView("_IngredientRow", new IngredientRowViewModel { Index = index });
    }

    [HttpGet("ingredients/search")]
    public async Task<IActionResult> SearchIngredients(string? q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return PartialView("~/Views/Admin/Ingredients/_SearchResults.cshtml", new List<Ingredient>());
        var results = await _adminService.SearchIngredientsAsync(q);
        return PartialView("~/Views/Admin/Ingredients/_SearchResults.cshtml", results);
    }

    private async Task<string?> SaveImageAsync(IFormFile? image)
    {
        if (image is null || image.Length == 0) return null;
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsDir);
        var ext = Path.GetExtension(image.FileName);
        var filename = $"{Guid.NewGuid()}{ext}";
        var path = Path.Combine(uploadsDir, filename);
        await using var stream = new FileStream(path, FileMode.Create);
        await image.CopyToAsync(stream);
        return $"/uploads/{filename}";
    }

    private static IEnumerable<AdminService.IngredientEntry> ToEntries(List<IngredientRowViewModel> rows) =>
        rows.Where(r => !string.IsNullOrWhiteSpace(r.IngredientName))
            .Select((r, i) => new AdminService.IngredientEntry(
                r.IngredientName, r.Quantity, r.Unit, r.PreparationNote, i));
}
