using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGame.ApplicationServices;

internal class DreamGameMarkGameCommandService : ICommandService<DreamGameMarkGameCommand>
{
    private readonly IDreamGameApiClient _client;
    private readonly IDreamGameService _service;

    public DreamGameMarkGameCommandService(
        IDreamGameApiClient client,
        IDreamGameService service
        )
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(DreamGameMarkGameCommand command, CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

        await _client.MarkReportAsync(configuration.ApiUrl, configuration.ApiKey, configuration.AgentName, command.externalRoundIds).ConfigureAwait(false);
    }
}
