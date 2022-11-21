using System.Text.Json;
using RedisSearchProduct.Data.Redis;
using RedisSearchProduct.Data.Products.Models;
using RediSearchClient;

namespace RedisSearchProduct.Data.Products.Services
{
    public interface IProductService
    {
        Task<Product?> GetProduct(string Id);
        Task<string[]?> GetProductSuggestions(string searchTerm);
    }

    public class ProductService : IProductService
    {
        private readonly IRedisService _redisService;

        public ProductService(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task<Product?> GetProduct(string Id)
        {
            var db = _redisService.Database;

            var productJson = await db.StringGetAsync($"json:{Id}");

            if (!productJson.HasValue) return null;

            return JsonSerializer.Deserialize<Product>(productJson!);
        }

        public async Task<string[]?> GetProductSuggestions(string searchTerm)
        {
            var db = _redisService.Database;

            var suggestions = await db.GetSuggestionsAsync("product:suggestions", searchTerm);

            if (suggestions == null) return Array.Empty<string>();

            return suggestions.Select(s => s.Suggestion).ToArray();
        }
    }
}