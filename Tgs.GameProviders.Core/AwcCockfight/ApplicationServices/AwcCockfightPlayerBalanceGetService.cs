using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcCockfight.ApplicationServices
{
    public class AwcCockfightPlayerBalanceGetService : IGetService<AwcCockfightPlayerBalanceQuery, decimal?>
    {
        private readonly IBrandService _brand;
        private readonly IAwcCockfightGamePlayer _gamePlayer;
        private readonly IGameProviderRepository _gameProvider;
        private readonly IGameInterfaceAwcServiceGrpc _grpc;
        private readonly IPlayerRepository _player;
        private readonly ISettingRepository _setting;

        public AwcCockfightPlayerBalanceGetService(
            IBrandService brand,
            IAwcCockfightGamePlayer gamePlayer,
            IGameProviderRepository gameProvider,
            IGameInterfaceAwcServiceGrpc grpc,
            IPlayerRepository player,
            ISettingRepository setting)
        {
            _brand = brand ?? throw new ArgumentNullException(nameof(brand));
            _gamePlayer = gamePlayer ?? throw new ArgumentNullException(nameof(gamePlayer));
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _setting = setting ?? throw new ArgumentNullException(nameof(setting));
        }

        public async ValueTask<decimal?> GetAsync(AwcCockfightPlayerBalanceQuery command, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_COCKFIGHT).ConfigureAwait(false);
            var configuration = await _setting.GetAwcCockfightConfigurationAsync().ConfigureAwait(false);
            var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
            var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
            var configurationData = await _gameProvider.GetAwcConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

            var apiUrl = configuration.ApiUrl;
            string agentId = configurationData.AgentId;
            string cert = configurationData.Cert;
            string playerCode = command.PlayerCode;

            var response = await _grpc.GetBalanceAsync(
                apiUrl,
                agentId,
                cert,
                playerCode,
                cancellationToken).ConfigureAwait(false);

            if (response.Success && !response.IsPlayerExist)
            {
                await _gamePlayer.CreatePlayerAsync(
                      configuration,
                      configurationData,
                      player.PlayerCode,
                      player.DefaultWalletCurrencyCode,
                      cancellationToken: cancellationToken).ConfigureAwait(false);

                response = await _grpc.GetBalanceAsync(
                    apiUrl,
                    agentId,
                    cert,
                    playerCode,
                    cancellationToken).ConfigureAwait(false);
            }

            return !response.Success
                ? throw new Exception($"This game product has failed to withdraw the balance..{response.Message}")
                : response.Balance;
        }
    }
}
