using System;
using StackExchange.Redis;

namespace RedisSearchProduct.Data.Redis
{
	public interface IRedisService
	{
        IDatabase Database { get; }
        IServer Server { get;  }
    }
}

