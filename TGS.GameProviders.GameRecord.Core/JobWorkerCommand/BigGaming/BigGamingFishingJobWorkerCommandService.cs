using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.BigGaming;

namespace TGS.GameProviders.GameRecord.Core.JobWorkerCommand.BigGaming
{
    internal class BigGamingFishingJobWorkerCommandService : ICommandService<BigGamingFishingJobWorkerCommand>
    {
        private readonly IGetService<BigGamingFishingGameRecordHandlerQuery, BigGamingFishingGameRecordHandlerResult> _getService;
        private readonly ILogger<BigGamingFishingJobWorkerCommandService> _logger;
        private readonly IBigGamingService _service;

        public BigGamingFishingJobWorkerCommandService(
            IBigGamingService service,
            IGetService<BigGamingFishingGameRecordHandlerQuery, BigGamingFishingGameRecordHandlerResult> getService,
            ILogger<BigGamingFishingJobWorkerCommandService> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        async ValueTask ICommandService<BigGamingFishingJobWorkerCommand>.ExecuteAsync(BigGamingFishingJobWorkerCommand command, CancellationToken cancellationToken)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var tasks = new List<Task>();

            var brandMerchants = configuration.BrandMerchants.Where(x => x.Status);

            foreach (var brand in brandMerchants)
            {
                try
                {
                    var gameProviderSchedule = await _service.GetGameProviderScheduleAsync(gameProviderId, brand.CurrencyCode).ConfigureAwait(false);
                    if (gameProviderSchedule == null)
                    {
                        gameProviderSchedule = await _service.GameProviderScheduleInitAsync(gameProviderId, brand.CurrencyCode).ConfigureAwait(false);
                    }

                    var sysTime = DateTimeOffset.UtcNow;

                    while (gameProviderSchedule != null)
                    {
                        var config = _service.GetGameProviderScheduleConfig(gameProviderSchedule);
                        if (config.EndTime >= sysTime)
                        {
                            break;
                        }
                        var result = await _getService.GetAsync(
                            new BigGamingFishingGameRecordHandlerQuery()
                            {
                                CurrencyCode = gameProviderSchedule.CurrencyCode,
                                StartTime = config.StartTime,
                                EndTime = config.EndTime
                            })
                        .ConfigureAwait(false);
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
}
