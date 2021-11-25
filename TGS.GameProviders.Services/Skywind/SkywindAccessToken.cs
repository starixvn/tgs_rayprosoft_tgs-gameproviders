using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TGS.GameProviders.Core.Skywind;
using TGS.GameProviders.Core.Skywind.Models;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Repositories;

namespace TGS.GameProviders.Services.Skywind
{
    internal class SkywindAccessToken : ISkywindAccessToken
    {
        private readonly ISkywindApiClient _client;
        private readonly ILogger<SkywindAccessToken> _logger;
        private readonly IDatabase _redisDatabase;
        private readonly ISkywindService _service;

        public SkywindAccessToken(
            ILogger<SkywindAccessToken> logger,
            ISkywindApiClient client,
            ISkywindService service,
            RedisConnectionFactory redisConnectionFactory
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _redisDatabase = redisConnectionFactory.Database;
        }

        public async ValueTask<string> GetAccessTokenAsync()
        {
            var redisKey = $"{GameProviderType.SexyGaming}:AccessToken";
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var accessToken = await _redisDatabase.StringGetAsync(redisKey).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(accessToken))
            {
                return accessToken;
            }
            var request = new SkywindUserLoginRequest()
            {
                SecretKey = configuration.SecretKey,
                Username = configuration.Username,
                Password = configuration.Password
            };
            var response = await _client.GetAccessTokenAsync(configuration.ApiUrl, request).ConfigureAwait(false);
            if (string.IsNullOrEmpty(response.AccessToken))
            {
                _logger.Log(
                    LogLevel.Warning,
                    new EventId(1000, nameof(SkywindAccessToken)),
                    request,
             null,
             (state, ex) =>
             $"【{nameof(request)}】->{JsonSerializer.Serialize(state)}.{ Environment.NewLine}" +
             $"【DateTime】->{DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(+8))}.{ Environment.NewLine}" +
             $"【{nameof(response)}】->{JsonSerializer.Serialize(response)}.{ Environment.NewLine}" +
             $"【Message】->取得AccessToken失敗"
             );

                return accessToken;
            }
            var isSet = await _redisDatabase.StringSetAsync(redisKey, response.AccessToken, TimeSpan.FromMinutes(45)).ConfigureAwait(false);

            return isSet ? response.AccessToken : null;
        }
    }
}
