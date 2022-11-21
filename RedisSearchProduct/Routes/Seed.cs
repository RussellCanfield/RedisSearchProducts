using System;
using RedisSearchProduct.Data.Products.Services;

namespace RedisSearchProduct.Routes
{
    public static class Seed
    {
        public static WebApplication UseSeedRoutes(this WebApplication app)
        {
            app.MapPost("/seed", async (ISeedService seedService) =>
            {
                await seedService.SeedProducts();

                return Results.NoContent();
            })
            .WithName("SeedProducts")
            .WithDescription("Seeds product data")
            .WithOpenApi();

            return app;
        }
    }
}

