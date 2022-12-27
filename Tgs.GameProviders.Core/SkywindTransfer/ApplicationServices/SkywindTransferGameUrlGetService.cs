using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameToken.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;

public class SkywindTransferGameUrlGetService : IGetService<SkywindTransferGameUrlQuery, GameUrlResult>
{
    private readonly IGameInterfaceSkywindClient _client;
    private readonly ICommandService<SkywindTransferCreatePlayerCommand> _createPlayerCommandService;
    private readonly IGameRepository _game;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameTokenClient _gameTokenClient;
    private readonly IPlayerRepository _player;
    private readonly ISettingRepository _setting;

    public SkywindTransferGameUrlGetService(
        ICommandService<SkywindTransferCreatePlayerCommand> createPlayerCommandService,
        IGameRepository game,
        IGameProviderRepository gameProvider,
        IGameInterfaceSkywindClient client,
        IPlayerRepository player,
        ISettingRepository setting,
        IGameTokenClient gameTokenClient)
    {
        _createPlayerCommandService = createPlayerCommandService ?? throw new ArgumentNullException(nameof(createPlayerCommandService));
        _game = game ?? throw new ArgumentNullException(nameof(game));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
        _gameTokenClient = gameTokenClient ?? throw new ArgumentNullException(nameof(gameTokenClient));
    }

    public async ValueTask<GameUrlResult> GetAsync(SkywindTransferGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.SkywindTransfer).ConfigureAwait(false);
        var configuration = await _setting.GetSkywindTransferConfigurationAsync().ConfigureAwait(false);
        var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var currencyCode = await _gameProvider
                       .GetGameProviderCurrencyCodeAsync(gameProviderId, query.CurrencyCode)
                       .ConfigureAwait(false)
                   ?? throw new Exception($"This game product currency does not exist.({query.CurrencyCode})");
        var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, query.Language) ?? "en";
        var game = await _game.GetAsync(query.GameId.Value).ConfigureAwait(false);

        Uri gameUrl;

        if (game.GameType == GameType.LiveDealerLobby)
        {
            var info = new ValidateTicketInfo()
            {
                PlayerCode = player.PlayerCode,
                CurrencyCode = currencyCode,
                TestCust = false,
                Country = "CN"
            };
            var authToken = await _gameTokenClient
               .CreateAuthAsync(
                   playerCode: player.PlayerCode,
                   info: JsonSerializer.Serialize(info),
                   cancellationToken: cancellationToken)
               .ConfigureAwait(false);

            gameUrl = new($"{configuration.LiveLobbyUrl}#/ticket/{authToken}?lang={cultureCode}");
        }
        else
        {
            var request = new GameUrlRequest()
            {
                ApiUrl = configuration.ApiUrl.ToString(),
                SecretKey = configuration.SecretKey,
                Username = configuration.Username,
                Password = configuration.Password,
                PlayerCode = query.PlayerCode,
                GameCode = game.GameExternalId,
                Language = cultureCode
            };

            var reply = await _client
                    .GameUrlAsync(
                        request: request,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            if (!reply.IsPlayer)
            {
                await _createPlayerCommandService.ExecuteAsync(new SkywindTransferCreatePlayerCommand(
                    Configuration: configuration,
                    PlayerCode: player.PlayerCode,
                    CurrencyCode: player.DefaultWalletCurrencyCode),
                    cancellationToken).ConfigureAwait(false);

                reply = await _client
                        .GameUrlAsync(
                            request: request,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
            }
            gameUrl = (reply.IsPlayer) ? new(reply.GameUrl) : null;
        }
        return gameUrl is not null ? new() { Url = gameUrl } : null;
    }
}
