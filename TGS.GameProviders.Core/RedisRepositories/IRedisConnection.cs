using System;
using StackExchange.Redis;

namespace TGS.GameProviders.Core.RedisRepositories
{
    public interface IRedisConnection : IDisposable
    {
        IConnectionMultiplexer GetConnection();
    }
}
