using System;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.GameProviders.Core.IMESports;
using TGS.GameProviders.Core.IMESports.Models;
using TGS.GameProviders.Repositories;

namespace TGS.GameProviders.Services.IMESports
{
    internal class IMESportsRedisDataStore : IIMESportsDataStore
    {
        private static readonly string _KeyPrefix = "IMSessionToken";
        private readonly IDatabase _database;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public IMESportsRedisDataStore(RedisConnectionFactory redisConnectionFactory)
        {
            if (redisConnectionFactory is null)
            {
                throw new ArgumentNullException(nameof(redisConnectionFactory));
            }

            _database = redisConnectionFactory.Database;

            _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = false };
        }

        public async ValueTask CacheSessionTokenAsync(
            string playerCode,
            string gameExternalId,
            string sessionToken,
            int timeToLeave)
        {
            var tokenInfo = new IMESportsTokenInfo(sessionToken, timeToLeave);
            var expiry = TimeSpan.FromSeconds(timeToLeave);

            var dataKey = $"{_KeyPrefix}:{playerCode}_{gameExternalId}";
            var setValue = JsonSerializer.Serialize(tokenInfo, _jsonSerializerOptions);

            await _database.StringSetAsync(dataKey, setValue, expiry, When.Always)
                .ConfigureAwait(false);
        }
    }
}