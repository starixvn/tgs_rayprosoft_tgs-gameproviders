using System;
using StackExchange.Redis;

namespace TGS.GameProviders.RedisRepositories
{
    public interface IRedisConnection : IDisposable
    {
        IConnectionMultiplexer GetConnection();
    }
}
