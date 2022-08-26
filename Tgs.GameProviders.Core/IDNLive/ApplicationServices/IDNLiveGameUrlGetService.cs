using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.IdnLiveService.Client;
using Tgs.GamePlayerTransform.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameToken.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.IDNLive.ApplicationServices;

public class IDNLiveGameUrlGetService : IGetService<IDNLiveGameUrlQuery, GameUrlResult>
{
    private readonly IGameInterfaceIdnLiveClient _client;
    private readonly IGamePlayerTransformClient _gamePlayerTransformClient;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameTokenClient _gameTokenClient;
    private readonly IPlayerRepository _player;
    private readonly ISettingRepository _setting;

    public IDNLiveGameUrlGetService(
        IGameProviderRepository gameProvider,
        IGameInterfaceIdnLiveClient client,
        IPlayerRepository player,
        ISettingRepository setting,
        IGameTokenClient gameTokenClient,
        IGamePlayerTransformClient gamePlayerTransformClient)
    {
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
        _gameTokenClient = gameTokenClient ?? throw new ArgumentNullException(nameof(gameTokenClient));
        _gamePlayerTransformClient = gamePlayerTransformClient ?? throw new ArgumentNullException(nameof(gamePlayerTransformClient));
    }

    public async ValueTask<GameUrlResult> GetAsync(IDNLiveGameUrlQuery command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.IDNLive).ConfigureAwait(false);
        var configuration = await _setting.GetIDNLiveConfigurationAsync().ConfigureAwait(false);
        var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, command.Language) ?? "en";
        var currencyCode = await _gameProvider
                .GetGameProviderCurrencyCodeAsync(gameProviderId, command.CurrencyCode)
                .ConfigureAwait(false)
            ?? throw new Exception($"This game product currency does not exist.({command.CurrencyCode})");

        var gamePlayer = await _gamePlayerTransformClient
                .GamePlayerTransformIdnLiveGetAsync(
                    playerCode: player.PlayerCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? await _gamePlayerTransformClient
                .GamePlayerTransformIdnLiveGetAddAsync(
                    playerCode: player.PlayerCode,
                    JsonSerializer.Serialize(new IdnLivePlayerconfiguration()),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

        var config = JsonSerializer.Deserialize<IdnLivePlayerconfiguration>(gamePlayer.Configuration);

        var info = new IdnLivePlayerInfo()
        {
            Uid = gamePlayer.Id,
            WalletId = gamePlayer.DefaultWalletId,
            Nickname = gamePlayer.PlayerCode,
            Currency = currencyCode,
            Table = config.Table,
            Language = cultureCode,
            ClientIP = command.IPAddress
        };

        var auth = await _gameTokenClient
                    .CreateAuthAsync(
                        playerCode: player.PlayerCode,
                        info: JsonSerializer.Serialize(info),
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

        var gameUrl = await _client
                 .GetGameUrl(
                     apiUrl: configuration.ApiUrl,
                     operatorId: configuration.OperatorId,
                     token: auth,
                     currency: currencyCode,
                     language: cultureCode,
                     cancellationToken: cancellationToken)
                 .ConfigureAwait(false);

        return gameUrl is not null ? new() { Url = gameUrl } : null;
    }
}
