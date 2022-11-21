using System;
using System.Text.Json;
using RediSearchClient;
using RediSearchClient.Indexes;
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
            var db = _redisService.Database;

            await PrepareDatabase(db);

            for (int i = 0; i < 10000; i++)
            {
                var product = ProductGenerator.Create();

                await CreateJson(db, product);
                await CreateFields(db, product);
                await CreateSuggestions(db, product);
                CreateFilters(db, product);
            }
        }

        private async Task PrepareDatabase(IDatabase database)
        {
            var server = _redisService.Server;
            await server.FlushDatabaseAsync();

            var indexDefinition = RediSearchIndex
                .OnHash()
                .ForKeysWithPrefix("fields:")
                .WithSchema(
                    x => x.Text("Name", sortable: false, nostem: true),
                    x => x.Text("Color", sortable: false, nostem: true),
                    x => x.Text("Size", sortable: false, nostem: true)
                )
                .Build();

            await database.CreateIndexAsync("product:search", indexDefinition);
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

        private async Task CreateFields(IDatabase database, Product product)
        {
            await database.HashSetAsync($"fields:{product.Id.ToString()}", new HashEntry[]
            {
                new HashEntry("Name", product.Name),
                new HashEntry("Color", Enum.GetName(typeof(Color), product.Color)),
                new HashEntry("Size", Enum.GetName(typeof(Size), product.Size))
            });
        }

        private async Task CreateSuggestions(IDatabase database, Product product)
        {
            await database.AddSuggestionAsync("product:suggestions", product.Name, 1);
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
