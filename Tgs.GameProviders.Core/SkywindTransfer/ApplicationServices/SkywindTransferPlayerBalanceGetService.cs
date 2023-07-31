using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;

public class SkywindTransferPlayerBalanceGetService : IGetService<SkywindTransferPlayerBalanceQuery, PlayerBalanceResult>
{
    private readonly IGameInterfaceSkywindClient _client;
    private readonly ICommandService<SkywindTransferCreatePlayerCommand> _createPlayerCommandService;
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

    public async ValueTask<PlayerBalanceResult> GetAsync(SkywindTransferPlayerBalanceQuery query, CancellationToken cancellationToken = default)
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

        var reply = await _client
                .PlayerBalanceAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

        if (!reply.IsPlayer)
        {
            await _createPlayerCommandService.ExecuteAsync(new SkywindTransferCreatePlayerCommand(
                Configuration: configuration,
                PlayerCode: player.PlayerCode,
                CurrencyCode: player.DefaultWalletCurrencyCode),
                cancellationToken).ConfigureAwait(false);

            reply = await _client
                .PlayerBalanceAsync(
                request: request,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        return !reply.IsPlayer
            ? throw new Exception($"Player does not exist.{player.PlayerCode}")
            : new PlayerBalanceResult
            {
                FreeBalance = reply.Balance.ToDecimal().GetValueOrDefault(0)
            };
    }
}
