using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;

public class WGFmGamingPlayerFundCheckingGetService : IGetService<WGFmGamingPlayerFundCheckingQuery, PlayerFundCheckingResult>
{
    private readonly IGameInterfaceWGFmGamingClient _client;
    private readonly ISettingRepository _settingRepository;

    public WGFmGamingPlayerFundCheckingGetService(
        IGameInterfaceWGFmGamingClient client,
        ISettingRepository settingRepository)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
    }

    public async ValueTask<PlayerFundCheckingResult> GetAsync(WGFmGamingPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
    {
        var configuration = await _settingRepository.GetWGSportsConfigurationAsync().ConfigureAwait(false);

        var response = await _client.FundStatusAsync(
            configuration.ApiUrl,
            configuration.PartnerCode,
            configuration.PartnerKey,
            configuration.IV,
            query.OrderId,
            cancellationToken).ConfigureAwait(false);

        return new() { OrderStatus = response };
    }
}
