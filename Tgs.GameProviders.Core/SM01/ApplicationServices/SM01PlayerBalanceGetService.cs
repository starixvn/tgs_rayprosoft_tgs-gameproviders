using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SM01Service.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SM01.ApplicationServices;

internal class SM01PlayerBalanceGetService : IGetService<SM01PlayerBalanceQuery, decimal?>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceSM01Client _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public SM01PlayerBalanceGetService(
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

    public async ValueTask<decimal?> GetAsync(SM01PlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.SM01).ConfigureAwait(false);
        var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetSM01Configuration(gameProviderConfigurationId).ConfigureAwait(false);

        var reply = await _client
            .PlayerBalanceAsync(
                apiUrl: configuration.ApiUrl,
                merchantId: configuration.MerchantId,
                merchantKey: configuration.MerchantKey,
                customerName: player.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            await _client
                .PlayerCreateAsync(
                    apiUrl: configuration.ApiUrl,
                    merchantId: configuration.MerchantId,
                    merchantKey: configuration.MerchantKey,
                    customerName: player.PlayerCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            reply = await _client
               .PlayerBalanceAsync(
                    apiUrl: configuration.ApiUrl,
                    merchantId: configuration.MerchantId,
                    merchantKey: configuration.MerchantKey,
                    customerName: player.PlayerCode,
                   cancellationToken: cancellationToken)
               .ConfigureAwait(false);
        }

        return reply.IsPlayerExist ? reply.Balance : null;
    }
}
