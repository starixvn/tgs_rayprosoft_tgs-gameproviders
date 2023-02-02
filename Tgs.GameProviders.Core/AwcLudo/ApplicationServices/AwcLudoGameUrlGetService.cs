using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcLudo.ApplicationServices;

public class AwcLudoGameUrlGetService : IGetService<AwcLudoGameUrlQuery, GameUrlResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceAwcServiceClient _client;
    private readonly ICommandService<AwcLudoCreatePlayerCommand> _createPlayerCommandService;
    private readonly IGameRepository _game;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;
    private readonly ISettingRepository _setting;

    public AwcLudoGameUrlGetService(
        IBrandRepository brand,
        ICommandService<AwcLudoCreatePlayerCommand> createPlayerCommandService,
        IGameRepository game,
        IGameProviderRepository gameProvider,
        IGameInterfaceAwcServiceClient client,
        IPlayerRepository player,
        ISettingRepository setting)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _createPlayerCommandService = createPlayerCommandService ?? throw new ArgumentNullException(nameof(createPlayerCommandService));
        _game = game ?? throw new ArgumentNullException(nameof(game));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
    }

    public async ValueTask<GameUrlResult> GetAsync(AwcLudoGameUrlQuery command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_LUDO).ConfigureAwait(false);
        var configuration = await _setting.GetAwcLudoConfigurationAsync().ConfigureAwait(false);
        var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configurationData = await _gameProvider.GetAwcConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);
        var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, command.Language) ?? "en";
        var game = await _game.GetAsync(command.GameId.Value).ConfigureAwait(false)
             ?? throw new Exception($"The game for this game product does not exist.({command.GameId})");

        var request = new GameUrlRequest()
        {
            ApiUrl = configuration.ApiUrl.ToString(),
            AgentId = configurationData.AgentId,
            Cert = configurationData.Cert,
            PlayerCode = command.PlayerCode,
            BackUrl = command.BackUrl.ToString(),
            IsMobileLogin = command.PlatformType == PlatformType.Mobile,
            ExternalGameId = game.GameExternalId,
            Language = cultureCode
        };

        var response = await _client.GameUrlByAwcLudoAsync(
            request: request,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (response.Success && !response.IsPlayerExist)
        {
            await _createPlayerCommandService.ExecuteAsync(new AwcLudoCreatePlayerCommand(
                Configuration: configuration,
                ConfigurationData: configurationData,
                PlayerCode: player.PlayerCode,
                CurrencyCode: player.DefaultWalletCurrencyCode,
                Language: command.Language),
                cancellationToken).ConfigureAwait(false);

            response = await _client.GameUrlByAwcLudoAsync(
                request: request,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        return (response != null) ? new() { Url = new(response.GameUrl) } : null;
    }
}
