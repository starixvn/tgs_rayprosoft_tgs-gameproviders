using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.EvolutionTransfer.ApplicationServices
{
    public class EvolutionTransferPlayerBalanceGetService : IGetService<EvolutionTransferPlayerBalanceQuery, decimal?>
    {
        private readonly IGameProviderRepository _gameProvider;
        private readonly IGameInterfaceEvolutionClient _grpc;
        private readonly IPlayerRepository _player;
        private readonly ISettingRepository _setting;

        public EvolutionTransferPlayerBalanceGetService(
            IGameProviderRepository gameProvider,
            IGameInterfaceEvolutionClient grpc,
            IPlayerRepository player,
            ISettingRepository setting)
        {
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _setting = setting ?? throw new ArgumentNullException(nameof(setting));
        }

        public async ValueTask<decimal?> GetAsync(EvolutionTransferPlayerBalanceQuery command, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.EvolutionTransfer).ConfigureAwait(false);
            var configuration = await _setting.GetEvolutionTransferConfigurationAsync().ConfigureAwait(false);
            var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);

            var response = await _grpc.GetBalanceAsync(
                configuration.ApiUrl,
                configuration.EcId,
                player.PlayerCode,
                cancellationToken).ConfigureAwait(false);

            if (response.Result)
            {
                return response.Balance;
            }

            var currencyCode = await _gameProvider
                    .GetGameProviderCurrencyCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode)
                    .ConfigureAwait(false)
                ?? throw new Exception($"This game product currency does not exist.({player.DefaultWalletCurrencyCode})");

            _ = await _grpc.GetLiveDealerGameUrlAsync(
                configuration.Ua2AuthenticationUrl,
                player.BrandCode,
                player.PlayerCode,
                (short)PlatformType.Desktop,
                "127.0.0.1",
                Guid.NewGuid().ToString(),
                "cn",
                currencyCode,
                cancellationToken).ConfigureAwait(false);

            response = await _grpc.GetBalanceAsync(
                configuration.ApiUrl,
                configuration.EcId,
                player.PlayerCode,
                cancellationToken).ConfigureAwait(false);

            return response.Balance;
        }
    }
}
