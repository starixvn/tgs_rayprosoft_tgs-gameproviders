using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SabaTransfer.ApplicationServices;

public class SabaTransferPlayerFundCheckingGetService : IGetService<SabaTransferPlayerFundCheckingQuery, PlayerFundCheckingResult>
{
    private readonly IGameInterfaceSabaClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;
    private readonly IBrandRepository _brand;

    public SabaTransferPlayerFundCheckingGetService(
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

    public async ValueTask<PlayerFundCheckingResult> GetAsync(SabaTransferPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.SabaTransfer).ConfigureAwait(false);
        var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetSabaTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        var reply = await _client
                .FundStatusAsync(
                    apiUrl: configuration.ApiUrl,
                    operatorId: configuration.OperatorId,
                    vendorId: configuration.VendorId,
                    vendorTransId: query.OrderId,
                    cancellationToken)
                .ConfigureAwait(false);

        return reply.Retry
            ? throw new Exception($"請呼叫遊戲廠商提供CheckFundTransfer.({JsonSerializer.Serialize(reply)})")
            : (new() { OrderStatus = reply.IsOrderExist });
    }
}
