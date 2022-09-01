using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;

public class WGFmGamingPlayerFundInGetService : IGetService<WGFmGamingPlayerFundInQuery, PlayerFundResult>
{
    private readonly IGameInterfaceWGFmGamingClient _client;
    private readonly ISettingRepository _settingRepository;

    public WGFmGamingPlayerFundInGetService(
        IGameInterfaceWGFmGamingClient client,
        ISettingRepository settingRepository)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
    }

    public async ValueTask<PlayerFundResult> GetAsync(WGFmGamingPlayerFundInQuery query, CancellationToken cancellationToken = default)
    {
        var configuration = await _settingRepository.GetWGSportsConfigurationAsync().ConfigureAwait(false);

        await _client.FundInAsync(
           configuration.ApiUrl,
           configuration.PartnerCode,
           configuration.PartnerKey,
           configuration.IV,
           query.PlayerCode,
           query.OrderId,
           query.Amount,
           cancellationToken).ConfigureAwait(false);

        return new PlayerFundResult()
        {
            OrderId = query.OrderId
        };
    }
}
