using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.MessageQueue;
using TGS.GameProviders.GameRecord.Core.JobWorkerCommand.TCG;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecord.ReceiverServer.ReceiveHandler
{
    internal class TCGGameRecordReceiveHandler : IMessageHandler
    {
        private readonly ICommandService<TCGJobWorkerCommand> _commandService;
        private readonly ILogger<TCGGameRecordReceiveHandler> _logger;

        public TCGGameRecordReceiveHandler(
            ILogger<TCGGameRecordReceiveHandler> logger,
            ICommandService<TCGJobWorkerCommand> commandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data);
            var command = JsonSerializer.Deserialize<TCGJobWorkerCommand>(json);
            _logger.Log(LogLevel.Information,
                 new EventId(1000, nameof(TCGGameRecordReceiveHandler)),
                 command,
                 null,
                 (state, ex) => json);
            await _commandService.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
