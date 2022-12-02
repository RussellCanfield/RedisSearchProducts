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
            CreateFilterMetadata(db);

            for (int i = 0; i < 10000; i++)
            {
                var product = ProductGenerator.Create();

                await CreateJson(db, product);
                await CreateFields(db, product);
                await CreateSuggestions(db, product);
                CreateFilters(db, product);

                if (Random.Shared.NextDouble() > 0.8)
                {
                    await CreateDefaultProductList(db, product);
                }
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

        private async Task CreateDefaultProductList(IDatabase database, Product product)
        {
            await database.SetAddAsync($"default:products", product.Id.ToString());
        }

        private async Task CreateJson(IDatabase database, Product product)
        {
            await database.StringSetAsync($"json:{product.Id}", JsonSerializer.Serialize(product));
        }

        private void CreateFilterMetadata(IDatabase database)
        {
            var batch = database.CreateBatch();

            var colorEnumValues = Enum.GetNames(typeof(Color));

            HashEntry[] colorValues = new HashEntry[colorEnumValues.Length];

            for (int i = 0; i < colorValues.Length; i++)
            {
                colorValues[i] = new HashEntry(colorEnumValues[i], 0);
            }

            batch.HashSetAsync("filters:meta:Color", colorValues);

            var sizeEnumValues = Enum.GetNames(typeof(Size));

            HashEntry[] sizeValues = new HashEntry[sizeEnumValues.Length];

            for (int i = 0; i < sizeValues.Length; i++)
            {
                sizeValues[i] = new HashEntry(sizeEnumValues[i], 0);
            }

            batch.HashSetAsync("filters:meta:Size", sizeValues);

            batch.SetAddAsync("filters:meta", "Color");
            batch.SetAddAsync("filters:meta", "Size");

            batch.Execute();
        }

        private void CreateFilters(IDatabase database, Product product)
        {
            var productId = product.Id.ToString();

            var batch = database.CreateBatch();
            batch.SetAddAsync($"filters:Color:{Enum.GetName(product.Color)}", productId);
            batch.SetAddAsync($"filters:Size:{Enum.GetName(product.Size)}", productId);
            batch.SortedSetAddAsync("ranges:Price", productId, (double)product.Price);

            batch.HashIncrementAsync("filters:meta:Color", Enum.GetName(product.Color), 1);
            batch.HashIncrementAsync("filters:meta:Size", Enum.GetName(product.Size), 1);

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
                var color = RandomColor;
                return new Product($"{RandomShirtName} {color} Shirt",
                    Random.Shared.NextInt64(5, 150),
                    color,
                    RandomSize);
            }
        }
    }
}
