using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SabaTransfer.ApplicationServices;

public class SabaTransferPlayerBalanceGetService : IGetService<SabaTransferPlayerBalanceQuery, decimal?>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceSabaClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public SabaTransferPlayerBalanceGetService(
        IGameInterfaceSabaClient client,
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IBrandRepository brand)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
    }

    public async ValueTask<decimal?> GetAsync(SabaTransferPlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.SabaTransfer).ConfigureAwait(false);
        var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetSabaTransferConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);

        var reply = await _client
                .PlayerBalanceAsync(
                    apiUrl: configuration.ApiUrl,
                    operatorId: configuration.OperatorId,
                    vendorId: configuration.VendorId,
                    vendorMemberId: player.PlayerCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            var gameProviderCurrencyCode = await _gameProvider
                    .GetGameProviderCurrencyCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode)
                    .ConfigureAwait(false)
                ?? throw new Exception($"This game product currency does not exist.({player.DefaultWalletCurrencyCode})");

            await _client
               .PlayerCreateAsync(
                    configuration.ApiUrl,
                    operatorId: configuration.OperatorId,
                    vendorId: configuration.VendorId,
                    brandCode: player.BrandCode,
                    vendorMemberId: player.PlayerCode,
                    oddsType: configuration.OddsType,
                    currency: int.Parse(gameProviderCurrencyCode),
                    cancellationToken)
               .ConfigureAwait(false);

            reply = await _client
                .PlayerBalanceAsync(
                    apiUrl: configuration.ApiUrl,
                    operatorId: configuration.OperatorId,
                    vendorId: configuration.VendorId,
                    vendorMemberId: player.PlayerCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        return reply.IsPlayerExist ? reply.Balance : 0;
    }
}
