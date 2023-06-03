using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SM01Service.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SM01.ApplicationServices;

public class SM01PlayerFundOutGetService : IGetService<SM01PlayerFundOutQuery, PlayerFundResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceSM01Client _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public SM01PlayerFundOutGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceSM01Client client,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<PlayerFundResult> GetAsync(SM01PlayerFundOutQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: GameProviderType.SM01,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var player = await _player
        .GetAsync(
            playerCode: query.PlayerCode,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand
            .GetBrandGameProviderConfigurationIdAsync(
                gameProviderId: gameProviderId,
                brandId: player.BrandId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var configuration = await _gameProvider
            .GetSM01ConfigurationAsync(
                gameProviderConfigurationId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var reply = await _client
            .FundOutAsync(
                apiUrl: configuration.ApiUrl,
                merchantId: configuration.MerchantId,
                merchantKey: configuration.MerchantKey,
                customerName: player.PlayerCode,
                refTransactionCode: query.OrderId,
                amount: int.Parse(query.Amount.ToString()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new PlayerFundResult()
        {
            OrderId = reply.RefTransactionCode
        };
    }
}
