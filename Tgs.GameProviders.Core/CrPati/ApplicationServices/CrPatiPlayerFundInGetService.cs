using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.CrPati.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.CrPati.ApplicationServices;

public class CrPatiPlayerFundInGetService : IGetService<CrPatiPlayerFundInQuery, PlayerFundResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceCrPatiClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPAddressOptions _iPAddress;
    private readonly IPlayerRepository _player;

    public CrPatiPlayerFundInGetService(
        IOptions<IPAddressOptions> options,
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IBrandRepository brand,
        IGameInterfaceCrPatiClient client)
    {
        _iPAddress = options.Value;
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<PlayerFundResult> GetAsync(CrPatiPlayerFundInQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.CrPati,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var player = await _player.GetAsync(
            playerCode: query.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: gameProviderId,
            brandId: player.BrandId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var configuration = await _gameProvider.GetCrPatiConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var reply = await _client.FundInAsync(
            apiUrl: configuration.ApiUrl,
            apiKey: configuration.ApiKey,
            apiSecret: configuration.ApiSecret,
            playerCode: player.PlayerCode,
            ipAddress: _iPAddress.IPAddress,
            orderId: query.OrderId,
            amount: query.Amount,
            cancellationToken: cancellationToken)
           .ConfigureAwait(false);

        return new PlayerFundResult
        {
            OrderId = reply.ApiOrderNo
        };
    }
}
