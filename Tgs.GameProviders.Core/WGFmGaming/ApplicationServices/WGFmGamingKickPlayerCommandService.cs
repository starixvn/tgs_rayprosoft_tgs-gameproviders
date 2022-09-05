using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;

public class WGFmGamingKickPlayerCommandService : ICommandService<WGFmGamingKickPlayerCommand>
{
    private readonly IGameInterfaceWGFmGamingClient _client;
    private readonly ISettingRepository _settingRepository;

    public WGFmGamingKickPlayerCommandService(
        IGameInterfaceWGFmGamingClient client,
        ISettingRepository settingRepository)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
    }

    public async ValueTask ExecuteAsync(WGFmGamingKickPlayerCommand command, CancellationToken cancellationToken = default)
    {
        var configuration = await _settingRepository.GetWGSportsConfigurationAsync().ConfigureAwait(false);

        await _client.PlayerKickAsync(
            configuration.ApiUrl,
            configuration.PartnerCode,
            configuration.PartnerKey,
            configuration.IV,
            command.PlayerCode,
            cancellationToken).ConfigureAwait(false);
    }
}
