using System;
namespace RedisSearchProduct.Data.Products.Models
{
	public class Filter
	{
		public string Name { get; set; }
		public FilterValue[] Values { get; set; }
	}

	public class FilterValue
	{
		public string Name { get; set; }
		public int Count { get; set; }
	}
}

