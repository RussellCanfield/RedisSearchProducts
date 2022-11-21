using System;
using System.Net;
using Microsoft.Extensions.Options;
using RedisSearchProduct.Configuration;
using StackExchange.Redis;

namespace RedisSearchProduct.Data.Redis
{
    public class RedisService : IRedisService
    {
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;
        private readonly RedisOptions _redisOptions;

        public ConnectionMultiplexer Connection => _lazyConnection.Value;

        public RedisService(IOptions<RedisOptions> redisOptions)
        {
            _redisOptions = redisOptions.Value;

            ThreadPool.SetMinThreads(500, 500);

            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect($"{redisOptions.Value.Hostname}:{redisOptions.Value.Port},password={redisOptions.Value.Password},abortConnect=false,allowAdmin=true");
            });
        }

        public IDatabase Database => Connection.GetDatabase(0);

        public IServer Server => Connection.GetServer($"{_redisOptions.Hostname}:{_redisOptions.Port}");
    }
}

