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
    public async Task<IActionResult> Create(RecipeFormViewModel vm, IFormFileCollection? images)
    {
        var id = await _adminService.CreateRecipeAsync(
            vm.Title, vm.Slug, vm.Notes,
            vm.InstructionsText, vm.TranscriptionText,
            ToEntries(vm.Ingredients), vm.SelectedTagIds);
        var paths = await SaveImagesAsync(images);
        if (paths.Any()) await _adminService.AddImagesAsync(id, paths);
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
    public async Task<IActionResult> Edit(int id, RecipeFormViewModel vm, IFormFileCollection? images)
    {
        await _adminService.UpdateRecipeAsync(
            id, vm.Title, vm.Slug, vm.Notes,
            vm.InstructionsText, vm.TranscriptionText,
            ToEntries(vm.Ingredients), vm.SelectedTagIds);
        var paths = await SaveImagesAsync(images);
        if (paths.Any()) await _adminService.AddImagesAsync(id, paths);
        return RedirectToAction("Edit", new { id });
    }

    [HttpPost("recipes/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _adminService.DeleteRecipeAsync(id);
        return RedirectToAction("Index");
    }

    [HttpPost("recipes/{id:int}/images/{imageId:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int id, int imageId)
    {
        var filePath = await _adminService.DeleteImageAsync(imageId);
        if (filePath is not null)
        {
            var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
        return RedirectToAction("Edit", new { id });
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

    private async Task<List<string>> SaveImagesAsync(IFormFileCollection? images)
    {
        var paths = new List<string>();
        if (images is null) return paths;
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsDir);
        foreach (var image in images.Where(f => f.Length > 0))
        {
            var ext = Path.GetExtension(image.FileName);
            var filename = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(uploadsDir, filename);
            await using var stream = new FileStream(fullPath, FileMode.Create);
            await image.CopyToAsync(stream);
            paths.Add($"/uploads/{filename}");
        }
        return paths;
    }

    private static IEnumerable<AdminService.IngredientEntry> ToEntries(List<IngredientRowViewModel> rows) =>
        rows.Where(r => !string.IsNullOrWhiteSpace(r.IngredientName))
            .Select((r, i) => new AdminService.IngredientEntry(
                r.IngredientName, r.Quantity, r.Unit, r.PreparationNote, i));
}
