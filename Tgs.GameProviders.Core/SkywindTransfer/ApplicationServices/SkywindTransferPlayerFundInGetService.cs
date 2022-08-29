using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;

public class SkywindTransferPlayerFundInGetService : IGetService<SkywindTransferPlayerFundInQuery, PlayerFundResult>
{
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameInterfaceSkywindClient _client;
    private readonly ISettingRepository _setting;

    public SkywindTransferPlayerFundInGetService(
        IGameProviderRepository gameProvider,
        IGameInterfaceSkywindClient client,
        ISettingRepository setting)
    {
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
    }

    public async ValueTask<PlayerFundResult> GetAsync(SkywindTransferPlayerFundInQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.SkywindTransfer).ConfigureAwait(false);
        var configuration = await _setting.GetSkywindTransferConfigurationAsync().ConfigureAwait(false);
        var gameProviderCurrencyCode = await _gameProvider
                .GetGameProviderCurrencyCodeAsync(gameProviderId, query.CurrencyCode)
                .ConfigureAwait(false)
            ?? throw new Exception($"This game product currency does not exist.({query.CurrencyCode})");

        var request = new FundRequest()
        {
            ApiUrl = configuration.ApiUrl.ToString(),
            SecretKey = configuration.SecretKey,
            Username = configuration.Username,
            Password = configuration.Password,
            ExtTrxId = query.OrderId,
            PlayerCode = query.PlayerCode,
            Currency = gameProviderCurrencyCode,
            Amount = query.Amount,
        };

        var reply = await _client
                    .FundInAsync(
                        request: request,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

        return (reply.Status) ? new() { OrderId = reply.ExtTrxId } : null;
    }
}
