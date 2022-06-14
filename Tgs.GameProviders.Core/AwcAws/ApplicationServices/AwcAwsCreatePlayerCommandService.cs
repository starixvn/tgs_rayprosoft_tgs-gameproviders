using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcAws.ApplicationServices
{
    internal class AwcAwsCreatePlayerCommandService : ICommandService<AwcAwsCreatePlayerCommand>
    {
        private readonly IGameProviderRepository _gameProvider;
        private readonly IGameInterfaceAwcServiceGrpc _grpc;

        public AwcAwsCreatePlayerCommandService(
            IGameProviderRepository gameProvider,
            IGameInterfaceAwcServiceGrpc grpc)
        {
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
        }

        public async ValueTask ExecuteAsync(AwcAwsCreatePlayerCommand command, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_AWS).ConfigureAwait(false);
            var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, command.Language).ConfigureAwait(false) ?? "en";
            var currency = await _gameProvider.GetGameProviderCurrencyCodeAsync(gameProviderId, command.CurrencyCode).ConfigureAwait(false)
                ?? throw new Exception($"This game product currency does not exist.({command.CurrencyCode})");

            await _grpc.CreatePlayerAsync(
                 command.Configuration.ApiUrl,
                 command.ConfigurationData.AgentId,
                 command.ConfigurationData.Cert,
                 command.PlayerCode,
                 command.PlayerCode,
                 currency,
                 cultureCode,
                 cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
