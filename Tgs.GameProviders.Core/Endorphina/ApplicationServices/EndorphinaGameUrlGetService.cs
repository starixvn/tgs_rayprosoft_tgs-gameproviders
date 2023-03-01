using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.EndorphinaService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Endorphina.ApplicationServices;

internal class EndorphinaGameUrlGetService : IGetService<EndorphinaGameUrlQuery, Uri>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceEndorphinaClient _client;
    private readonly IGameRepository _game;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public EndorphinaGameUrlGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceEndorphinaClient client,
        IPlayerRepository player,
        IGameRepository game)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _game = game ?? throw new ArgumentNullException(nameof(game));
    }

    public async ValueTask<Uri> GetAsync(EndorphinaGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.Endorphina).ConfigureAwait(false);
        var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetEndorphinaConfiguration(gameProviderConfigurationId).ConfigureAwait(false);
        var game = await _game.GetAsync(query.GameId.Value).ConfigureAwait(false)
             ?? throw new Exception($"The game for this game product does not exist.({query.GameId})");
        var currency = await _gameProvider.GetGameProviderCurrencyCodeAsync(gameProviderId, query.CurrencyCode).ConfigureAwait(false)
            ?? throw new Exception($"This game product currency does not exist.({query.CurrencyCode})");

        var response = await _client
            .GameTokenAsync(
                apiUrl: configuration.ApiUrl,
                merchantId: configuration.MerchantId,
                secretKey: configuration.SecretKey,
                gameId: game.GameExternalId,
                currency: currency,
                username: query.PlayerCode,
                cancellationToken)
            .ConfigureAwait(false);

        var sign = ComputeHash($"{response.Token}{configuration.SecretKey}");

        var url = $"{configuration.ApiUrl}/icasino/sessions/launch?token={response.Token}&sign={sign}";

        return (response != null) ? new(url) : null;
    }

    private static string ComputeHash(string data)
    {
        var dataByte = Encoding.UTF8.GetBytes(data);

        var hash = MD5.Create().ComputeHash(dataByte);

        return Convert.ToHexString(hash).ToUpper();
    }
}
