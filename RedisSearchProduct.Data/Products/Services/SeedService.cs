using System;
using System.Text.Json;
using RedisSearchProduct.Data.Products.Models;
using RedisSearchProduct.Data.Redis;
using StackExchange.Redis;

namespace RedisSearchProduct.Data.Products.Services
{
    public interface ISeedService
    {
        Task SeedProducts();
    }

    public class SeedService : ISeedService
    {
        private readonly IRedisService _redisService;

        public SeedService(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task SeedProducts()
        {
            var server = _redisService.Server;
            await server.FlushDatabaseAsync();

            var db = _redisService.Database;

            for (int i = 0; i < 10000; i++)
            {
                var product = ProductGenerator.Create();

                await CreateJson(db, product);
                CreateFilters(db, product);
            }
        }

        private async Task CreateJson(IDatabase database, Product product)
        {
            await database.StringSetAsync($"json:{product.Id}", JsonSerializer.Serialize(product));
        }

        private void CreateFilters(IDatabase database, Product product)
        {
            var productId = product.Id.ToString();

            var batch = database.CreateBatch();
            batch.SetAddAsync($"filters:color:{Enum.GetName(product.Color)}", productId);
            batch.SetAddAsync($"filters:size:{Enum.GetName(product.Size)}", productId);
            batch.SortedSetAddAsync("ranges:price", productId, (double)product.Price);
            batch.Execute();
        }

        private static class ProductGenerator
        {
            private static readonly int NumOfColors;
            private static readonly int NumOfSizes;

            private static readonly string[] ShirtTypes = new[]
            {
            "Fuzzy",
            "Form-fitting",
            "Weird",
            "Cotton",
            "Awesome"
        };

            private static string RandomShirtName => ShirtTypes[Random.Shared.Next(0, ShirtTypes.Length - 1)];

            private static Color RandomColor =>
                (Color)Random.Shared.Next(0, NumOfColors);

            private static Size RandomSize =>
                (Size)Random.Shared.Next(0, NumOfSizes);

            static ProductGenerator()
            {
                NumOfColors = Enum.GetValues<Color>().Length - 1;
                NumOfSizes = Enum.GetValues<Size>().Length - 1;
            }

            public static Product Create()
            {
                return new Product(RandomShirtName,
                    Random.Shared.NextInt64(5, 150),
                    RandomColor,
                    RandomSize);
            }
        }
    }
}
