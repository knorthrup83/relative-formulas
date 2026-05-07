using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RelativeFormulas.Application.Services;
using RelativeFormulas.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("isAdmin", "True"));
});

builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<FavoriteService>();
builder.Services.AddScoped<AdminService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    DbInitializer.Seed(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "admin-ingredient-search",
    pattern: "admin/ingredients/search",
    defaults: new { controller = "Admin", action = "SearchIngredients" });

app.MapControllerRoute(
    name: "admin-ingredient-row",
    pattern: "admin/recipes/ingredient-row",
    defaults: new { controller = "Admin", action = "IngredientRow" });

app.MapControllerRoute(
    name: "admin-recipe-new",
    pattern: "admin/recipes/new",
    defaults: new { controller = "Admin", action = "Create" });

app.MapControllerRoute(
    name: "admin-recipe-edit",
    pattern: "admin/recipes/{id:int}/edit",
    defaults: new { controller = "Admin", action = "Edit" });

app.MapControllerRoute(
    name: "admin-recipe-delete",
    pattern: "admin/recipes/{id:int}/delete",
    defaults: new { controller = "Admin", action = "Delete" });

app.MapControllerRoute(
    name: "admin-recipes",
    pattern: "admin/recipes",
    defaults: new { controller = "Admin", action = "Index" });

app.MapControllerRoute(
    name: "recipe-search",
    pattern: "recipes/search",
    defaults: new { controller = "Recipes", action = "Search" });

app.MapControllerRoute(
    name: "tags-detail",
    pattern: "tags/{slug}",
    defaults: new { controller = "Tags", action = "Detail" });

app.MapControllerRoute(
    name: "tags",
    pattern: "tags",
    defaults: new { controller = "Tags", action = "Index" });

app.MapControllerRoute(
    name: "recipe-favorite",
    pattern: "recipes/{slug}/favorite",
    defaults: new { controller = "Recipes", action = "Favorite" });

app.MapControllerRoute(
    name: "recipe-unfavorite",
    pattern: "recipes/{slug}/unfavorite",
    defaults: new { controller = "Recipes", action = "Unfavorite" });

app.MapControllerRoute(
    name: "recipe-detail",
    pattern: "recipes/{slug}",
    defaults: new { controller = "Recipes", action = "Detail" });

app.MapControllerRoute(
    name: "register",
    pattern: "register",
    defaults: new { controller = "Account", action = "Register" });

app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "logout",
    pattern: "logout",
    defaults: new { controller = "Account", action = "Logout" });

app.MapControllerRoute(
    name: "favorites",
    pattern: "favorites",
    defaults: new { controller = "Favorites", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Recipes}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
