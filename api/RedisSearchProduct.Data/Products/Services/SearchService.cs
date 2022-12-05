using System;
using RedisSearchProduct.Contracts;
using RedisSearchProduct.Data.Redis;
using StackExchange.Redis;
using System.Text.Json;
using RediSearchClient.Query;
using RediSearchClient;
using static System.Net.Mime.MediaTypeNames;

namespace RedisSearchProduct.Data.Products.Services
{
	public interface ISearchService
	{
		Task<SearchResponseDto> SearchProducts(SearchRequestDto searchRequest);
	}

	public class SearchService : ISearchService
	{
		private readonly IRedisService _redisService;

		public SearchService(IRedisService redisService)
		{
			_redisService = redisService;
		}

        private TimeSpan KeyExpiry => TimeSpan.FromSeconds(120);

        public async Task<SearchResponseDto> SearchProducts(SearchRequestDto searchRequest)
        {
			var db = _redisService.Database;

            string resultsKey = $"results:{ObjectHasher.Hash(new
            {
                searchRequest.Text,
                searchRequest.Filters
            })}";

            List<RedisKey> cacheKeys = new List<RedisKey>(5);

            var batch = db.CreateTransaction();

            if ((searchRequest.Filters == null || searchRequest.Filters.Length == 0) &&
                string.IsNullOrWhiteSpace(searchRequest.Text))
            {
                cacheKeys.Add("default:products");
            }

            if (!string.IsNullOrWhiteSpace(searchRequest.Text))
            {
                string textKey = $"results:text:{ObjectHasher.Hash(searchRequest.Text)}";

                if (!await db.KeyExistsAsync(textKey))
                    await HandleTextSearch(db, batch, textKey, searchRequest.Text);

                //Extend the expiry on this key
                batch.KeyExpireAsync(textKey, KeyExpiry);

                cacheKeys.Add(textKey);
            }

            if (searchRequest.Filters != null && searchRequest.Filters.Length > 0)
            {
                string filtersKey = $"results:filters:{ObjectHasher.Hash(searchRequest.Filters)}";

                if (!await db.KeyExistsAsync(filtersKey))
                    HandleFilters(batch, filtersKey, searchRequest.Filters);

                //Extend the expiry on this key
                batch.KeyExpireAsync(filtersKey, KeyExpiry);

                cacheKeys.Add(filtersKey);
            }

            await batch.ExecuteAsync();

            var total = await db.SortedSetCombineAndStoreAsync(SetOperation.Intersect,
                resultsKey,
                cacheKeys.ToArray());

            var start = (searchRequest.PageNumber - 1) * searchRequest.PageSize;
            var stop = searchRequest.PageNumber * searchRequest.PageSize - 1;

            var results = await db.SortedSetRangeByRankAsync(resultsKey, start, stop);

            if (results.Length == 0) return new SearchResponseDto(0, total, Array.Empty<ProductDto>());

            ProductDto[] products = new ProductDto[results.Length];
            for (int i = 0; i < results.Length; i++)
            {
                var jsonString = await db.StringGetAsync($"json:{results[i].ToString()}");
                if (!jsonString.HasValue) continue;

                products[i] = JsonSerializer.Deserialize<ProductDto>(jsonString!)!;
            }

            return new SearchResponseDto(products.Length, total, products);
        }

        private async Task HandleTextSearch(IDatabase db, IBatch batch, string cacheKey, string text)
        {
            var queryDefinition = RediSearchQuery
                    .On("product:search")
                    .UsingQuery(text)
                    .NoContent()
                    .Limit(0, 10000)
                    .Build();

            var result = await db.SearchAsync(queryDefinition);

            if (result.RecordCount > 0)
            {
                List<RedisValue> productIds = new List<RedisValue>(result.RecordCount);
                for (int i = 1; i <= result.RecordCount; i++)
                {
                    productIds.Add(result.RawResult[i].ToString()!.Split("fields:")[1]);
                }
                batch.SetAddAsync(cacheKey, productIds.ToArray());
            }
        }

		private void HandleFilters(IBatch batch, string cacheKey, SearchRequestFilterDto[] filters)
		{
            for (int i = 0; i < filters.Length; i++)
            {
                var filter = filters[i];

                if (filter.Values != null)
                {
                    RedisKey[] redisKeys = new RedisKey[filter.Values.Length];
                    for (int ii = 0; i < filter.Values?.Length; i++)
                    {
                        redisKeys[i] = $"filters:{filter.Name}:{filter.Values[ii]}";
                    }
                    batch.SetCombineAndStoreAsync(SetOperation.Union, cacheKey, redisKeys);
                }
            }
        }
    }
}

