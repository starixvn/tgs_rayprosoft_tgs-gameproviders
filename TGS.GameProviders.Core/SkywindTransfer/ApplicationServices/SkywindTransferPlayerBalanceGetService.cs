using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;

public class SkywindTransferPlayerBalanceGetService : IGetService<SkywindTransferPlayerBalanceQuery, decimal?>
{
    private readonly ICommandService<SkywindTransferCreatePlayerCommand> _createPlayerCommandService;
    private readonly IGameInterfaceSkywindClient _client;
    private readonly IPlayerRepository _player;
    private readonly ISettingRepository _setting;

    public SkywindTransferPlayerBalanceGetService(
        ICommandService<SkywindTransferCreatePlayerCommand> createPlayerCommandService,
        IGameInterfaceSkywindClient client,
        IPlayerRepository player,
        ISettingRepository setting)
    {
        _createPlayerCommandService = createPlayerCommandService ?? throw new ArgumentNullException(nameof(createPlayerCommandService));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
    }

    public async ValueTask<decimal?> GetAsync(SkywindTransferPlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var configuration = await _setting.GetSkywindTransferConfigurationAsync().ConfigureAwait(false);
        var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);

        var request = new PlayerBalanceRequest()
        {
            ApiUrl = configuration.ApiUrl.ToString(),
            SecretKey = configuration.SecretKey,
            Username = configuration.Username,
            Password = configuration.Password,
            PlayerCode = query.PlayerCode
        };

        var response = await _client
                .PlayerBalanceAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

        if (!response.IsPlayer)
        {
            await _createPlayerCommandService.ExecuteAsync(new SkywindTransferCreatePlayerCommand(
                Configuration: configuration,
                PlayerCode: player.PlayerCode,
                CurrencyCode: player.DefaultWalletCurrencyCode),
                cancellationToken).ConfigureAwait(false);

            response = await _client
                    .PlayerBalanceAsync(
                        request: request,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
        }

        return !response.IsPlayer
            ? throw new Exception($"This game product has failed to withdraw the balanc.")
            : response.Balance;
    }
}
