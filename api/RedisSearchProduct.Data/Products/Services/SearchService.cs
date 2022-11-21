using System;
using RedisSearchProduct.Contracts;
using RedisSearchProduct.Data.Redis;

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

        public async Task<SearchResponseDto> SearchProducts(SearchRequestDto searchRequest)
        {
			var db = _redisService.Database;

			return new SearchResponseDto(0, 0, Array.Empty<ProductDto>());
        }
    }
}

