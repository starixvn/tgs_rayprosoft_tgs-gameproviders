using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.WGHorse;
using TGS.GameProviders.RedisRepositories;
using TGS.GameProviders.RedisRepositories.Models;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecord.Core.JobWorkerCommand.WGHorse
{
    internal class WGHorseJobWorkerCommandService : ICommandService<WGHorseJobWorkerCommand>
    {
        private readonly IGameProviderScheduleLockRepository _gameProviderScheduleLockRepository;
        private readonly IGetService<WGHorseGameRecordHandlerQuery, WGHorseGameRecordHandlerResult> _getService;
        private readonly ILogger<WGHorseJobWorkerCommandService> _logger;
        private readonly IWGHorseService _service;

        public WGHorseJobWorkerCommandService(
            IWGHorseService service,
            IGetService<WGHorseGameRecordHandlerQuery, WGHorseGameRecordHandlerResult> getService,
            IGameProviderScheduleLockRepository gameProviderScheduleLockRepository,
            ILogger<WGHorseJobWorkerCommandService> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
            _gameProviderScheduleLockRepository = gameProviderScheduleLockRepository ?? throw new ArgumentNullException(nameof(gameProviderScheduleLockRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        async ValueTask ICommandService<WGHorseJobWorkerCommand>.ExecuteAsync(WGHorseJobWorkerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

                var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

                var tasks = new List<Task>();

                var gameProviderScheduleId = string.Empty;
                try
                {
                    var gameProviderSchedule = await _service.GetGameProviderScheduleAsync(gameProviderId).ConfigureAwait(false);
                    if (gameProviderSchedule == null)
                    {
                        gameProviderSchedule = await _service.GameProviderScheduleInitAsync(gameProviderId).ConfigureAwait(false);
                    }
                    if (!await _gameProviderScheduleLockRepository.IsLockAsync(_service.GetGameProviderCode(), gameProviderSchedule.Id.ToString()).ConfigureAwait(false))
                    {
                        await _gameProviderScheduleLockRepository.SetAsync(
                            _service.GetGameProviderCode(),
                            gameProviderSchedule.Id.ToString(),
                            new GameProviderScheduleLockData()
                            {
                                GameProviderCode = _service.GetGameProviderCode(),
                                GameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false),
                                GameProviderScheduleId = gameProviderSchedule.Id,
                                LockDate = DateTimeOffset.UtcNow
                            }).ConfigureAwait(false);
                        var sysTime = DateTimeOffset.UtcNow;
                        while (gameProviderSchedule != null)
                        {
                            var config = _service.GetGameProviderScheduleConfig(gameProviderSchedule);

                            if (config.EndTime >= sysTime)
                            {
                                break;
                            }

                            var result = await _getService.GetAsync(new WGHorseGameRecordHandlerQuery()
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
                        await _gameProviderScheduleLockRepository.RemoveAsync(_service.GetGameProviderCode(), gameProviderSchedule.Id.ToString()).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    if (!string.IsNullOrEmpty(gameProviderScheduleId))
                    {
                        await _gameProviderScheduleLockRepository.RemoveAsync(_service.GetGameProviderCode(), gameProviderScheduleId).ConfigureAwait(false);
                    }
                    _logger.LogError(ex, $"發生例外");
                }
            }
            catch (Exception e)
            {
                await _gameProviderScheduleLockRepository.RemoveGameProviderCodeAsync(_service.GetGameProviderCode()).ConfigureAwait(false);
            }
        }
    }
}
