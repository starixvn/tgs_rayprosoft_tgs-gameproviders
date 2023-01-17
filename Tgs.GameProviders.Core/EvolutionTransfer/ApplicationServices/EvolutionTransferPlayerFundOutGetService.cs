using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.EvolutionTransfer.ApplicationServices;

public class EvolutionTransferPlayerFundOutGetService : IGetService<EvolutionTransferPlayerFundOutQuery, PlayerFundResult>
{
    private readonly IGameInterfaceEvolutionClient _grpc;
    private readonly IPlayerRepository _player;
    private readonly ISettingRepository _setting;

    public EvolutionTransferPlayerFundOutGetService(
        IGameInterfaceEvolutionClient grpc,
        IPlayerRepository player,
        ISettingRepository setting)
    {
        _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
    }

    public async ValueTask<PlayerFundResult> GetAsync(EvolutionTransferPlayerFundOutQuery command, CancellationToken cancellationToken = default)
    {
        var configuration = await _setting
            .GetEvolutionTransferConfigurationAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var player = await _player
            .GetAsync(
                playerCode: command.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var reply = await _grpc.FundOutAsync(
            configuration.ApiUrl,
            configuration.EcId,
            player.PlayerCode,
            command.OrderId,
            command.Amount,
            cancellationToken).ConfigureAwait(false);

        return (reply.Result) ? new() { OrderId = reply.TransactionId } : null;
    }
}
