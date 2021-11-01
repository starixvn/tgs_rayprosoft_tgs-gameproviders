using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.LY;

namespace TGS.GameProviders.GameRecord.Core.JobWorkerCommand.LY
{
    internal class LYJobWorkerCommandService : ICommandService<LYJobWorkerCommand>
    {
        private readonly ILYGameProviderScheduleService _gameProviderScheduleService;
        private readonly IGetService<LYGameRecordHandlerQuery, LYGameRecordHandlerResult> _getService;
        private readonly ILogger<LYJobWorkerCommandService> _logger;
        private readonly ILYService _service;

        public LYJobWorkerCommandService(
            ILYService service,
            IGetService<LYGameRecordHandlerQuery, LYGameRecordHandlerResult> getService,
            ILYGameProviderScheduleService gameProviderScheduleService,
            ILogger<LYJobWorkerCommandService> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
            _gameProviderScheduleService = gameProviderScheduleService ?? throw new ArgumentNullException(nameof(gameProviderScheduleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        async ValueTask ICommandService<LYJobWorkerCommand>.ExecuteAsync(LYJobWorkerCommand command, CancellationToken cancellationToken)
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
                        var gameProviderSchedule = await _gameProviderScheduleService.GetGameProviderScheduleAsync(gameProviderId, brand.BrandCode, agent.AgentId, agent.CurrencyCode).ConfigureAwait(false);
                        if (gameProviderSchedule == null)
                        {
                            gameProviderSchedule = await _gameProviderScheduleService.GameProviderScheduleInitAsync(gameProviderId, brand.BrandCode, agent.AgentId, agent.CurrencyCode).ConfigureAwait(false);
                        }

                        var sysTime = DateTimeOffset.UtcNow;

                        while (gameProviderSchedule != null)
                        {
                            var config = _gameProviderScheduleService.GetGameProviderScheduleConfig(gameProviderSchedule);
                            if (config.EndTime >= sysTime)
                            {
                                break;
                            }
                            var result = await _getService.GetAsync(new LYGameRecordHandlerQuery()
                            {
                                BrandCode = brand.BrandCode,
                                CurrencyCode = agent.CurrencyCode,
                                StartTime = config.StartTime,
                                EndTime = config.EndTime
                            }).ConfigureAwait(false);
                            if (result.Success)
                            {
                                gameProviderSchedule = await _gameProviderScheduleService.GameProviderScheduleUpdateAsync(gameProviderSchedule, result.LastGameRecorTime, sysTime).ConfigureAwait(false);
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
