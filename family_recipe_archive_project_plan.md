# Family Recipe Archive – Full Project Plan

## 1. Project Overview

**Goal:**
Create a long-lived, family-only recipe website to preserve and share your grandmother’s handwritten recipes. Each recipe will include:
- A scanned image of the original recipe card
- A faithful transcription
- Structured ingredient data
- Tags for browsing
- User accounts so people can favorite recipes

**Guiding principles:**
- Longevity and maintainability over novelty
- Simple architecture, minimal moving parts
- Server-rendered HTML
- Clean domain modeling
- Printable, archival-friendly output

---

## 2. Technology Choices

### Backend
- **Language:** C#
- **Framework:** ASP.NET Core MVC
- **Architecture:** Onion Architecture
- **Authentication:** ASP.NET Core Identity (cookie-based)

### Frontend
- **UI approach:** Server-rendered HTML + HTMX
- **Styling:** Plain CSS (or minimal framework like PicoCSS / custom CSS)
- **JavaScript:** HTMX only (no SPA framework)

### Database
- **Database:** PostgreSQL (from day one)
- **Access:** Entity Framework Core
- **Migrations:** EF Core migrations checked into source control

### Hosting
- **Target:** Cloud hosting (low cost)
- **Examples:** Fly.io, Azure App Service, Render
- **Storage:** Cloud object storage for scanned images

---

## 3. High-Level Architecture

```
FamilyRecipes (Git Repo)
│
├─ src/
│  ├─ FamilyRecipes.Domain
│  ├─ FamilyRecipes.Application
│  ├─ FamilyRecipes.Infrastructure
│  └─ FamilyRecipes.Web
│
├─ tests/
│  ├─ FamilyRecipes.Domain.Tests
│  └─ FamilyRecipes.Application.Tests
│
└─ README.md
```

### Layer Responsibilities

**Domain**
- Entities: Recipe, Ingredient, RecipeIngredient, Tag, Favorite, User
- Value objects: Quantity, Unit
- Business rules only

**Application**
- Use cases (AddRecipe, FavoriteRecipe, SearchRecipes)
- Interfaces for persistence
- No ASP.NET dependencies

**Infrastructure**
- EF Core DbContext
- PostgreSQL mappings
- Identity implementation
- File/image storage

**Web**
- Controllers
- Razor views & partials
- HTMX endpoints
- CSS & static assets

---

## 4. Data Model (Initial)

### Core Tables

**Users**
- Id
- Email
- PasswordHash

**Recipes**
- Id
- Title
- Slug
- InstructionsText
- TranscriptionText
- ImageUrl
- CreatedAt

**Ingredients**
- Id
- Name

**RecipeIngredients**
- RecipeId
- IngredientId
- Quantity
- Unit

**Tags**
- Id
- Name

**RecipeTags**
- RecipeId
- TagId

**Favorites**
- UserId
- RecipeId

---

## 5. UI Approach (HTMX)

### Principles
- Full page loads for navigation
- HTMX for small interactions
- No client-side state management

### Typical HTMX Use Cases
- Favorite/unfavorite button
- Search-as-you-type
- Tag filtering
- Inline form validation

Responses are HTML partials, not JSON.

---

## 6. Printing Strategy

- Dedicated print endpoint per recipe: `/recipes/{slug}/print`
- Print-optimized Razor view
- Includes:
  - Recipe title
  - Ingredients
  - Instructions
  - Original scanned card image

- Print-specific CSS using `@media print`

---

## 7. Security & Accounts

- User registration & login
- Cookie-based auth
- Favorites tied to user account
- Admin-only access for:
  - Creating/editing recipes
  - Uploading images

---

## 8. Phased Work Plan

### Phase 0 – Planning & Setup
- Finalize requirements
- Create repo
- Create solution & projects
- Configure EF Core & PostgreSQL

---

### Phase 1 – Core Domain & Persistence
- Model domain entities
- Configure EF Core mappings
- Create initial migrations
- Seed development data

---

### Phase 2 – Read-Only Recipe Browsing
- Recipe list page
- Recipe detail page
- Display image + transcription
- Basic CSS layout

(No auth, no editing yet)

---

### Phase 3 – Accounts & Favorites
- Add ASP.NET Identity
- Registration/login pages
- Favorite/unfavorite via HTMX
- Favorites page per user

---

### Phase 4 – Admin Recipe Management
- Admin-only recipe creation
- Ingredient entry via form
- Tag assignment
- Image upload

---

### Phase 5 – Search, Tags, UX Polish
- Search endpoint (HTMX)
- Tag filtering
- Improved mobile layout
- Error handling

---

### Phase 6 – Print & Archival Features
- Print-friendly views
- Print CSS tuning
- Image resolution verification
- Final data backup strategy

---

## 9. Long-Term Maintenance Plan

- Single repo, single deployable
- Minimal dependencies
- Periodic DB backups
- Image storage backups
- Avoid frontend build tooling

---

## 10. Non-Goals (Intentionally Excluded)

- Public API
- SPA architecture
- Real-time collaboration
- Frequent content updates
- Social features

---

## 11. Success Criteria

- Recipes are preserved accurately
- Site is usable on mobile
- Recipes print cleanly
- System is understandable years later
- Minimal maintenance burden

---

## 12. Final Notes

This project is intentionally conservative in tech choices to maximize longevity. HTMX and server-rendered HTML keep the system understandable, testable, and archival-friendly.

The architecture leaves room to grow, but does not force complexity up front.

