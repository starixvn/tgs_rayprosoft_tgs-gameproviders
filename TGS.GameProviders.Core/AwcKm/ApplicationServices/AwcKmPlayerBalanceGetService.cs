using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcKm.ApplicationServices;

public class AwcKmPlayerBalanceGetService : IGetService<AwcKmPlayerBalanceQuery, decimal?>
{
    private readonly IBrandService _brand;
    private readonly ICommandService<AwcKmCreatePlayerCommand> _createPlayerCommandService;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameInterfaceAwcServiceGrpc _grpc;
    private readonly IPlayerRepository _player;
    private readonly ISettingRepository _setting;

    public AwcKmPlayerBalanceGetService(
        IBrandService brand,
        ICommandService<AwcKmCreatePlayerCommand> createPlayerCommandService,
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

    public async ValueTask<decimal?> GetAsync(AwcKmPlayerBalanceQuery command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_KM).ConfigureAwait(false);
        var configuration = await _setting.GetAwcKmConfigurationAsync().ConfigureAwait(false);
        var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configurationData = await _gameProvider.GetAwcConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

        var response = await _grpc.GetBalanceAsync(
            apiUrl: configuration.ApiUrl,
            agentId: configurationData.AgentId,
            cert: configurationData.Cert,
            playerCode: player.PlayerCode,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (response.Success && !response.IsPlayerExist)
        {
            await _createPlayerCommandService.ExecuteAsync(new AwcKmCreatePlayerCommand(
                Configuration: configuration,
                ConfigurationData: configurationData,
                PlayerCode: player.PlayerCode,
                CurrencyCode: player.DefaultWalletCurrencyCode),
                cancellationToken).ConfigureAwait(false);

            response = await _grpc.GetBalanceAsync(
                apiUrl: configuration.ApiUrl,
                agentId: configurationData.AgentId,
                cert: configurationData.Cert,
                playerCode: player.PlayerCode,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        return !response.Success
            ? throw new Exception($"This game product has failed to withdraw the balance..{response.Message}")
            : response.Balance;
    }
}
