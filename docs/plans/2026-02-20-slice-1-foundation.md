# Slice 1: Foundation + Recipe Browsing — Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Set up PostgreSQL via Docker, create the Recipe entity with EF Core, convert from Razor Pages to MVC with PicoCSS/HTMX, and deliver a browsable recipe list + detail site with seed data.

**Architecture:** Lean Onion with 4 layers. Recipe entity in Domain, RecipeService in Application using DbContext directly, AppDbContext + EF config in Infrastructure, thin MVC controller + Razor views in Presentation. No repository pattern.

**Tech Stack:** .NET 9.0, PostgreSQL 16 (Docker), EF Core 9.0, ASP.NET Core MVC, PicoCSS (CDN), HTMX (CDN), xUnit + EF Core InMemory for tests.

---

### Task 1: Docker Compose for PostgreSQL

**Files:**
- Create: `docker-compose.yml`

**Step 1: Create docker-compose.yml**

```yaml
services:
  postgres:
    image: postgres:16
    container_name: relativeformulas-db
    environment:
      POSTGRES_USER: relativeformulas
      POSTGRES_PASSWORD: devpassword
      POSTGRES_DB: relativeformulas
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data

volumes:
  pgdata:
```

**Step 2: Verify PostgreSQL starts**

Run: `docker compose up -d`
Expected: Container `relativeformulas-db` starts, port 5432 accessible.

Run: `docker compose ps`
Expected: Shows `relativeformulas-db` running.

**Step 3: Commit**

```bash
git add docker-compose.yml
git commit -m "Add docker-compose for PostgreSQL 16 dev environment"
```

---

### Task 2: Update .gitignore

**Files:**
- Modify: `.gitignore`

**Step 1: Add entries**

Append to `.gitignore`:
```
appsettings.Local.json
.vs/
*.user
wwwroot/uploads/
```

**Step 2: Commit**

```bash
git add .gitignore
git commit -m "Update gitignore with IDE files, local config, uploads"
```

---

### Task 3: Add NuGet packages and project references

**Files:**
- Modify: `src/RelativeFormulas.Infrastructure/RelativeFormulas.Infrastructure.csproj`
- Modify: `src/RelativeFormulas.Presentation/RelativeFormulas.Presentation.csproj`

**Step 1: Add Npgsql to Infrastructure**

Run: `dotnet add src/RelativeFormulas.Infrastructure/RelativeFormulas.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL`

**Step 2: Add Presentation → Infrastructure project reference**

Run: `dotnet add src/RelativeFormulas.Presentation/RelativeFormulas.Presentation.csproj reference src/RelativeFormulas.Infrastructure/RelativeFormulas.Infrastructure.csproj`

**Step 3: Verify solution builds**

Run: `dotnet build RelativeFormulas.sln`
Expected: Build succeeded.

**Step 4: Commit**

```bash
git add src/RelativeFormulas.Infrastructure/RelativeFormulas.Infrastructure.csproj src/RelativeFormulas.Presentation/RelativeFormulas.Presentation.csproj
git commit -m "Add Npgsql package and Presentation->Infrastructure reference"
```

---

### Task 4: Create Recipe entity

**Files:**
- Create: `src/RelativeFormulas.Domain/Entities/Recipe.cs`
- Delete: `src/RelativeFormulas.Domain/Class1.cs`

**Step 1: Create Recipe entity**

```csharp
namespace RelativeFormulas.Domain.Entities;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string InstructionsText { get; set; } = string.Empty;
    public string TranscriptionText { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

**Step 2: Delete placeholder**

Delete `src/RelativeFormulas.Domain/Class1.cs`.

**Step 3: Verify build**

Run: `dotnet build src/RelativeFormulas.Domain/RelativeFormulas.Domain.csproj`
Expected: Build succeeded.

**Step 4: Commit**

```bash
git add src/RelativeFormulas.Domain/
git commit -m "Add Recipe entity, remove placeholder Class1"
```

---

### Task 5: Create AppDbContext with Recipe configuration

**Files:**
- Create: `src/RelativeFormulas.Infrastructure/Data/AppDbContext.cs`
- Create: `src/RelativeFormulas.Infrastructure/Data/Configurations/RecipeConfiguration.cs`
- Delete: `src/RelativeFormulas.Infrastructure/Class1.cs`

**Step 1: Create AppDbContext**

```csharp
using Microsoft.EntityFrameworkCore;
using RelativeFormulas.Domain.Entities;

namespace RelativeFormulas.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Recipe> Recipes => Set<Recipe>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

**Step 2: Create RecipeConfiguration**

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RelativeFormulas.Domain.Entities;

namespace RelativeFormulas.Infrastructure.Data.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Slug)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(r => r.Slug)
            .IsUnique();

        builder.Property(r => r.InstructionsText)
            .IsRequired();

        builder.Property(r => r.TranscriptionText)
            .IsRequired();

        builder.Property(r => r.ImagePath)
            .HasMaxLength(500);

        builder.Property(r => r.CreatedAt)
            .IsRequired();
    }
}
```

**Step 3: Delete placeholder**

Delete `src/RelativeFormulas.Infrastructure/Class1.cs`.

**Step 4: Verify build**

Run: `dotnet build src/RelativeFormulas.Infrastructure/RelativeFormulas.Infrastructure.csproj`
Expected: Build succeeded.

**Step 5: Commit**

```bash
git add src/RelativeFormulas.Infrastructure/
git commit -m "Add AppDbContext with Recipe entity configuration"
```

---

### Task 6: Configure connection string and register DbContext

**Files:**
- Modify: `src/RelativeFormulas.Presentation/appsettings.Local.json`
- Modify: `src/RelativeFormulas.Presentation/Program.cs`

**Step 1: Add connection string to appsettings.Local.json**

```json
{
  "DetailedErrors": true,
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=relativeformulas;Username=relativeformulas;Password=devpassword"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**Step 2: Register DbContext in Program.cs**

Add to `Program.cs` after `var builder = ...`:

```csharp
using Microsoft.EntityFrameworkCore;
using RelativeFormulas.Infrastructure.Data;
```

Replace `builder.Services.AddRazorPages();` with:

```csharp
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

Replace the routing section (`app.MapStaticAssets()` through `app.MapRazorPages()...`) with:

```csharp
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Recipes}/{action=Index}/{id?}")
    .WithStaticAssets();
```

**Step 3: Verify build**

Run: `dotnet build src/RelativeFormulas.Presentation/RelativeFormulas.Presentation.csproj`
Expected: Build succeeded.

**Step 4: Commit**

```bash
git add src/RelativeFormulas.Presentation/Program.cs
git commit -m "Register DbContext with Npgsql, switch to MVC routing"
```

Note: `appsettings.Local.json` is gitignored. Do NOT commit it.

---

### Task 7: Create initial migration

**Files:**
- Created by EF tooling: `src/RelativeFormulas.Infrastructure/Data/Migrations/`

**Step 1: Ensure PostgreSQL is running**

Run: `docker compose up -d`

**Step 2: Install EF Core tools if not present**

Run: `dotnet tool install --global dotnet-ef` (or `dotnet tool update --global dotnet-ef` if already installed)

**Step 3: Create migration**

Run: `dotnet ef migrations add InitialCreate -p src/RelativeFormulas.Infrastructure -s src/RelativeFormulas.Presentation`
Expected: Migration files created in `src/RelativeFormulas.Infrastructure/Data/Migrations/`.

**Step 4: Apply migration**

Run: `dotnet ef database update -p src/RelativeFormulas.Infrastructure -s src/RelativeFormulas.Presentation`
Expected: Database `relativeformulas` updated with `Recipes` table.

**Step 5: Commit**

```bash
git add src/RelativeFormulas.Infrastructure/Data/Migrations/
git commit -m "Add InitialCreate migration with Recipes table"
```

---

### Task 8: Seed development data

**Files:**
- Create: `src/RelativeFormulas.Infrastructure/Data/DbInitializer.cs`
- Modify: `src/RelativeFormulas.Presentation/Program.cs`

**Step 1: Create DbInitializer**

```csharp
using RelativeFormulas.Domain.Entities;

namespace RelativeFormulas.Infrastructure.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        if (context.Recipes.Any())
            return;

        var recipes = new[]
        {
            new Recipe
            {
                Title = "Grandma's Chocolate Chip Cookies",
                Slug = "grandmas-chocolate-chip-cookies",
                TranscriptionText = "2 1/4 cups flour, 1 tsp baking soda, 1 tsp salt, 1 cup butter softened, 3/4 cup sugar, 3/4 cup brown sugar, 2 eggs, 1 tsp vanilla, 2 cups chocolate chips. Mix dry ingredients. Cream butter and sugars. Add eggs and vanilla. Combine wet and dry. Fold in chips. Bake 375° for 9-11 min.",
                InstructionsText = "Preheat oven to 375°F. Mix flour, baking soda, and salt in a bowl. In a separate bowl, cream butter with both sugars until fluffy. Beat in eggs and vanilla. Gradually blend in flour mixture. Stir in chocolate chips. Drop by rounded tablespoon onto ungreased baking sheets. Bake 9 to 11 minutes or until golden brown.",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Recipe
            {
                Title = "Mom's Banana Bread",
                Slug = "moms-banana-bread",
                TranscriptionText = "3 ripe bananas, 1/3 cup melted butter, 3/4 cup sugar, 1 egg, 1 tsp vanilla, 1 tsp baking soda, pinch salt, 1 1/3 cups flour. Mash bananas, mix in butter. Add sugar, egg, vanilla. Sprinkle in baking soda and salt. Mix in flour. Pour into greased loaf pan. Bake 350° 55-65 min.",
                InstructionsText = "Preheat oven to 350°F. Mash ripe bananas in a mixing bowl. Stir in melted butter. Mix in sugar, beaten egg, and vanilla. Sprinkle in baking soda and salt, then mix in flour. Pour batter into a greased 4x8 inch loaf pan. Bake for 55 to 65 minutes, until a toothpick comes out clean.",
                CreatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            },
            new Recipe
            {
                Title = "Aunt Betty's Cornbread",
                Slug = "aunt-bettys-cornbread",
                TranscriptionText = "1 cup cornmeal, 1 cup flour, 1/4 cup sugar, 1 tbsp baking powder, 1/2 tsp salt, 1 cup milk, 1/3 cup vegetable oil, 1 egg. Mix dry, mix wet, combine. Pour into greased 8-inch pan. Bake 400° 20-25 min.",
                InstructionsText = "Preheat oven to 400°F. Combine cornmeal, flour, sugar, baking powder, and salt. In a separate bowl, whisk milk, oil, and egg. Pour wet ingredients into dry and stir until just combined. Pour into a greased 8-inch square baking pan. Bake 20 to 25 minutes until golden on top.",
                CreatedAt = new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc)
            },
            new Recipe
            {
                Title = "Grandpa's Chili",
                Slug = "grandpas-chili",
                TranscriptionText = "2 lbs ground beef, 1 onion diced, 3 cloves garlic, 2 cans kidney beans, 1 can crushed tomatoes, 2 tbsp chili powder, 1 tsp cumin, salt and pepper. Brown beef with onion and garlic. Add everything else. Simmer 45 min.",
                InstructionsText = "Brown ground beef in a large pot over medium-high heat. Add diced onion and cook until soft. Add minced garlic and cook 1 minute. Drain excess fat. Add kidney beans (drained), crushed tomatoes, chili powder, cumin, salt, and pepper. Bring to a boil, then reduce heat and simmer for 45 minutes, stirring occasionally.",
                CreatedAt = new DateTime(2026, 1, 4, 0, 0, 0, DateTimeKind.Utc)
            },
            new Recipe
            {
                Title = "Nana's Lemon Squares",
                Slug = "nanas-lemon-squares",
                TranscriptionText = "Crust: 1 cup butter, 1/2 cup powdered sugar, 2 cups flour. Filling: 4 eggs, 2 cups sugar, 1/3 cup lemon juice, 1/4 cup flour. Press crust into 9x13 pan, bake 350° 20 min. Pour filling over hot crust, bake 25 min more. Cool and dust with powdered sugar.",
                InstructionsText = "Preheat oven to 350°F. For the crust, cream butter and powdered sugar, then blend in flour. Press into a 9x13 inch pan. Bake 20 minutes until lightly golden. For the filling, beat eggs, sugar, lemon juice, and flour together. Pour over hot crust. Bake an additional 25 minutes. Cool completely and dust with powdered sugar before cutting.",
                CreatedAt = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        context.Recipes.AddRange(recipes);
        context.SaveChanges();
    }
}
```

**Step 2: Call seed from Program.cs**

Add to `Program.cs` after `var app = builder.Build();`:

```csharp
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    DbInitializer.Seed(context);
}
```

Add using at top:
```csharp
using RelativeFormulas.Infrastructure.Data;
```

**Step 3: Verify seed runs**

Run: `dotnet run --project src/RelativeFormulas.Presentation`
Expected: App starts without errors. (It will fail to serve pages since we haven't created MVC views yet, but the seed should run.)

Stop the app (Ctrl+C).

**Step 4: Commit**

```bash
git add src/RelativeFormulas.Infrastructure/Data/DbInitializer.cs src/RelativeFormulas.Presentation/Program.cs
git commit -m "Add seed data with 5 sample recipes, auto-migrate on startup"
```

---

### Task 9: Create test project

**Files:**
- Create: `tests/RelativeFormulas.Application.Tests/RelativeFormulas.Application.Tests.csproj`

**Step 1: Create test project**

```bash
mkdir -p tests
dotnet new xunit -n RelativeFormulas.Application.Tests -o tests/RelativeFormulas.Application.Tests
dotnet sln add tests/RelativeFormulas.Application.Tests/RelativeFormulas.Application.Tests.csproj
dotnet add tests/RelativeFormulas.Application.Tests/RelativeFormulas.Application.Tests.csproj reference src/RelativeFormulas.Application/RelativeFormulas.Application.csproj
dotnet add tests/RelativeFormulas.Application.Tests/RelativeFormulas.Application.Tests.csproj reference src/RelativeFormulas.Infrastructure/RelativeFormulas.Infrastructure.csproj
```

**Step 2: Add EF Core InMemory provider for testing**

```bash
dotnet add tests/RelativeFormulas.Application.Tests/RelativeFormulas.Application.Tests.csproj package Microsoft.EntityFrameworkCore.InMemory
```

**Step 3: Delete placeholder test, verify build**

Delete `tests/RelativeFormulas.Application.Tests/UnitTest1.cs`.

Run: `dotnet build tests/RelativeFormulas.Application.Tests/RelativeFormulas.Application.Tests.csproj`
Expected: Build succeeded.

**Step 4: Commit**

```bash
git add tests/ RelativeFormulas.sln
git commit -m "Add Application test project with xUnit and EF InMemory"
```

---

### Task 10: Create RecipeService with tests (TDD)

**Files:**
- Create: `src/RelativeFormulas.Application/Services/RecipeService.cs`
- Create: `tests/RelativeFormulas.Application.Tests/Services/RecipeServiceTests.cs`
- Delete: `src/RelativeFormulas.Application/Class1.cs`

**Step 1: Write failing tests**

Create `tests/RelativeFormulas.Application.Tests/Services/RecipeServiceTests.cs`:

```csharp
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
```

**Step 2: Run tests to verify they fail**

Run: `dotnet test tests/RelativeFormulas.Application.Tests/ --verbosity normal`
Expected: FAIL — `RecipeService` class does not exist.

**Step 3: Write minimal RecipeService implementation**

Create `src/RelativeFormulas.Application/Services/RecipeService.cs`:

```csharp
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
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Recipe?> GetBySlugAsync(string slug)
    {
        return await _context.Recipes
            .FirstOrDefaultAsync(r => r.Slug == slug);
    }
}
```

**Step 4: Delete Application placeholder**

Delete `src/RelativeFormulas.Application/Class1.cs`.

**Step 5: Run tests to verify they pass**

Run: `dotnet test tests/RelativeFormulas.Application.Tests/ --verbosity normal`
Expected: 3 tests passed.

**Step 6: Commit**

```bash
git add src/RelativeFormulas.Application/ tests/RelativeFormulas.Application.Tests/
git commit -m "Add RecipeService with GetAll and GetBySlug, tests passing"
```

---

### Task 11: Convert Presentation from Razor Pages to MVC

**Files:**
- Delete: `src/RelativeFormulas.Presentation/Pages/` (entire directory)
- Create: `src/RelativeFormulas.Presentation/Views/_ViewImports.cshtml`
- Create: `src/RelativeFormulas.Presentation/Views/_ViewStart.cshtml`
- Create: `src/RelativeFormulas.Presentation/Views/Shared/_Layout.cshtml`
- Create: `src/RelativeFormulas.Presentation/Views/Shared/Error.cshtml`

**Step 1: Delete Razor Pages directory**

Delete the entire `src/RelativeFormulas.Presentation/Pages/` directory and all contents.

**Step 2: Create Views/_ViewImports.cshtml**

```cshtml
@using RelativeFormulas.Presentation
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

**Step 3: Create Views/_ViewStart.cshtml**

```cshtml
@{
    Layout = "_Layout";
}
```

**Step 4: Create Views/Shared/_Layout.cshtml**

```cshtml
<!DOCTYPE html>
<html lang="en" data-theme="light">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Relative Formulas</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/@@picocss/pico@2/css/pico.min.css" />
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
</head>
<body>
    <header class="container">
        <nav>
            <ul>
                <li><a href="/" aria-label="Home"><strong>Relative Formulas</strong></a></li>
            </ul>
            <ul>
                <li><a asp-controller="Recipes" asp-action="Index">Recipes</a></li>
            </ul>
        </nav>
    </header>

    <main class="container">
        @RenderBody()
    </main>

    <footer class="container">
        <small>&copy; @DateTime.Now.Year Relative Formulas — A Family Recipe Archive</small>
    </footer>

    <script src="https://unpkg.com/htmx.org@2.0.4"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

**Step 5: Create Views/Shared/Error.cshtml**

```cshtml
@{
    ViewData["Title"] = "Error";
}

<article>
    <header>
        <h1>Error</h1>
    </header>
    <p>An error occurred while processing your request.</p>
</article>
```

**Step 6: Remove Bootstrap and jQuery from wwwroot**

Delete the entire `src/RelativeFormulas.Presentation/wwwroot/lib/` directory.

**Step 7: Verify build**

Run: `dotnet build src/RelativeFormulas.Presentation/RelativeFormulas.Presentation.csproj`
Expected: Build succeeded.

**Step 8: Commit**

```bash
git add -A src/RelativeFormulas.Presentation/
git commit -m "Convert from Razor Pages to MVC, replace Bootstrap with PicoCSS + HTMX"
```

---

### Task 12: Register RecipeService and create RecipesController

**Files:**
- Create: `src/RelativeFormulas.Presentation/Controllers/RecipesController.cs`
- Modify: `src/RelativeFormulas.Presentation/Program.cs`

**Step 1: Register RecipeService in DI**

Add to `Program.cs` after the `AddDbContext` call:

```csharp
using RelativeFormulas.Application.Services;
```

```csharp
builder.Services.AddScoped<RecipeService>();
```

**Step 2: Create RecipesController**

```csharp
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
```

**Step 3: Verify build**

Run: `dotnet build src/RelativeFormulas.Presentation/RelativeFormulas.Presentation.csproj`
Expected: Build succeeded.

**Step 4: Commit**

```bash
git add src/RelativeFormulas.Presentation/
git commit -m "Add RecipesController with Index and Detail actions"
```

---

### Task 13: Create Recipe views

**Files:**
- Create: `src/RelativeFormulas.Presentation/Views/Recipes/Index.cshtml`
- Create: `src/RelativeFormulas.Presentation/Views/Recipes/Detail.cshtml`

**Step 1: Create recipe list view**

Create `src/RelativeFormulas.Presentation/Views/Recipes/Index.cshtml`:

```cshtml
@model List<RelativeFormulas.Domain.Entities.Recipe>
@{
    ViewData["Title"] = "Recipes";
}

<h1>Family Recipes</h1>

@if (!Model.Any())
{
    <p>No recipes yet.</p>
}
else
{
    <div class="grid">
        @foreach (var recipe in Model)
        {
            <article>
                <header>
                    <h2>
                        <a asp-action="Detail" asp-route-slug="@recipe.Slug">@recipe.Title</a>
                    </h2>
                </header>
                <p>@recipe.TranscriptionText[..Math.Min(150, recipe.TranscriptionText.Length)]...</p>
                <footer>
                    <small>Added @recipe.CreatedAt.ToString("MMMM d, yyyy")</small>
                </footer>
            </article>
        }
    </div>
}
```

**Step 2: Create recipe detail view**

Create `src/RelativeFormulas.Presentation/Views/Recipes/Detail.cshtml`:

```cshtml
@model RelativeFormulas.Domain.Entities.Recipe
@{
    ViewData["Title"] = Model.Title;
}

<article>
    <header>
        <h1>@Model.Title</h1>
        <small>Added @Model.CreatedAt.ToString("MMMM d, yyyy")</small>
    </header>

    @if (!string.IsNullOrEmpty(Model.ImagePath))
    {
        <figure>
            <img src="@Model.ImagePath" alt="Original recipe card for @Model.Title" />
            <figcaption>Original recipe card</figcaption>
        </figure>
    }

    <section>
        <h2>Transcription</h2>
        <blockquote>@Model.TranscriptionText</blockquote>
    </section>

    <section>
        <h2>Instructions</h2>
        <p>@Model.InstructionsText</p>
    </section>
</article>

<a asp-action="Index">&larr; Back to all recipes</a>
```

**Step 3: Verify build**

Run: `dotnet build src/RelativeFormulas.Presentation/RelativeFormulas.Presentation.csproj`
Expected: Build succeeded.

**Step 4: Commit**

```bash
git add src/RelativeFormulas.Presentation/Views/Recipes/
git commit -m "Add recipe list and detail Razor views"
```

---

### Task 14: Create site.css and update routing for slug-based URLs

**Files:**
- Create: `src/RelativeFormulas.Presentation/wwwroot/css/site.css`
- Modify: `src/RelativeFormulas.Presentation/Program.cs` (add slug route)

**Step 1: Create site.css**

```css
/* Site-wide custom overrides for PicoCSS */

/* Recipe card grid */
.grid article {
    padding: 1rem;
}

.grid article h2 {
    margin-bottom: 0.5rem;
}

.grid article h2 a {
    text-decoration: none;
}

/* Recipe detail */
article blockquote {
    font-style: italic;
    border-left: 3px solid var(--pico-primary);
    padding-left: 1rem;
}

article figure img {
    max-width: 100%;
    height: auto;
    border-radius: var(--pico-border-radius);
}

/* Print styles */
@media print {
    header nav,
    footer {
        display: none;
    }

    article {
        break-inside: avoid;
    }
}
```

**Step 2: Add slug route to Program.cs**

Add a route for slug-based recipe URLs before the default route in `Program.cs`:

```csharp
app.MapControllerRoute(
    name: "recipe-detail",
    pattern: "recipes/{slug}",
    defaults: new { controller = "Recipes", action = "Detail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Recipes}/{action=Index}/{id?}")
    .WithStaticAssets();
```

**Step 3: Verify build**

Run: `dotnet build src/RelativeFormulas.Presentation/RelativeFormulas.Presentation.csproj`
Expected: Build succeeded.

**Step 4: Commit**

```bash
git add src/RelativeFormulas.Presentation/wwwroot/css/site.css src/RelativeFormulas.Presentation/Program.cs
git commit -m "Add site.css with PicoCSS overrides and slug-based recipe routing"
```

---

### Task 15: End-to-end verification

**Step 1: Ensure Docker is running**

Run: `docker compose up -d`

**Step 2: Drop and recreate database (clean slate)**

Run: `dotnet ef database drop -p src/RelativeFormulas.Infrastructure -s src/RelativeFormulas.Presentation --force`
Expected: Database dropped.

**Step 3: Run the application**

Run: `dotnet run --project src/RelativeFormulas.Presentation`
Expected: App starts, auto-migrates, seeds data.

**Step 4: Verify in browser**

- Open `http://localhost:1874` — Recipe list page shows 5 seeded recipes with PicoCSS styling.
- Click a recipe title — Detail page shows title, transcription (in blockquote), and instructions.
- URL should be `/recipes/grandmas-chocolate-chip-cookies` (slug-based).
- No Bootstrap artifacts visible (no `.navbar`, no `.container` Bootstrap styles).
- HTMX loaded (check browser dev tools → Network → htmx.org).

**Step 5: Run all tests**

Run: `dotnet test RelativeFormulas.sln --verbosity normal`
Expected: All 3 tests pass.

**Step 6: Final commit if any cleanup needed**

If everything works, no additional commit needed. If small fixes were required, commit them.

---

## Summary of Final Program.cs

After all tasks, `Program.cs` should look like:

```csharp
using Microsoft.EntityFrameworkCore;
using RelativeFormulas.Application.Services;
using RelativeFormulas.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<RecipeService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    DbInitializer.Seed(context);
}

if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Local"))
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "recipe-detail",
    pattern: "recipes/{slug}",
    defaults: new { controller = "Recipes", action = "Detail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Recipes}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
```
