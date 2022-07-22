using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.EvolutionTransfer.ApplicationServices
{
    public class EvolutionTransferPlayerFundCheckingGetService : IGetService<EvolutionTransferPlayerFundCheckingQuery, PlayerFundCheckingResult>
    {
        private readonly IGameInterfaceEvolutionClient _grpc;
        private readonly ISettingRepository _setting;

        public EvolutionTransferPlayerFundCheckingGetService(
            IGameInterfaceEvolutionClient grpc,
            ISettingRepository setting)
        {
            _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
            _setting = setting ?? throw new ArgumentNullException(nameof(setting));
        }

        public async ValueTask<PlayerFundCheckingResult> GetAsync(EvolutionTransferPlayerFundCheckingQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _setting.GetEvolutionTransferConfigurationAsync().ConfigureAwait(false);

            var response = await _grpc.GetTransactionInfoAsync(
                configuration.ApiUrl,
                configuration.ecID,
                command.PlayerCode,
                command.OrderId,
                cancellationToken).ConfigureAwait(false);

            return new() { OrderStatus = response.Result };
        }
    }
}
