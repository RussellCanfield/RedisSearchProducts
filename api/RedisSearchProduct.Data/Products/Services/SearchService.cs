using System;
using RedisSearchProduct.Contracts;
using RedisSearchProduct.Data.Redis;
using StackExchange.Redis;
using System.Text.Json;

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
                searchRequest.Filters
            })}";

            List<string> cacheKeys = new List<string>(5);

            var batch = db.CreateBatch();

            if (searchRequest.Filters == null || searchRequest.Filters.Length == 0)
            {
                cacheKeys.Add("default:products");
            }

            if (searchRequest.Filters != null && searchRequest.Filters.Length > 0)
            {
                string filtersKey = $"results:filters:{ObjectHasher.Hash(searchRequest.Filters)}";

                if (!await db.KeyExistsAsync(filtersKey))
                    HandleFilters(batch, filtersKey, searchRequest.Filters);
                else
                    batch.KeyExpireAsync(filtersKey, KeyExpiry);

                cacheKeys.Add(filtersKey);
            }

            batch.Execute();

            var total = await db.SortedSetCombineAndStoreAsync(SetOperation.Intersect,
                resultsKey,
                cacheKeys.Select(c => (RedisKey)c).ToArray());

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

            batch.KeyExpireAsync(cacheKey, KeyExpiry);
        }
    }
}

