using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.HabaneroService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.HabaneroTransfer.ApplicationServices;

internal class HabaneroTransferPlayerFundOutGetService : IGetService<HabaneroTransferPlayerFundOutQuery, PlayerFundResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceHabaneroClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public HabaneroTransferPlayerFundOutGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceHabaneroClient client,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<PlayerFundResult> GetAsync(HabaneroTransferPlayerFundOutQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.HabaneroTransfer).ConfigureAwait(false);
        var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetHabaneroTransferConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);

        var gameProviderCurrencyCode = await _gameProvider
            .GetGameProviderCurrencyCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode)
            .ConfigureAwait(false)
        ?? throw new Exception($"This game product currency does not exist.({player.DefaultWalletCurrencyCode})");

        var reply = await _client
            .FundOutAsync(
                apiUrl: configuration.ApiUrl,
                brandId: configuration.BrandId,
                apiKey: configuration.ApiKey,
                username: player.PlayerCode,
                password: "HabaTfP@ssw0rd",
                currencyCode: gameProviderCurrencyCode,
                amount: query.Amount,
                transactionId: query.OrderId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new PlayerFundResult
        {
            OrderId = reply.TransactionId
        };
    }
}
