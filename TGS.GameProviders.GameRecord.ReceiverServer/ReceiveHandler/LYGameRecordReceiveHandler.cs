using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;
using TGS.GameProviders.GameRecord.Core.JobWorkerCommand.LY;

namespace TGS.GameProviders.GameRecord.ReceiverServer.ReceiveHandler
{
    internal class LYGameRecordReceiveHandler : IMessageHandler
    {
        private readonly ICommandService<LYJobWorkerCommand> _commandService;
        private readonly ILogger<LYGameRecordReceiveHandler> _logger;

        public LYGameRecordReceiveHandler(
            ILogger<LYGameRecordReceiveHandler> logger,
            ICommandService<LYJobWorkerCommand> commandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data);
            var command = JsonSerializer.Deserialize<LYJobWorkerCommand>(json);
            _logger.Log(LogLevel.Information,
                 new EventId(1000, nameof(LYGameRecordReceiveHandler)),
                 command,
                 null,
                 (state, ex) => json);
            await _commandService.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
