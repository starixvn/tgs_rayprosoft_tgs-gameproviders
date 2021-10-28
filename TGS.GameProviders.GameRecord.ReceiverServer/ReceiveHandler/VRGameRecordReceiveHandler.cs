using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;
using TGS.GameProviders.GameRecord.Core.JobWorkerCommand.VR;

namespace TGS.GameProviders.GameRecord.ReceiverServer.ReceiveHandler
{
    internal class VRGameRecordReceiveHandler : IMessageHandler
    {
        private readonly ILogger<VRGameRecordReceiveHandler> _logger;
        private ICommandService<VRJobWorkerCommand> _commandService;

        public VRGameRecordReceiveHandler(
            ILogger<VRGameRecordReceiveHandler> logger,
            ICommandService<VRJobWorkerCommand> commandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data);
            var command = JsonSerializer.Deserialize<VRJobWorkerCommand>(json);
            _logger.Log(LogLevel.Information,
                 new EventId(1000, nameof(VRGameRecordReceiveHandler)),
                 command,
                 null,
                 (state, ex) => json);
            await _commandService.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
