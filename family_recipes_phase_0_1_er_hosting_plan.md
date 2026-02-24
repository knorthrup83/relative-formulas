# Family Recipe Archive – Phase 0–1 Starter Commit, ER Diagram, Hosting & Cost Plan

---

## 1. Phase 0–1 Starter Commit Guide

### Purpose
To create the **initial project structure**, implement **domain entities**, set up **EF Core with PostgreSQL**, and seed some **development data**.

### Steps

#### 1. Create solution and projects
```bash
dotnet new sln -n FamilyRecipes
mkdir src
cd src

# Create projects
dotnet new classlib -n FamilyRecipes.Domain
dotnet new classlib -n FamilyRecipes.Application
dotnet new classlib -n FamilyRecipes.Infrastructure
dotnet new webapp -n FamilyRecipes.Web

# Add references
dotnet sln add src/FamilyRecipes.Domain/FamilyRecipes.Domain.csproj
dotnet sln add src/FamilyRecipes.Application/FamilyRecipes.Application.csproj
dotnet sln add src/FamilyRecipes.Infrastructure/FamilyRecipes.Infrastructure.csproj
dotnet sln add src/FamilyRecipes.Web/FamilyRecipes.Web.csproj

dotnet add src/FamilyRecipes.Application/FamilyRecipes.Application.csproj reference src/FamilyRecipes.Domain/FamilyRecipes.Domain.csproj

dotnet add src/FamilyRecipes.Infrastructure/FamilyRecipes.Infrastructure.csproj reference src/FamilyRecipes.Application/FamilyRecipes.Application.csproj

dotnet add src/FamilyRecipes.Web/FamilyRecipes.Web.csproj reference src/FamilyRecipes.Application/FamilyRecipes.Application.csproj
```

#### 2. Configure PostgreSQL with EF Core
- Add packages:
```bash
dotnet add src/FamilyRecipes.Infrastructure/FamilyRecipes.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL
```
- Create `AppDbContext` with DbSets for `Recipe`, `Ingredient`, `RecipeIngredient`, `Tag`, `RecipeTag`, `User`, `Favorite`.
- Configure PostgreSQL connection string in `appsettings.Development.json`.
- Add initial migration and update database:
```bash
dotnet ef migrations add InitialCreate -p src/FamilyRecipes.Infrastructure -s src/FamilyRecipes.Web
dotnet ef database update -p src/FamilyRecipes.Infrastructure -s src/FamilyRecipes.Web
```

#### 3. Seed Development Data
- Create a `DbInitializer` in `Infrastructure` to insert a few sample recipes, ingredients, and users.
- Ensure you can see data in the database locally.

#### 4. Initial Git Commit
```bash
git init
git add .
git commit -m "Phase 0-1: initial solution, projects, EF Core setup, dev seed"
```
- Optionally tag as `v0.1-phase0-1`.

---

## 2. ER Diagram for Data Model

```text
+-----------------+        +-------------------+
|     Users       |        |      Recipes      |
|-----------------|        |------------------|
| Id (PK)         |<-----+ | Id (PK)          |
| Email           |       | | Title            |
| PasswordHash    |       | | Slug             |
| DisplayName     |       | | InstructionsText |
+-----------------+       | | TranscriptionText|
                          | | ImageUrl         |
                          | +------------------+
                          |
                          |      +----------------+
                          |      | RecipeIngredients |
                          |      |------------------|
                          +----> | RecipeId (FK)    |
                                 | IngredientId(FK) |
                                 | Quantity         |
                                 | Unit             |
                                 +------------------+
+-----------------+        +----------------+
|  Ingredients    |        |   Tags         |
|-----------------|        |----------------|
| Id (PK)         |<----+  | Id (PK)       |
| Name            |     |  | Name           |
+-----------------+     |  +----------------+
                        |
                        |   +----------------+
                        |   |  RecipeTags    |
                        |   |----------------|
                        +-->| RecipeId (FK)  |
                            | TagId (FK)     |
                            +----------------+
+-----------------+
|   Favorites     |
|-----------------|
| UserId (FK)     |
| RecipeId (FK)   |
| CreatedAt       |
+-----------------+
```

**Notes:**
- `RecipeIngredients` and `RecipeTags` are **many-to-many join tables**.
- `Favorites` is a **many-to-many between Users and Recipes**.
- All FK relationships enforce data integrity.

---

## 3. Hosting + Cost Breakdown

### Hosting Requirements
- Single web app
- Minimal traffic (family only)
- PostgreSQL database
- Storage for scanned images (~200 recipes × ~1MB each = ~200MB)

### Recommended Cloud Hosting Options

| Provider        | Service                         | Notes/Pros                                      | Est. Cost (low traffic) |
|-----------------|---------------------------------|------------------------------------------------|-------------------------|
| Fly.io          | App + PostgreSQL                | Simple deployment from git, free tier for small apps | $0–$10/month            |
| Render          | Web Service + Managed Postgres  | Easy PostgreSQL setup, deploy from GitHub     | ~$7–$15/month           |
| Azure App Service | Web App + Azure Postgres        | Seamless for C#, managed DB                   | ~$20/month               |
| Railway         | Web + DB                        | Quick deploy, free tier                        | $0–$5/month              |

### Storage
- Images stored in the cloud bucket (S3, Azure Blob, or built-in service)
- ~1GB storage more than sufficient
- Cost negligible on small scale (~$0–$5/month)

### Traffic & Bandwidth
- Expect < 200 users
- Low bandwidth
- Even free tiers usually suffice

### Recommendation for MVP
- **Render Web Service + Managed Postgres**
- Store images in **Render persistent storage or S3**
- Cost: ~$10/month
- Single deployment, easy to scale later if needed

---

### Summary
- **Phase 0–1 starter commit:** solution + projects + EF Core + dev seed
- **ER Diagram:** shows all entities, many-to-many relationships, and favorites
- **Hosting:** low-cost cloud provider, managed PostgreSQL, minimal storage needs

This setup gives a **fully working local dev environment** and a path to a **cheap, durable production deployment**.

