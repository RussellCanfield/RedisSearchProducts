using System;
namespace RedisSearchProduct.Configuration
{
	public record RedisOptions
	{
        public string? Hostname { get; set; }
        public string? Port { get; set; }
        public string? Password { get; set; }
    }
}

