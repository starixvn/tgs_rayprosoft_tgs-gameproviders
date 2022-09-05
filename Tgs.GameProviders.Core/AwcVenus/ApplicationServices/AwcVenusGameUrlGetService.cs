using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcVenus.ApplicationServices;

public class AwcVenusGameUrlGetService : IGetService<AwcVenusGameUrlQuery, GameUrlResult>
{
    private readonly IBrandRepository _brand;
    private readonly ICommandService<AwcVenusCreatePlayerCommand> _createPlayerCommandService;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameInterfaceAwcServiceGrpc _grpc;
    private readonly IPlayerRepository _player;
    private readonly ISettingRepository _setting;

    public AwcVenusGameUrlGetService(
        IBrandRepository brand,
        ICommandService<AwcVenusCreatePlayerCommand> createPlayerCommandService,
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

    public async ValueTask<GameUrlResult> GetAsync(AwcVenusGameUrlQuery command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_VENUS).ConfigureAwait(false);
        var configuration = await _setting.GetAwcVenusConfigurationAsync().ConfigureAwait(false);
        var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configurationData = await _gameProvider.GetAwcConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);
        var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, command.Language) ?? "en";

        var request = new GameInterfaceAwcServiceGameUrlByLiveLobbyRequest()
        {
            ApiUrl = configuration.ApiUrl.ToString(),
            AgentId = configurationData.AgentId,
            Cert = configurationData.Cert,
            PlayerCode = command.PlayerCode,
            BackUrl = command.BackUrl.ToString(),
            IsMobileLogin = command.PlatformType == PlatformType.Mobile,
            BetLimit = JsonSerializer.Serialize(configurationData.BetLimit),
            Language = cultureCode
        };

        var response = await _grpc.GameUrlLiveLobbyByAwcVenusAsync(
            request: request,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (response.Success && !response.IsPlayerExist)
        {
            await _createPlayerCommandService.ExecuteAsync(new AwcVenusCreatePlayerCommand(
                Configuration: configuration,
                ConfigurationData: configurationData,
                PlayerCode: player.PlayerCode,
                CurrencyCode: player.DefaultWalletCurrencyCode,
                Language: command.Language),
                cancellationToken).ConfigureAwait(false);

            response = await _grpc.GameUrlLiveLobbyByAwcVenusAsync(
                request: request,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        return (response != null) ? new() { Url = new(response.GameUrl) } : null;
    }
}
