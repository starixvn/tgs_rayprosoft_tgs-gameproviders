using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.GameProviders.RedisRepositories.Models;

namespace TGS.GameProviders.RedisRepositories
{
    internal sealed class GameProviderScheduleLockRepository : IGameProviderScheduleLockRepository
    {
        private readonly IRedisConnection _connection;
        private readonly string _prefix = "GameProviderScheduleLock";

        public GameProviderScheduleLockRepository(IRedisConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async ValueTask<GameProviderScheduleLockData> GetAsync(string gameProviderCode, string key)
        {
            var connection = _connection.GetConnection();
            var db = connection.GetDatabase();
            var value = await db.StringGetAsync(Format(gameProviderCode, key)).ConfigureAwait(false);
            return JsonSerializer.Deserialize<GameProviderScheduleLockData>(value);
        }

        public async ValueTask<bool> IsLockAsync(string gameProviderCode, string key)
        {
            var connection = _connection.GetConnection();
            var db = connection.GetDatabase();
            var value = await db.StringGetAsync(Format(gameProviderCode, key)).ConfigureAwait(false);
            return !string.IsNullOrEmpty(value);
        }

        public async ValueTask<bool> RemoveAsync(string gameProviderCode, string key)
        {
            var connection = _connection.GetConnection();
            var db = connection.GetDatabase();
            return await db.KeyDeleteAsync(Format(gameProviderCode, key)).ConfigureAwait(false);
        }

        public async ValueTask RemoveGameProviderCodeAsync(string gameProviderCode)
        {
            var connection = _connection.GetConnection();
            var server = connection.GetServer(_connection.GetConnection().GetEndPoints().First());
            var db = connection.GetDatabase();
            var keys = server.Keys(database: db.Database, pattern: $"{_prefix}:{gameProviderCode}:*", pageSize: int.MaxValue);
            var tasks = new List<Task>();
            foreach (var key in keys)
            {
                Task task = Task.Run(()
                    => db.KeyDeleteAsync(key));
                tasks.Add(task);
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async ValueTask<bool> SetAsync(string gameProviderCode, string key, GameProviderScheduleLockData date)
        {
            var connection = _connection.GetConnection();
            var db = connection.GetDatabase();
            return await db.StringSetAsync(Format(gameProviderCode, key), JsonSerializer.Serialize(date)).ConfigureAwait(false);
        }

        private string Format(string gameProviderCode, string key)
        {
            return string.Format("{0}:{1}:{2}", _prefix, gameProviderCode, key);
        }
    }
}
