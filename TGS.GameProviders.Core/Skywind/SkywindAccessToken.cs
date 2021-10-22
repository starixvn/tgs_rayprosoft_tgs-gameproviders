using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.Domains;
using TGS.GameProviders.RedisRepositories;

namespace TGS.GameProviders.Core.Skywind
{
    internal class SkywindAccessToken : ISkywindAccessToken
    {
        private readonly ISkywindApiClient _client;
        private readonly ILogger<SkywindAccessToken> _logger;
        private readonly ISkywindService _service;
        private readonly IRedisConnection _redisConnection;
        public SkywindAccessToken(
            ILogger<SkywindAccessToken> logger,
            ISkywindApiClient client,
            ISkywindService service,
            IRedisConnection redisConnection
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
        }
        public async ValueTask<string> GetAccessTokenAsync()
        {
            var configuration = _service.GetSettingConfigurationAsync();
            var accessToken = await _redisConnection.GetConnection().GetDatabase().StringGetAsync($"{GameProviderType.SexyGaming}:AccessToken").ConfigureAwait(false);
            if (string.IsNullOrEmpty(accessToken))
            {

                var response = await _client.GetAccessTokenAsync < (HttpMethod.Post, configuration.ApiUrl, " /login", source, logPrefix: $"{_gameProvider}.UserLogin").ConfigureAwait(false);
                if (!string.IsNullOrEmpty(response.AccessToken))
                {
                    await _redisCacheManager.SafeCacheAsync(_gameProvider, _accessTokenCacheType, TimeSpan.FromMinutes(45), response.AccessToken).ConfigureAwait(false);
                    accessToken = response.AccessToken;
                }
            }
            return accessToken;
        }
    }
}
