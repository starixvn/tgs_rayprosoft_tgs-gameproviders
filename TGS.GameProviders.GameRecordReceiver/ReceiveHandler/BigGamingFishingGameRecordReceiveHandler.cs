using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.MessageQueue;
using TGS.GameProviders.GameRecord.Core.JobWorkerCommand.BigGaming;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecordReceiver.ReceiveHandler
{
    internal class BigGamingFishingGameRecordReceiveHandler : IMessageHandler
    {
        private readonly ILogger<BigGamingFishingGameRecordReceiveHandler> _logger;
        private readonly ICommandService<BigGamingFishingJobWorkerCommand> _commandService;

        public BigGamingFishingGameRecordReceiveHandler(
            ILogger<BigGamingFishingGameRecordReceiveHandler> logger,
            ICommandService<BigGamingFishingJobWorkerCommand> commandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data);
            var command = JsonSerializer.Deserialize<BigGamingFishingJobWorkerCommand>(json);
            _logger.Log(LogLevel.Information,
                 new EventId(1000, nameof(KYGameRecordReceiveHandler)),
                 command,
                 null,
                 (state, ex) => json);
            await _commandService.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
