using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.JiliService.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JiliTransfer.ApplicationServices
{
    internal class JiliTransferCreatePlayerCommandService : ICommandService<JiliTransferCreatePlayerCommand>
    {
        private readonly IGameInterfaceJiliServiceGrpc _grpc;

        public JiliTransferCreatePlayerCommandService(
            IGameInterfaceJiliServiceGrpc grpc)
        {
            _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
        }

        public ValueTask ExecuteAsync(JiliTransferCreatePlayerCommand command, CancellationToken cancellationToken = default)
            => _grpc.CreatePlayerAsync(
                apiUrl: command.Configuration.ApiUrl,
                agentId: command.ConfigurationData.AgentId,
                agentKey: command.Configuration.AgentKey,
                playerCode: command.PlayerCode,
                cancellationToken: cancellationToken);
    }
}
