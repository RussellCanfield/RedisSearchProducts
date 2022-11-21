using System;
using RedisSearchProduct.Data.Products.Services;

namespace RedisSearchProduct.Routes
{
    public static class Product
    {
        public static WebApplication UseProductRoutes(this WebApplication app)
        {
            app.MapGet("/product/{id}", async (string Id, IProductService productService) =>
            {
                var product = await productService.GetProduct(Id);

                if (product == null) return Results.NotFound();

                return Results.Ok(product);
            })
            .WithName("Products")
            .WithOpenApi();

            return app;
        }
    }
}

