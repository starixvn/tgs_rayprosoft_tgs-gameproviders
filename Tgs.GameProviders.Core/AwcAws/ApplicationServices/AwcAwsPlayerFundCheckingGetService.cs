using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcAws.ApplicationServices
{
    public class AwcAwsPlayerFundCheckingGetService : IGetService<AwcAwsPlayerFundCheckingQuery, PlayerFundCheckingResult>
    {
        private readonly IBrandService _brand;
        private readonly IGameProviderRepository _gameProvider;
        private readonly IGameInterfaceAwcServiceGrpc _grpc;
        private readonly IPlayerRepository _player;
        private readonly ISettingRepository _setting;

        public AwcAwsPlayerFundCheckingGetService(
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

        public async ValueTask<PlayerFundCheckingResult> GetAsync(AwcAwsPlayerFundCheckingQuery command, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_AWS).ConfigureAwait(false);
            var configuration = await _setting.GetAwcAwsConfigurationAsync().ConfigureAwait(false);
            var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
            var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
            var configurationData = await _gameProvider.GetAwcConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

            var response = await _grpc.FundOrderCheckingAsync(
                apiUrl: configuration.ApiUrl,
                agentId: configurationData.AgentId,
                cert: configurationData.Cert,
                transactionId: command.OrderId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            return new() { OrderStatus = response };
        }
    }
}
