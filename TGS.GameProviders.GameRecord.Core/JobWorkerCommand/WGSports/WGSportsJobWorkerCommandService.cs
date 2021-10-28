using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.WGSports;

namespace TGS.GameProviders.GameRecord.Core.JobWorkerCommand.WGSports
{
    internal class WGSportsJobWorkerCommandService : ICommandService<WGSportsJobWorkerCommand>
    {
        private readonly IGetService<WGSportsGameRecordHandlerQuery, WGSportsGameRecordHandlerResult> _getService;
        private readonly ILogger<WGSportsJobWorkerCommandService> _logger;
        private readonly IWGSportsService _service;

        public WGSportsJobWorkerCommandService(
            IWGSportsService service,
            IGetService<WGSportsGameRecordHandlerQuery, WGSportsGameRecordHandlerResult> getService,
            ILogger<WGSportsJobWorkerCommandService> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        async ValueTask ICommandService<WGSportsJobWorkerCommand>.ExecuteAsync(WGSportsJobWorkerCommand command, CancellationToken cancellationToken)
        {
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            try
            {
                var gameProviderSchedule = await _service.GetGameProviderScheduleAsync(gameProviderId).ConfigureAwait(false);
                if (gameProviderSchedule == null)
                {
                    gameProviderSchedule = await _service.GameProviderScheduleInitAsync(gameProviderId).ConfigureAwait(false);
                }

                var sysTime = DateTimeOffset.UtcNow;
                while (gameProviderSchedule != null)
                {
                    var config = _service.GetGameProviderScheduleConfig(gameProviderSchedule);

                    if (config.EndTime >= sysTime)
                    {
                        break;
                    }

                    var result = await _getService.GetAsync(new WGSportsGameRecordHandlerQuery()
                    {
                        StartTime = config.StartTime,
                        EndTime = config.EndTime
                    }).ConfigureAwait(false);

                    if (result.Success)
                    {
                        gameProviderSchedule = await _service.GameProviderScheduleUpdateAsync(gameProviderSchedule, result.LastGameRecorTime, sysTime).ConfigureAwait(false);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"發生例外");
            }
        }
    }
}
