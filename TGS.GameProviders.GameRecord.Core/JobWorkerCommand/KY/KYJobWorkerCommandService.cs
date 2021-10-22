using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.Ky;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecord.Core.JobWorkerCommand.KY
{
    internal class KYJobWorkerCommandService : ICommandService<KYJobWorkerCommand>
    {
        private readonly IGetService<KYGameRecordHandlerQuery, KYGameRecordHandlerResult> _getService;
        private readonly ILogger<KYJobWorkerCommandService> _logger;
        private readonly IKYService _service;

        public KYJobWorkerCommandService(
            IKYService service,
            IGetService<KYGameRecordHandlerQuery, KYGameRecordHandlerResult> getService,
            ILogger<KYJobWorkerCommandService> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        async ValueTask ICommandService<KYJobWorkerCommand>.ExecuteAsync(KYJobWorkerCommand command, CancellationToken cancellationToken)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var brandMerchants = configuration.BrandMerchants.Where(x => x.Status);

            foreach (var brand in brandMerchants)
            {
                foreach (var agent in brand.AgentMappers.Where(x => x.Status))
                {
                    try
                    {
                        var gameProviderSchedule = await _service.GetGameProviderScheduleAsync(gameProviderId, brand.BrandCode, agent.AgentId, agent.CurrencyCode).ConfigureAwait(false);
                        if (gameProviderSchedule == null)
                        {
                            gameProviderSchedule = await _service.GameProviderScheduleInitAsync(gameProviderId, brand.BrandCode, agent.AgentId, agent.CurrencyCode).ConfigureAwait(false);
                        }

                        var sysTime = DateTimeOffset.UtcNow;

                        while (gameProviderSchedule != null)
                        {
                            var config = _service.GetGameProviderScheduleConfig(gameProviderSchedule);
                            if (config.EndTime >= sysTime)
                            {
                                break;
                            }
                            var result = await _getService.GetAsync(new KYGameRecordHandlerQuery()
                            {
                                BrandCode = brand.BrandCode,
                                CurrencyCode = agent.CurrencyCode,
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
}
