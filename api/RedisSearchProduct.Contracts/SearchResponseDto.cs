using System;
namespace RedisSearchProduct.Contracts
{
	public record SearchResponseDto(int count, long total, ProductDto[] Products);
}

