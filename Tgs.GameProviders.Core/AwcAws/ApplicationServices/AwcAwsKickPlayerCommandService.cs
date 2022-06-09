using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcAws.ApplicationServices
{
    public class AwcAwsKickPlayerCommandService : ICommandService<AwcAwsKickPlayerCommand>
    {
        private readonly IBrandService _brand;
        private readonly IGameProviderRepository _gameProvider;
        private readonly IGameInterfaceAwcServiceGrpc _grpc;
        private readonly IPlayerRepository _player;
        private readonly ISettingRepository _setting;

        public AwcAwsKickPlayerCommandService(
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

        public async ValueTask ExecuteAsync(AwcAwsKickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_AWS).ConfigureAwait(false);
            var configuration = await _setting.GetAwcAwsConfigurationAsync().ConfigureAwait(false);
            var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
            var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
            var configurationData = await _gameProvider.GetAwcConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);
            var apiUrl = configuration.ApiUrl;
            string agentId = configurationData.AgentId;
            string cert = configurationData.Cert;
            string playerCode = command.PlayerCode;

            await _grpc.KickPlayerAsync(
                apiUrl,
                agentId,
                cert,
                playerCode,
                cancellationToken).ConfigureAwait(false);
        }
    }
}
