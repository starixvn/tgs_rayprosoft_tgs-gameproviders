using System;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Tgs.GameProviders
{
    internal class RedisConnectionFactory : IDisposable
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private bool _disposedValue;

        public IConnectionMultiplexer Connection { get; }

        public IDatabase Database { get; }

        public RedisConnectionFactory(
            IOptions<RedisOptions> optionsAccessor)
        {
            if (optionsAccessor is null)
                throw new ArgumentNullException(nameof(optionsAccessor));

            var options = optionsAccessor.Value;

            var connectionOptions = ConfigurationOptions.Parse(options.ConnectionString);

            _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionOptions);
            Connection = _connectionMultiplexer;
            Database = _connectionMultiplexer.GetDatabase();
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _connectionMultiplexer?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }
    }
}
