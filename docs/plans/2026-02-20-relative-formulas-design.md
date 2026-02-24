# RelativeFormulas — Family Recipe Archive Design & Implementation Plan

## Context

Build a family-only recipe website to preserve and share grandmother's handwritten recipes. Each recipe includes a scanned image of the original recipe card, a faithful transcription, structured ingredient data, tags for browsing, and user accounts for favorites. The project prioritizes longevity, simplicity, and minimal maintenance over novelty.

The repo already has a .NET 9.0 solution with 4 projects (Domain, Application, Infrastructure, Presentation) scaffolded but empty (only placeholder Class1.cs files). Current branch is `setting-up-postgres`.

---

## Design Decisions

| Decision | Choice |
|---|---|
| Naming | `RelativeFormulas` (keep current) |
| Web framework | ASP.NET Core MVC (controllers + Razor views) — convert from scaffolded Razor Pages |
| Auth | ASP.NET Core Identity (`ApplicationUser : IdentityUser`) |
| Local database | PostgreSQL 16 via Docker Compose |
| CSS framework | PicoCSS via CDN (remove Bootstrap/jQuery) |
| JS framework | HTMX via CDN only (no SPA) |
| Ingredient quantities | Free-text string (e.g., "2 cups", "a pinch of") |
| Ingredient modeling | Normalized — separate `Ingredients` table with autocomplete for standardization across multiple contributors |
| Image storage | Store originals only. `IImageStorage` abstraction in Application, `LocalImageStorage` in Infrastructure. Defer thumbnails. |
| Architecture style | Lean Onion — services use DbContext directly, no Repository/UoW pattern |
| Implementation sequence | Vertical slices — each slice delivers a working feature |

---

## Architecture

```
RelativeFormulas.Domain
├── Entities/
│   ├── Recipe.cs
│   ├── Ingredient.cs
│   ├── RecipeIngredient.cs
│   ├── Tag.cs
│   ├── RecipeTag.cs
│   └── Favorite.cs

RelativeFormulas.Application
├── Services/
│   ├── RecipeService.cs
│   ├── IngredientService.cs
│   ├── TagService.cs
│   └── FavoriteService.cs
├── Interfaces/
│   └── IImageStorage.cs

RelativeFormulas.Infrastructure
├── Data/
│   ├── AppDbContext.cs
│   ├── Configurations/ (EF Core entity configs)
│   └── Migrations/
├── Identity/
│   └── ApplicationUser.cs (extends IdentityUser)
├── Storage/
│   └── LocalImageStorage.cs

RelativeFormulas.Presentation
├── Controllers/
│   ├── RecipesController.cs
│   ├── IngredientsController.cs
│   ├── TagsController.cs
│   ├── FavoritesController.cs
│   ├── AdminController.cs
│   └── AccountController.cs
├── Views/ (Razor views organized by controller)
├── wwwroot/
│   ├── css/site.css
│   └── uploads/ (local dev images)
```

**Layer rules:**
- Domain: entities with navigation properties, no dependencies
- Application: services injected with `AppDbContext` via DI, defines `IImageStorage`
- Infrastructure: `AppDbContext`, EF configurations, Identity, `LocalImageStorage`
- Presentation: thin controllers → services → views/partials

**Project references:**
- Application → Domain
- Infrastructure → Application (and transitively Domain)
- Presentation → Application
- Presentation → Infrastructure (for DI registration)

---

## Data Model

```
ApplicationUser (extends IdentityUser)
├── DisplayName (string)

Recipe
├── Id (int, PK)
├── Title (string, required)
├── Slug (string, unique)
├── InstructionsText (string, required)
├── TranscriptionText (string, required)
├── ImagePath (string?, nullable)
├── CreatedAt (DateTime)

Ingredient
├── Id (int, PK)
├── Name (string, required, unique)

RecipeIngredient
├── Id (int, PK)
├── RecipeId (FK → Recipe)
├── IngredientId (FK → Ingredient)
├── Quantity (string) — free-text, e.g., "2 cups"
├── SortOrder (int) — preserves order from original card

Tag
├── Id (int, PK)
├── Name (string, required, unique)

RecipeTag
├── RecipeId (FK → Recipe)
├── TagId (FK → Tag)
├── Composite PK: (RecipeId, TagId)

Favorite
├── Id (int, PK)
├── UserId (FK → ApplicationUser)
├── RecipeId (FK → Recipe)
├── CreatedAt (DateTime)
├── Unique index: (UserId, RecipeId)
```

---

## Development Setup

**docker-compose.yml** (repo root):
- PostgreSQL 16 container, port 5432
- Persistent volume for data
- Default dev credentials

**NuGet packages to add:**
- `Npgsql.EntityFrameworkCore.PostgreSQL` → Infrastructure
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` → Infrastructure

**CDN references in _Layout.cshtml:**
- PicoCSS
- HTMX

**Files to remove:**
- Bootstrap CSS/JS from wwwroot/lib/
- jQuery from wwwroot/lib/
- Razor Pages files (Pages/ directory, _ViewImports, _ViewStart)

**.gitignore additions:**
- `appsettings.Local.json`
- `.vs/`
- `*.user`
- `wwwroot/uploads/`

---

## HTMX Patterns

Controllers detect HTMX via `Request.Headers["HX-Request"]`:
- Full page loads → `return View(model)` (with layout)
- HTMX requests → `return PartialView("_PartialName", model)` (fragment only)

| Feature | Trigger | Response |
|---|---|---|
| Favorite/unfavorite | `hx-post` on button | Toggled heart partial |
| Ingredient autocomplete | `hx-get` on keyup with 300ms debounce | Matching ingredients list partial |
| Tag filtering | `hx-get` on tag click | Filtered recipe list partial |
| Search | `hx-get` on search input with debounce | Filtered recipe list partial |

---

## Implementation Sequence (Vertical Slices)

### Slice 1 — Foundation + Recipe Browsing
*Branch: `setting-up-postgres` (current)*

1. Add `docker-compose.yml` with PostgreSQL 16
2. Add `Npgsql.EntityFrameworkCore.PostgreSQL` to Infrastructure
3. Add Presentation → Infrastructure project reference
4. Create `Recipe` entity in Domain
5. Create `AppDbContext` in Infrastructure with Recipe DbSet
6. Add EF entity configuration for Recipe
7. Configure connection string in `appsettings.Local.json`
8. Register DbContext in `Program.cs`
9. Create initial migration
10. Seed 3-5 sample recipes via `DbInitializer`
11. Convert Presentation from Razor Pages to MVC:
    - Remove Pages/ directory
    - Add Controllers/ and Views/ directories
    - Update Program.cs for MVC routing
12. Replace Bootstrap/jQuery with PicoCSS + HTMX CDN links in `_Layout.cshtml`
13. Create `RecipesController` with `Index` and `Detail` actions
14. Create list and detail Razor views
15. Create `site.css` with custom overrides

**Deliverable:** Browsable recipe site with sample data

### Slice 2 — Ingredients
1. Create `Ingredient` and `RecipeIngredient` entities in Domain
2. EF configurations + migration
3. Update seed data with ingredients
4. Display ingredient list on recipe detail page

**Deliverable:** Recipes show their ingredient lists

### Slice 3 — Tags
1. Create `Tag` and `RecipeTag` entities in Domain
2. EF configurations + migration
3. Update seed data with tags
4. Display tags on recipe cards and detail page
5. Tag filtering via HTMX

**Deliverable:** Recipes browsable/filterable by tag

### Slice 4 — Auth + Favorites
1. Add `Microsoft.AspNetCore.Identity.EntityFrameworkCore` to Infrastructure
2. Create `ApplicationUser : IdentityUser` with `DisplayName`
3. Create `Favorite` entity
4. Update `AppDbContext` to extend `IdentityDbContext<ApplicationUser>`
5. Configure Identity in `Program.cs`
6. Migration for Identity tables + Favorite
7. Login/Register views
8. `FavoritesController` with HTMX toggle
9. User favorites page

**Deliverable:** Family members log in and save favorites

### Slice 5 — Admin CRUD + Images
1. Define `IImageStorage` in Application
2. Implement `LocalImageStorage` in Infrastructure
3. Admin-only recipe create/edit forms
4. Ingredient entry with HTMX autocomplete
5. Tag assignment on recipe form
6. Image upload endpoint
7. `[Authorize(Roles = "Admin")]` on admin actions

**Deliverable:** Admins add and manage recipes with images

### Slice 6 — Search + Polish
1. Recipe search endpoint with HTMX
2. Print-friendly view at `/recipes/{slug}/print`
3. `@media print` CSS
4. Mobile layout refinements
5. Error handling and validation

**Deliverable:** Complete, polished site

---

## Critical Files to Modify

| File | Change |
|---|---|
| `src/RelativeFormulas.Infrastructure/RelativeFormulas.Infrastructure.csproj` | Add Npgsql, Identity packages |
| `src/RelativeFormulas.Presentation/RelativeFormulas.Presentation.csproj` | Add reference to Infrastructure |
| `src/RelativeFormulas.Presentation/Program.cs` | MVC routing, DbContext registration, Identity config |
| `src/RelativeFormulas.Domain/Class1.cs` | Delete (replace with entity classes) |
| `src/RelativeFormulas.Application/Class1.cs` | Delete (replace with services) |
| `src/RelativeFormulas.Infrastructure/Class1.cs` | Delete (replace with DbContext) |

---

## Verification

**Per slice, verify:**
1. `docker compose up -d` starts PostgreSQL
2. `dotnet ef database update -p src/RelativeFormulas.Infrastructure -s src/RelativeFormulas.Presentation` applies migrations
3. `dotnet run --project src/RelativeFormulas.Presentation` starts the site
4. Browse to `https://localhost:5001` — pages render correctly
5. Seed data appears in the UI

**Slice 1 specifically:**
- Recipe list page shows seeded recipes
- Clicking a recipe shows the detail page with title, transcription, and instructions
- PicoCSS styling is applied (no Bootstrap artifacts)
- Database tables exist in PostgreSQL container

---

## First Step

Begin Slice 1 implementation on the `setting-up-postgres` branch.
