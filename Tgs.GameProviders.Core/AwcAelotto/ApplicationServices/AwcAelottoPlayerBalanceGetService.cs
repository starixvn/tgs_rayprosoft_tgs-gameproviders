using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcAelotto.ApplicationServices
{
    public class AwcAelottoPlayerBalanceGetService : IGetService<AwcAelottoPlayerBalanceQuery, decimal?>
    {
        private readonly IBrandService _brand;
        private readonly ICommandService<AwcAelottoCreatePlayerCommand> _createPlayerCommandService;
        private readonly IGameProviderRepository _gameProvider;
        private readonly IGameInterfaceAwcServiceGrpc _grpc;
        private readonly IPlayerRepository _player;
        private readonly ISettingRepository _setting;

        public AwcAelottoPlayerBalanceGetService(
            IBrandService brand,
            ICommandService<AwcAelottoCreatePlayerCommand> createPlayerCommandService,
            IGameProviderRepository gameProvider,
            IGameInterfaceAwcServiceGrpc grpc,
            IPlayerRepository player,
            ISettingRepository setting)
        {
            _brand = brand ?? throw new ArgumentNullException(nameof(brand));
            _createPlayerCommandService = createPlayerCommandService ?? throw new ArgumentNullException(nameof(createPlayerCommandService));
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _setting = setting ?? throw new ArgumentNullException(nameof(setting));
        }

        public async ValueTask<decimal?> GetAsync(AwcAelottoPlayerBalanceQuery command, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_AELOTTO).ConfigureAwait(false);
            var configuration = await _setting.GetAwcAelottoConfigurationAsync().ConfigureAwait(false);
            var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
            var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
            var configurationData = await _gameProvider.GetAwcConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

            var response = await _grpc.GetBalanceAsync(
                configuration.ApiUrl,
                configurationData.AgentId,
                configurationData.Cert,
                command.PlayerCode,
                cancellationToken).ConfigureAwait(false);

            if (response.Success && !response.IsPlayerExist)
            {
                await _createPlayerCommandService.ExecuteAsync(new AwcAelottoCreatePlayerCommand(
                    Configuration: configuration,
                    ConfigurationData: configurationData,
                    PlayerCode: player.PlayerCode,
                    CurrencyCode: player.DefaultWalletCurrencyCode),
                    cancellationToken).ConfigureAwait(false);

                response = await _grpc.GetBalanceAsync(
                    configuration.ApiUrl,
                    configurationData.AgentId,
                    configurationData.Cert,
                    command.PlayerCode,
                    cancellationToken).ConfigureAwait(false);
            }

            return !response.Success
                ? throw new Exception($"This game product has failed to withdraw the balance..{response.Message}")
                : response.Balance;
        }
    }
}
