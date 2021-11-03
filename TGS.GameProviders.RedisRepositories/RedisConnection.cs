using System;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using TGS.GameProviders.Core.RedisRepositories;
using TGS.GameProviders.RedisRepositories.Options;

namespace TGS.GameProviders.RedisRepositories
{
    internal class RedisConnection : IRedisConnection
    {
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;
        private readonly RedisOption _redisOption;

        public RedisConnection(IOptions<RedisOption> redisOption)
        {
            _redisOption = redisOption.Value;
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
          {
              var connection = ConnectionMultiplexer.Connect(_redisOption.ConnectionString);
              Console.WriteLine($"{nameof(ConnectionMultiplexer)} connection opened.");
              connection.ConnectionFailed += OnConnectionFailed;
              connection.ConnectionRestored += ConnectionRestored;
              connection.ErrorMessage += ErrorMessage;
              connection.InternalError += InternalError;
              return connection;
          });
        }

        void IDisposable.Dispose()
        {
            if (_lazyConnection.IsValueCreated)
            {
                _lazyConnection.Value.Dispose();
                Console.WriteLine($"{nameof(ConnectionMultiplexer)} connection closed.");
            }
        }

        IConnectionMultiplexer IRedisConnection.GetConnection()
        {
            return _lazyConnection.Value;
        }

        private void ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine($@"{nameof(ConnectionMultiplexer)} connection restored.
FailureType is {e.FailureType}.
ConnectionType is {e.ConnectionType}.
Endpoint is {e.EndPoint}.
Exception is {e.Exception?.ToString() ?? "null"}");
        }

        private void ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Console.WriteLine($@"{nameof(ConnectionMultiplexer)} got an error message: {e.Message}.
Endpoint is {e.EndPoint}.");
        }

        private void InternalError(object sender, InternalErrorEventArgs e)
        {
            Console.WriteLine($@"{nameof(ConnectionMultiplexer)} internal error.
ConnectionType is {e.ConnectionType}.
Endpoint is {e.EndPoint}.
Origin is {e.Origin}.
Exception is {e.Exception?.ToString() ?? "null"}");
        }

        private void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine($@"{nameof(ConnectionMultiplexer)} connection failed.
FailureType is {e.FailureType}.
ConnectionType is {e.ConnectionType}.
Endpoint is {e.EndPoint}.
Exception is {e.Exception?.ToString() ?? "null"}");
        }
    }
}
