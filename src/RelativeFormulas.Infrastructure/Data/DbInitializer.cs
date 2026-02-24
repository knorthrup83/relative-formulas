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
