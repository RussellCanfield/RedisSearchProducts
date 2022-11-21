using System.Text.Json;
using RedisSearchProduct.Data.Redis;
using RedisSearchProduct.Data.Products.Models;

namespace RedisSearchProduct.Data.Products.Services
{
    public interface IProductService
    {
        Task<Product?> GetProduct(string Id);
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
    }
}