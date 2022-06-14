using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcLudo.ApplicationServices
{
    internal class AwcLudoCreatePlayerCommandService : ICommandService<AwcLudoCreatePlayerCommand>
    {
        private readonly IGameProviderRepository _gameProvider;
        private readonly IGameInterfaceAwcServiceGrpc _grpc;

        public AwcLudoCreatePlayerCommandService(
            IGameProviderRepository gameProvider,
            IGameInterfaceAwcServiceGrpc grpc)
        {
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
        }

        public async ValueTask ExecuteAsync(AwcLudoCreatePlayerCommand command, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_LUDO).ConfigureAwait(false);
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
