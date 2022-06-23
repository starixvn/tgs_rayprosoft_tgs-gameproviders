using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices
{
    internal class WMCasinoTransferCreatePlayerCommandService : ICommandService<WMCasinoTransferCreatePlayerCommand>
    {
        private readonly IGameInterfaceWMCasinoServiceGrpc _grpc;

        public WMCasinoTransferCreatePlayerCommandService(
            IGameInterfaceWMCasinoServiceGrpc grpc)
        {
            _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
        }

        public async ValueTask ExecuteAsync(WMCasinoTransferCreatePlayerCommand command, CancellationToken cancellationToken = default)
        {
            await _grpc.CreatePlayerAsync(
                apiUrl: command.Configuration.ApiUrl,
                vendorId: command.ConfigurationData.VendorId,
                signature: command.ConfigurationData.Signature,
                playerCode: command.PlayerCode,
                password: command.Password,
                userName: command.PlayerCode,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}