using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcVenus.ApplicationServices
{
    public class AwcVenusPlayerBalanceGetService : IGetService<AwcVenusPlayerBalanceQuery, decimal?>
    {
        private readonly IBrandService _brand;
        private readonly IGameProviderRepository _gameProvider;
        private readonly IGameInterfaceAwcServiceGrpc _grpc;
        private readonly IPlayerRepository _player;
        private readonly ISettingRepository _setting;

        public AwcVenusPlayerBalanceGetService(
            IBrandService brand,
            IGameProviderRepository gameProvider,
            IGameInterfaceAwcServiceGrpc grpc,
            IPlayerRepository player,
            ISettingRepository setting)
        {
            _brand = brand ?? throw new ArgumentNullException(nameof(brand));
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _setting = setting ?? throw new ArgumentNullException(nameof(setting));
        }

        public async ValueTask<decimal?> GetAsync(AwcVenusPlayerBalanceQuery command, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_VENUS).ConfigureAwait(false);
            var configuration = await _setting.GetAwcVenusConfigurationAsync().ConfigureAwait(false);
            var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
            var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
            var configurationData = await _gameProvider.GetAwcConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);
            var apiUrl = configuration.ApiUrl;
            string agentId = configurationData.AgentId;
            string cert = configurationData.Cert;
            string playerCode = command.PlayerCode;

            var balanceReply = await _grpc.GetBalanceAsync(
                apiUrl,
                agentId,
                cert,
                playerCode,
                cancellationToken).ConfigureAwait(false);

            if (!balanceReply.Success)
            {
                throw new Exception($"The player does not exist.{command.PlayerCode}");
            }

            if (!balanceReply.IsPlayerExist)
            {
                string currency = string.Empty;
                string betLimit = string.Empty;
                await _grpc.CreatePlayerByBetLimitAsync(
                     apiUrl,
                     agentId,
                     cert,
                     player.PlayerCode,
                     player.PlayerCode,
                     currency,
                     betLimit,
                     cancellationToken: cancellationToken).ConfigureAwait(false);

                balanceReply = await _grpc.GetBalanceAsync(
                                apiUrl,
                                agentId,
                                cert,
                                playerCode,
                                cancellationToken).ConfigureAwait(false);
            }

            return balanceReply.Balance;
        }
    }
}
