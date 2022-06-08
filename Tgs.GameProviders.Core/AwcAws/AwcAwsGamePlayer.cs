using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AwcAws
{
    internal class AwcAwsGamePlayer : IAwcAwsGamePlayer
    {
        private readonly IGameProviderRepository _gameProvider;
        private readonly IGameInterfaceAwcServiceGrpc _grpc;

        public AwcAwsGamePlayer(
            IGameProviderRepository gameProvider,
            IGameInterfaceAwcServiceGrpc grpc)
        {
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
        }

        public async ValueTask CreatePlayerAsync(AwcAwsConfiguration configuration, AwcConfigurationData configurationData, string playerCode, string currencyCode, string language = "en-US", CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_AWS).ConfigureAwait(false);
            var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, language).ConfigureAwait(false) ?? "en";
            var currency = await _gameProvider.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false)
                ?? throw new Exception($"This game product currency does not exist.({currencyCode})");
            await _grpc.CreatePlayerAsync(
                 configuration.ApiUrl,
                 configurationData.AgentId,
                 configurationData.Cert,
                 playerCode,
                 playerCode,
                 currency,
                 cultureCode,
                 cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
