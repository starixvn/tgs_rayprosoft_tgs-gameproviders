using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

internal class PragmaticPlayTransferKickPlayerCommandService : ICommandService<PragmaticPlayTransferKickPlayerCommand>
{
    private readonly IPragmaticPlayTransferApiClient _client;
    private readonly IPragmaticPlayTransferService _service;

    public PragmaticPlayTransferKickPlayerCommandService(
        IPragmaticPlayTransferService service,
        IPragmaticPlayTransferApiClient client)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(
        PragmaticPlayTransferKickPlayerCommand command,
        CancellationToken cancellationToken = default)
    {
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        if (configuration == null)
            return;

        await _client
            .LogoutAsync(
                configuration.ApiUrl,
                configuration.SecureLogin,
                configuration.SecretKey,
                command.PlayerCode)
            .ConfigureAwait(false);
    }
}
