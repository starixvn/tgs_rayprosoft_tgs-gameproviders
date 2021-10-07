using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.MessageQueue;
using TGS.GameProviders.GameRecord.Core.JobWorkerCommand.WGSports;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecord.ReceiverServer.ReceiveHandler
{
    internal class WGSportsGameRecordReceiveHandler : IMessageHandler
    {
        private readonly ILogger<WGSportsGameRecordReceiveHandler> _logger;
        private ICommandService<WGSportsJobWorkerCommand> _commandService;

        public WGSportsGameRecordReceiveHandler(
            ILogger<WGSportsGameRecordReceiveHandler> logger,
            ICommandService<WGSportsJobWorkerCommand> commandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data);
            var command = JsonSerializer.Deserialize<WGSportsJobWorkerCommand>(json);
            _logger.Log(LogLevel.Information,
                 new EventId(1000, nameof(WGSportsGameRecordReceiveHandler)),
                 command,
                 null,
                 (state, ex) => json);
            await _commandService.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
