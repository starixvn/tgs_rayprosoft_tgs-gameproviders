using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SabaTransfer.ApplicationServices;

public class SabaTransferPlayerFundOutGetService : IGetService<SabaTransferPlayerFundOutQuery, PlayerFundResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceSabaClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public SabaTransferPlayerFundOutGetService(
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

    public async ValueTask<PlayerFundResult> GetAsync(SabaTransferPlayerFundOutQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.SabaTransfer).ConfigureAwait(false);
        var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetSabaTransferConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);
        var gameProviderCurrencyCode = await _gameProvider
                    .GetGameProviderCurrencyCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode)
                    .ConfigureAwait(false)
                    ?? throw new Exception($"This game product currency does not exist.({player.DefaultWalletCurrencyCode})");

        var reply = await _client
                .FundOutAsync(
                    apiUrl: configuration.ApiUrl,
                    operatorId: configuration.OperatorId,
                    vendorId: configuration.VendorId,
                    vendorMemberId: player.PlayerCode,
                    vendorTransId: query.OrderId,
                    currency: int.Parse(gameProviderCurrencyCode),
                    amount: query.Amount,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

        if (!reply.IsOrderExist)
        {
            if (reply.Check)
            {
                throw new Exception($"請呼叫遊戲廠商提供CheckFundTransfer.({JsonSerializer.Serialize(reply)})");
            }
            throw new Exception($"遊戲廠商交易失敗.({JsonSerializer.Serialize(reply)})");
        }
        return new PlayerFundResult()
        {
            OrderId = reply.ActualVendorTransId
        };
    }
}
