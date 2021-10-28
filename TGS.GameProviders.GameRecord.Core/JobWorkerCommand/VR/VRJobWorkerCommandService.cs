using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.VR;

namespace TGS.GameProviders.GameRecord.Core.JobWorkerCommand.VR
{
    internal class VRJobWorkerCommandService : ICommandService<VRJobWorkerCommand>
    {
        private readonly IGetService<VRGameRecordHandlerQuery, VRGameRecordHandlerResult> _getService;
        private readonly ILogger<VRJobWorkerCommandService> _logger;
        private readonly IVRService _service;

        public VRJobWorkerCommandService(
            IVRService service,
            IGetService<VRGameRecordHandlerQuery, VRGameRecordHandlerResult> getService,
            ILogger<VRJobWorkerCommandService> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        async ValueTask ICommandService<VRJobWorkerCommand>.ExecuteAsync(VRJobWorkerCommand command, CancellationToken cancellationToken)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var brandMerchants = configuration.BrandMerchants.Where(x => x.Status);

            foreach (var brand in configuration.BrandMerchants.Where(x => x.Status))
            {
                try
                {
                    var gameProviderSchedule = await _service.GetGameProviderScheduleAsync(gameProviderId, brand.BrandCode, brand.Merchantid).ConfigureAwait(false);
                    if (gameProviderSchedule == null)
                    {
                        gameProviderSchedule = await _service.GameProviderScheduleInitAsync(gameProviderId, brand.BrandCode, brand.Merchantid).ConfigureAwait(false);
                    }

                    var sysTime = DateTimeOffset.UtcNow;
                    while (gameProviderSchedule != null)
                    {
                        var config = _service.GetGameProviderScheduleConfig(gameProviderSchedule);
                        if (config.EndTime >= sysTime)
                        {
                            break;
                        }
                        var result = await _getService.GetAsync(new VRGameRecordHandlerQuery()
                        {
                            BrandCode = brand.BrandCode,
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
}
