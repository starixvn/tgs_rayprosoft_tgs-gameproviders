using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.MessageQueue;
using TGS.GameProviders.GameRecordReceiver.JobWorkerCommand.KY;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecordReceiver.ReceiveHandler
{
    internal class KYGameRecordReceiveHandler : IMessageHandler
    {
        private readonly ILogger<KYGameRecordReceiveHandler> _logger;
        private ICommandService<KYJobWorkerCommand> _commandService;

        public KYGameRecordReceiveHandler(
            ILogger<KYGameRecordReceiveHandler> logger,
            ICommandService<KYJobWorkerCommand> commandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data);
            var command = JsonSerializer.Deserialize<KYJobWorkerCommand>(json);
            _logger.Log(LogLevel.Information,
                 new EventId(1000, nameof(KYGameRecordReceiveHandler)),
                 command,
                 null,
                 (state, ex) => json);
            await _commandService.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
