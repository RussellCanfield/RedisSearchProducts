using System;
using Microsoft.AspNetCore.Mvc;
using RedisSearchProduct.Contracts;
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
            .WithTags("Products")
            .WithName("GetProductById")
            .WithOpenApi();

            app.MapGet("/product/suggestion", async ([FromQuery] string searchTerm, IProductService productService) =>
            {
                var suggestions = await productService.GetProductSuggestions(searchTerm);

                return Results.Ok(suggestions);
            })
            .WithTags("Products")
            .WithName("GetProductSuggestion")
            .WithOpenApi();

            app.MapPost("/product/search", async ([FromBody] SearchRequestDto searchRequest, ISearchService searchService) =>
            {
                var products = await searchService.SearchProducts(searchRequest);

                return Results.Ok(products);
            })
            .WithTags("Products")
            .WithName("SearchProducts")
            .WithOpenApi();

            return app;
        }
    }
}

