using RelativeFormulas.Domain.Entities;

namespace RelativeFormulas.Infrastructure.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        SeedRecipes(context);
        SeedTagsAndIngredients(context);
    }

    private static void SeedRecipes(AppDbContext context)
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

    private static void SeedTagsAndIngredients(AppDbContext context)
    {
        if (context.Tags.Any())
            return;

        // Tags
        var dessert = new Tag { Name = "Dessert", Slug = "dessert" };
        var bakedGood = new Tag { Name = "Baked Good", Slug = "baked-good" };
        var savory = new Tag { Name = "Savory", Slug = "savory" };
        var familyFavorite = new Tag { Name = "Family Favorite", Slug = "family-favorite" };

        context.Tags.AddRange(dessert, bakedGood, savory, familyFavorite);
        context.SaveChanges();

        // Shared ingredients
        var flour = new Ingredient { Name = "All-purpose flour" };
        var sugar = new Ingredient { Name = "Sugar" };
        var butter = new Ingredient { Name = "Butter" };
        var salt = new Ingredient { Name = "Salt" };
        var eggs = new Ingredient { Name = "Eggs" };
        var vanilla = new Ingredient { Name = "Vanilla extract" };
        var bakingSoda = new Ingredient { Name = "Baking soda" };
        var bakingPowder = new Ingredient { Name = "Baking powder" };
        var brownSugar = new Ingredient { Name = "Brown sugar" };
        var chocolateChips = new Ingredient { Name = "Chocolate chips" };
        var bananas = new Ingredient { Name = "Ripe bananas" };
        var cornmeal = new Ingredient { Name = "Cornmeal" };
        var milk = new Ingredient { Name = "Milk" };
        var vegetableOil = new Ingredient { Name = "Vegetable oil" };
        var groundBeef = new Ingredient { Name = "Ground beef" };
        var onion = new Ingredient { Name = "Onion" };
        var garlic = new Ingredient { Name = "Garlic" };
        var kidneyBeans = new Ingredient { Name = "Kidney beans" };
        var crushedTomatoes = new Ingredient { Name = "Crushed tomatoes" };
        var chiliPowder = new Ingredient { Name = "Chili powder" };
        var cumin = new Ingredient { Name = "Cumin" };
        var blackPepper = new Ingredient { Name = "Black pepper" };
        var powderedSugar = new Ingredient { Name = "Powdered sugar" };
        var lemonJuice = new Ingredient { Name = "Lemon juice" };

        context.Ingredients.AddRange(
            flour, sugar, butter, salt, eggs, vanilla, bakingSoda, bakingPowder,
            brownSugar, chocolateChips, bananas, cornmeal, milk, vegetableOil,
            groundBeef, onion, garlic, kidneyBeans, crushedTomatoes,
            chiliPowder, cumin, blackPepper, powderedSugar, lemonJuice
        );
        context.SaveChanges();

        // Fetch recipes by slug
        var cookiesId = context.Recipes.Single(r => r.Slug == "grandmas-chocolate-chip-cookies").Id;
        var bananaBreakId = context.Recipes.Single(r => r.Slug == "moms-banana-bread").Id;
        var cornbreadId = context.Recipes.Single(r => r.Slug == "aunt-bettys-cornbread").Id;
        var chiliId = context.Recipes.Single(r => r.Slug == "grandpas-chili").Id;
        var lemonSquaresId = context.Recipes.Single(r => r.Slug == "nanas-lemon-squares").Id;

        // RecipeTags
        context.RecipeTags.AddRange(
            new RecipeTag { RecipeId = cookiesId, TagId = dessert.Id },
            new RecipeTag { RecipeId = cookiesId, TagId = bakedGood.Id },
            new RecipeTag { RecipeId = cookiesId, TagId = familyFavorite.Id },
            new RecipeTag { RecipeId = bananaBreakId, TagId = bakedGood.Id },
            new RecipeTag { RecipeId = bananaBreakId, TagId = familyFavorite.Id },
            new RecipeTag { RecipeId = cornbreadId, TagId = bakedGood.Id },
            new RecipeTag { RecipeId = cornbreadId, TagId = savory.Id },
            new RecipeTag { RecipeId = chiliId, TagId = savory.Id },
            new RecipeTag { RecipeId = chiliId, TagId = familyFavorite.Id },
            new RecipeTag { RecipeId = lemonSquaresId, TagId = dessert.Id },
            new RecipeTag { RecipeId = lemonSquaresId, TagId = bakedGood.Id }
        );

        // RecipeIngredients — Chocolate Chip Cookies
        context.RecipeIngredients.AddRange(
            new RecipeIngredient { RecipeId = cookiesId, IngredientId = flour.Id, Quantity = "2 1/4", Unit = "cups" },
            new RecipeIngredient { RecipeId = cookiesId, IngredientId = bakingSoda.Id, Quantity = "1", Unit = "tsp" },
            new RecipeIngredient { RecipeId = cookiesId, IngredientId = salt.Id, Quantity = "1", Unit = "tsp" },
            new RecipeIngredient { RecipeId = cookiesId, IngredientId = butter.Id, Quantity = "1", Unit = "cup" },
            new RecipeIngredient { RecipeId = cookiesId, IngredientId = sugar.Id, Quantity = "3/4", Unit = "cup" },
            new RecipeIngredient { RecipeId = cookiesId, IngredientId = brownSugar.Id, Quantity = "3/4", Unit = "cup" },
            new RecipeIngredient { RecipeId = cookiesId, IngredientId = eggs.Id, Quantity = "2", Unit = "" },
            new RecipeIngredient { RecipeId = cookiesId, IngredientId = vanilla.Id, Quantity = "1", Unit = "tsp" },
            new RecipeIngredient { RecipeId = cookiesId, IngredientId = chocolateChips.Id, Quantity = "2", Unit = "cups" }
        );

        // RecipeIngredients — Banana Bread
        context.RecipeIngredients.AddRange(
            new RecipeIngredient { RecipeId = bananaBreakId, IngredientId = bananas.Id, Quantity = "3", Unit = "" },
            new RecipeIngredient { RecipeId = bananaBreakId, IngredientId = butter.Id, Quantity = "1/3", Unit = "cup" },
            new RecipeIngredient { RecipeId = bananaBreakId, IngredientId = sugar.Id, Quantity = "3/4", Unit = "cup" },
            new RecipeIngredient { RecipeId = bananaBreakId, IngredientId = eggs.Id, Quantity = "1", Unit = "" },
            new RecipeIngredient { RecipeId = bananaBreakId, IngredientId = vanilla.Id, Quantity = "1", Unit = "tsp" },
            new RecipeIngredient { RecipeId = bananaBreakId, IngredientId = bakingSoda.Id, Quantity = "1", Unit = "tsp" },
            new RecipeIngredient { RecipeId = bananaBreakId, IngredientId = salt.Id, Quantity = "1", Unit = "pinch" },
            new RecipeIngredient { RecipeId = bananaBreakId, IngredientId = flour.Id, Quantity = "1 1/3", Unit = "cups" }
        );

        // RecipeIngredients — Cornbread
        context.RecipeIngredients.AddRange(
            new RecipeIngredient { RecipeId = cornbreadId, IngredientId = cornmeal.Id, Quantity = "1", Unit = "cup" },
            new RecipeIngredient { RecipeId = cornbreadId, IngredientId = flour.Id, Quantity = "1", Unit = "cup" },
            new RecipeIngredient { RecipeId = cornbreadId, IngredientId = sugar.Id, Quantity = "1/4", Unit = "cup" },
            new RecipeIngredient { RecipeId = cornbreadId, IngredientId = bakingPowder.Id, Quantity = "1", Unit = "tbsp" },
            new RecipeIngredient { RecipeId = cornbreadId, IngredientId = salt.Id, Quantity = "1/2", Unit = "tsp" },
            new RecipeIngredient { RecipeId = cornbreadId, IngredientId = milk.Id, Quantity = "1", Unit = "cup" },
            new RecipeIngredient { RecipeId = cornbreadId, IngredientId = vegetableOil.Id, Quantity = "1/3", Unit = "cup" },
            new RecipeIngredient { RecipeId = cornbreadId, IngredientId = eggs.Id, Quantity = "1", Unit = "" }
        );

        // RecipeIngredients — Grandpa's Chili
        context.RecipeIngredients.AddRange(
            new RecipeIngredient { RecipeId = chiliId, IngredientId = groundBeef.Id, Quantity = "2", Unit = "lbs" },
            new RecipeIngredient { RecipeId = chiliId, IngredientId = onion.Id, Quantity = "1", Unit = "large" },
            new RecipeIngredient { RecipeId = chiliId, IngredientId = garlic.Id, Quantity = "3", Unit = "cloves" },
            new RecipeIngredient { RecipeId = chiliId, IngredientId = kidneyBeans.Id, Quantity = "2", Unit = "cans" },
            new RecipeIngredient { RecipeId = chiliId, IngredientId = crushedTomatoes.Id, Quantity = "1", Unit = "can" },
            new RecipeIngredient { RecipeId = chiliId, IngredientId = chiliPowder.Id, Quantity = "2", Unit = "tbsp" },
            new RecipeIngredient { RecipeId = chiliId, IngredientId = cumin.Id, Quantity = "1", Unit = "tsp" },
            new RecipeIngredient { RecipeId = chiliId, IngredientId = salt.Id, Quantity = "1", Unit = "tsp" },
            new RecipeIngredient { RecipeId = chiliId, IngredientId = blackPepper.Id, Quantity = "1", Unit = "tsp" }
        );

        // RecipeIngredients — Nana's Lemon Squares
        context.RecipeIngredients.AddRange(
            new RecipeIngredient { RecipeId = lemonSquaresId, IngredientId = butter.Id, Quantity = "1", Unit = "cup" },
            new RecipeIngredient { RecipeId = lemonSquaresId, IngredientId = powderedSugar.Id, Quantity = "1/2", Unit = "cup" },
            new RecipeIngredient { RecipeId = lemonSquaresId, IngredientId = flour.Id, Quantity = "2 1/4", Unit = "cups" },
            new RecipeIngredient { RecipeId = lemonSquaresId, IngredientId = eggs.Id, Quantity = "4", Unit = "" },
            new RecipeIngredient { RecipeId = lemonSquaresId, IngredientId = sugar.Id, Quantity = "2", Unit = "cups" },
            new RecipeIngredient { RecipeId = lemonSquaresId, IngredientId = lemonJuice.Id, Quantity = "1/3", Unit = "cup" }
        );

        context.SaveChanges();
    }
}
