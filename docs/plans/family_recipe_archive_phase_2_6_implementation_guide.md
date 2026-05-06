# Family Recipe Archive – Phase 2–6 Implementation Guide

This document continues after the Phase 0–1 starter guide. It assumes the solution, projects, PostgreSQL database, EF Core migrations, and initial seed data are already in place.

---

# Phase 2 – Read-Only Recipe Browsing

## Goal
Create the first usable version of the public site where family members can browse and view recipes.

At the end of this phase, the site should support:
- Recipe list page
- Recipe detail page
- Displaying ingredients
- Displaying instructions/transcription
- Displaying scanned recipe card images
- Basic responsive layout
- Basic Pico/custom CSS styling

---

## 2.1 Application Layer Use Cases

Create read-focused use cases in the Application project.

Suggested structure:

```text
FamilyRecipes.Application/
├─ Recipes/
│  ├─ GetRecipeList/
│  │  ├─ GetRecipeListQuery.cs
│  │  ├─ GetRecipeListResult.cs
│  │  └─ GetRecipeListHandler.cs
│  │
│  └─ GetRecipeDetail/
│     ├─ GetRecipeDetailQuery.cs
│     ├─ GetRecipeDetailResult.cs
│     └─ GetRecipeDetailHandler.cs
```

### GetRecipeList
Returns summary information for recipe cards:
- Id
- Title
- Slug
- Short description or excerpt
- Primary image URL
- Tags

### GetRecipeDetail
Returns full display information:
- Title
- Slug
- Ingredients
- Instructions
- Transcription
- Notes
- Images
- Tags

---

## 2.2 Repository Methods

Add methods to the recipe repository interface:

```csharp
Task<IReadOnlyList<RecipeSummaryDto>> GetRecipeSummariesAsync(CancellationToken cancellationToken);
Task<RecipeDetailDto?> GetRecipeBySlugAsync(string slug, CancellationToken cancellationToken);
```

Implementation lives in Infrastructure.

Use EF Core projection rather than loading entire aggregates when displaying read-only pages.

---

## 2.3 Web Routes

Create public recipe routes:

```text
GET /recipes
GET /recipes/{slug}
```

Suggested controller:

```text
FamilyRecipes.Presentation/Controllers/RecipesController.cs
```

Actions:

```csharp
public async Task<IActionResult> Index(CancellationToken cancellationToken)
public async Task<IActionResult> Details(string slug, CancellationToken cancellationToken)
```

---

## 2.4 Views

Create views:

```text
Views/Recipes/Index.cshtml
Views/Recipes/Details.cshtml
Views/Recipes/_RecipeCard.cshtml
Views/Shared/_TagList.cshtml
```

### Recipe List Page
Use semantic HTML and Pico-friendly markup:

```html
<main class="container">
  <header>
    <h1>Family Recipes</h1>
    <p>Preserved recipes from our family collection.</p>
  </header>

  <section class="recipe-grid">
    <!-- recipe cards -->
  </section>
</main>
```

### Recipe Detail Page
Suggested sections:
- Title and tags
- Favorite placeholder, added in Phase 3
- Ingredients
- Instructions
- Transcription
- Original scanned card image

---

## 2.5 Styling

Keep Pico as the base and add custom classes:

```text
wwwroot/css/site.css
```

Suggested classes:

```css
.recipe-grid { }
.recipe-card { }
.recipe-layout { }
.recipe-image { }
.tag-list { }
.tag { }
```

Do not rely heavily on Pico’s `.grid` class for important layouts. Prefer app-specific layout classes.

---

## 2.6 Acceptance Criteria

Phase 2 is complete when:
- `/recipes` shows a list of recipes
- Each recipe links to `/recipes/{slug}`
- Recipe detail pages show ingredients, instructions, transcription, and image
- Pages are readable on desktop and mobile
- No login is required
- No editing is required yet

---

# Phase 3 – Accounts & Favorites

## Goal
Allow family members to create accounts, log in, and save favorite recipes.

At the end of this phase, the site should support:
- Registration
- Login/logout
- Cookie-based authentication
- Favorite/unfavorite recipe via HTMX
- Favorites page per user

---

## 3.1 Authentication Choice

Use ASP.NET Core Identity if it is already in the generated codebase. If not, this phase can use a simpler custom user table and password hasher, but Identity is the safest standard option.

Recommended for this project:
- Cookie auth
- Local email/password accounts
- No OAuth for MVP
- No password reset for MVP unless desired

---

## 3.2 Data Model Additions

Ensure these exist:

```text
Users
- Id
- Email
- PasswordHash
- DisplayName
- CreatedAt
- IsAdmin

Favorites
- UserId
- RecipeId
- CreatedAt
```

Add unique constraint:

```text
Favorites: unique(UserId, RecipeId)
```

This prevents duplicate favorites.

---

## 3.3 Application Use Cases

Suggested structure:

```text
FamilyRecipes.Application/
├─ Accounts/
│  ├─ RegisterUser/
│  ├─ LoginUser/
│  └─ GetCurrentUser/
│
└─ Favorites/
   ├─ FavoriteRecipe/
   ├─ UnfavoriteRecipe/
   ├─ ToggleFavoriteRecipe/
   ├─ GetUserFavorites/
   └─ IsRecipeFavorited/
```

### FavoriteRecipe
Inputs:
- UserId
- RecipeId

Behavior:
- Verify recipe exists
- Add favorite if not already present

### UnfavoriteRecipe
Inputs:
- UserId
- RecipeId

Behavior:
- Remove favorite if it exists

### GetUserFavorites
Returns recipe summaries favorited by the current user.

---

## 3.4 Web Routes

Authentication routes:

```text
GET  /register
POST /register
GET  /login
POST /login
POST /logout
```

Favorites routes:

```text
POST /recipes/{id}/favorite
POST /recipes/{id}/unfavorite
GET  /favorites
```

---

## 3.5 HTMX Favorite Button

On the recipe detail page:

```html
<div id="favorite-button">
  <!-- render _FavoriteButton.cshtml -->
</div>
```

Partial view:

```text
Views/Recipes/_FavoriteButton.cshtml
```

Favorited state:

```html
<button
  hx-post="/recipes/123/unfavorite"
  hx-target="#favorite-button"
  hx-swap="outerHTML">
  ♥ Favorited
</button>
```

Not favorited state:

```html
<button
  hx-post="/recipes/123/favorite"
  hx-target="#favorite-button"
  hx-swap="outerHTML">
  ♡ Favorite
</button>
```

If the user is not logged in, either:
- show a login link, or
- redirect HTMX requests to `/login` using `HX-Redirect`.

---

## 3.6 Favorites Page

Route:

```text
GET /favorites
```

Displays the same recipe card partials used by the recipe list page.

If the user has no favorites, show a friendly empty state:

```text
You have not saved any recipes yet.
```

---

## 3.7 Acceptance Criteria

Phase 3 is complete when:
- Users can register
- Users can log in and log out
- Logged-in users can favorite a recipe
- Favorite button updates without full-page reload
- Users can view `/favorites`
- Anonymous users cannot favorite recipes

---

# Phase 4 – Admin Recipe Management

## Goal
Allow an admin user to create and edit recipes using web forms.

At the end of this phase, the site should support:
- Admin-only recipe list
- Create recipe
- Edit recipe
- Add ingredients
- Assign tags
- Upload or attach scanned images

---

## 4.1 Authorization

Add an admin role or `IsAdmin` property.

Admin-only routes:

```text
/admin/recipes
/admin/recipes/new
/admin/recipes/{id}/edit
/admin/ingredients
/admin/tags
```

Protect these routes using authorization attributes or route conventions.

---

## 4.2 Application Use Cases

Suggested structure:

```text
FamilyRecipes.Application/
└─ Admin/
   ├─ Recipes/
   │  ├─ CreateRecipe/
   │  ├─ UpdateRecipe/
   │  ├─ DeleteRecipe/
   │  └─ GetRecipeForEdit/
   │
   ├─ Ingredients/
   │  ├─ SearchIngredients/
   │  ├─ CreateIngredient/
   │  └─ RenameIngredient/
   │
   └─ Tags/
      ├─ CreateTag/
      ├─ RenameTag/
      └─ DeleteTag/
```

---

## 4.3 Admin Recipe Form Fields

Recipe fields:
- Title
- Slug
- Description or notes
- Instructions text
- Transcription text
- Tags
- Ingredients
- Images

Ingredient row fields:
- Quantity
- Unit
- Ingredient name
- Preparation note
- Sort order

---

## 4.4 HTMX Admin Enhancements

Use HTMX for small form improvements:

### Add Ingredient Row

```html
<button
  type="button"
  hx-get="/admin/recipes/ingredient-row"
  hx-target="#ingredient-rows"
  hx-swap="beforeend">
  Add ingredient
</button>
```

### Ingredient Autocomplete

```html
<input
  name="ingredientName"
  hx-get="/admin/ingredients/search"
  hx-trigger="keyup changed delay:250ms"
  hx-target="#ingredient-suggestions" />
```

### Save Feedback

Recipe forms can either:
- use normal POST/redirect, or
- use HTMX for inline validation and save status.

For MVP, normal POST/redirect is simpler.

---

## 4.5 Image Handling

Start simple.

MVP image options:
1. Upload image through admin form
2. Store file locally in `wwwroot/uploads` during local development
3. Store URL in database

Later production options:
- S3
- Cloudflare R2
- Azure Blob Storage
- Backblaze B2

Recommended database representation:

```text
RecipeImages
- Id
- RecipeId
- Url
- Caption
- SortOrder
- CreatedAt
```

Prefer this over a single `ImageUrl` on `Recipes` if recipes may have front/back cards.

---

## 4.6 Acceptance Criteria

Phase 4 is complete when:
- Admin users can create recipes
- Admin users can edit recipes
- Ingredients can be added to recipes
- Tags can be assigned
- Images can be uploaded or attached
- Non-admin users cannot access admin pages

---

# Phase 5 – Search, Tags, UX Polish

## Goal
Improve discoverability and make the site pleasant to use.

At the end of this phase, the site should support:
- Search
- Tag filtering
- Improved empty states
- Better mobile layout
- Error handling
- Basic visual polish

---

## 5.1 Search Scope

MVP search should include:
- Recipe title
- Instructions
- Transcription
- Ingredient names
- Tags

Start with simple database queries. PostgreSQL full-text search can be added later if needed.

---

## 5.2 Application Use Case

```text
FamilyRecipes.Application/
└─ Recipes/
   └─ SearchRecipes/
      ├─ SearchRecipesQuery.cs
      ├─ SearchRecipesResult.cs
      └─ SearchRecipesHandler.cs
```

Inputs:
- Search query
- Optional tag
- Optional page number

Outputs:
- Matching recipe summaries
- Count
- Current filter info

---

## 5.3 Search Route

```text
GET /recipes/search?q=banana
```

If it is a normal request, return a full page.

If it is an HTMX request, return only the recipe list partial.

---

## 5.4 Search UI

```html
<input
  type="search"
  name="q"
  placeholder="Search recipes"
  hx-get="/recipes/search"
  hx-trigger="keyup changed delay:300ms"
  hx-target="#recipe-results"
  hx-push-url="true" />

<div id="recipe-results">
  <!-- recipe cards -->
</div>
```

Use `hx-push-url="true"` if you want the browser URL to reflect the search.

---

## 5.5 Tags

Routes:

```text
GET /tags
GET /tags/{slug}
```

Each tag page displays recipe cards using the same recipe list partial.

---

## 5.6 UX Polish Checklist

Add:
- Empty state for no recipes
- Empty state for no search results
- Empty state for no favorites
- Loading indicator for HTMX search
- Friendly error messages
- Consistent page titles
- Consistent tag badges
- Better recipe card images
- Mobile-first layout improvements

---

## 5.7 Styling Direction

Continue with Pico + custom CSS.

Suggested visual direction:
- Warm paper background
- Soft card surfaces
- Brown/gold accent color
- Rounded recipe cards
- Legible typography
- Subtle image borders

Avoid heavy design systems or complicated frontend build tools.

---

## 5.8 Acceptance Criteria

Phase 5 is complete when:
- Recipes can be searched
- Recipes can be filtered by tag
- Search works without a full-page reload using HTMX
- Important empty states are handled
- The site looks intentional rather than plain
- Mobile layout is acceptable

---

# Phase 6 – Print & Archival Features

## Goal
Make the site suitable for long-term preservation and practical printing.

At the end of this phase, the site should support:
- Print-friendly recipe pages
- Printing both text and scanned recipe card images
- Backup strategy
- Production readiness checklist

---

## 6.1 Print Routes

Add:

```text
GET /recipes/{slug}/print
```

This route should return a simplified print-specific view.

Suggested view:

```text
Views/Recipes/Print.cshtml
```

---

## 6.2 Print View Content

The print view should include:
- Recipe title
- Ingredients
- Instructions
- Transcription
- Original scanned card image or images
- Optional notes

It should exclude:
- Navigation
- Favorite buttons
- Login links
- Admin controls
- Search/filter UI

---

## 6.3 Print CSS

Create:

```text
wwwroot/css/print.css
```

Reference it in layout or print view:

```html
<link rel="stylesheet" href="/css/print.css" media="print" />
```

Suggested rules:

```css
@media print {
  nav,
  footer,
  .no-print,
  button {
    display: none !important;
  }

  body {
    background: white !important;
    color: black !important;
    font-size: 12pt;
  }

  article {
    box-shadow: none !important;
    border: none !important;
  }

  img {
    max-width: 100%;
    break-inside: avoid;
    page-break-inside: avoid;
  }

  h1,
  h2,
  h3 {
    break-after: avoid;
    page-break-after: avoid;
  }
}
```

---

## 6.4 Image Resolution Guidance

For scanned recipe cards:
- Prefer 300 DPI or higher
- Save archival originals separately if possible
- Use web-optimized copies for display
- Keep filenames stable and descriptive

Suggested naming:

```text
banana-bread-front.jpg
banana-bread-back.jpg
apple-pie-card-01.jpg
```

---

## 6.5 Backups

Backup categories:

### Database
Contains:
- Recipes
- Ingredients
- Tags
- Users
- Favorites
- Image metadata

Backup using:

```bash
pg_dump
```

### Images
Contains:
- Scanned recipe card files

Backup by:
- Cloud bucket backup
- Local external drive copy
- Periodic ZIP archive

### Source Code
Contains:
- App code
- migrations
- documentation

Backup by:
- GitHub or other remote git host

---

## 6.6 Production Readiness Checklist

Before deploying:
- Set production connection string securely
- Do not commit secrets
- Enable HTTPS
- Configure cookie security
- Configure database backups
- Configure image backup/storage
- Create admin account
- Test registration/login
- Test favorites
- Test recipe creation/editing
- Test search
- Test printing
- Test mobile layout

---

## 6.7 Acceptance Criteria

Phase 6 is complete when:
- Each recipe has a print page
- Printed recipes include text and original card image
- Print output is clean and readable
- Backup process is documented
- Production deployment checklist is complete

---

# Suggested Final Milestones

## MVP Complete
The project can be considered MVP-complete when:
- Recipes can be browsed
- Recipe details display properly
- Users can register/login
- Users can favorite recipes
- Admin can add/edit recipes
- Search works
- Recipes can be printed

## Archive Ready
The project can be considered archive-ready when:
- All recipe cards are scanned
- All recipe transcriptions are complete
- All recipes have ingredients structured
- Tags are assigned consistently
- Backups are verified
- Print pages have been tested

---

# Notes for Future Enhancements

Do not build these until the MVP is complete:
- Unit conversion
- Recipe scaling
- OCR-assisted transcription
- Family comments
- “Made this” notes
- Recipe collections
- PDF export
- Advanced image editing

These are good ideas, but not required for the first durable version.

