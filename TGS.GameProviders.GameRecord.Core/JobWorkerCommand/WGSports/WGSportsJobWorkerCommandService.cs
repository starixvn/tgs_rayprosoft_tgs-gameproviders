using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.WGSports;
using TGS.GameProviders.RedisRepositories;
using TGS.GameProviders.RedisRepositories.Models;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecord.Core.JobWorkerCommand.WGSports
{
    internal class WGSportsJobWorkerCommandService : ICommandService<WGSportsJobWorkerCommand>
    {
        private readonly IGameProviderScheduleLockRepository _gameProviderScheduleLockRepository;
        private readonly IGetService<WGSportsGameRecordHandlerQuery, WGSportsGameRecordHandlerResult> _getService;
        private readonly IWGHorseService _service;

        public WGSportsJobWorkerCommandService(
            IWGHorseService service,
            IGetService<WGSportsGameRecordHandlerQuery, WGSportsGameRecordHandlerResult> getService,
            IGameProviderScheduleLockRepository gameProviderScheduleLockRepository)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
            _gameProviderScheduleLockRepository = gameProviderScheduleLockRepository ?? throw new ArgumentNullException(nameof(gameProviderScheduleLockRepository));
        }

        async ValueTask ICommandService<WGSportsJobWorkerCommand>.ExecuteAsync(WGSportsJobWorkerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

                var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

                var tasks = new List<Task>();

                Task t = Task.Run(async () =>
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
                        await _gameProviderScheduleLockRepository.RemoveAsync(_service.GetGameProviderCode(), gameProviderSchedule.Id.ToString()).ConfigureAwait(false);
                    }
                });
                tasks.Add(t);
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                await _gameProviderScheduleLockRepository.RemoveGameProviderCodeAsync(_service.GetGameProviderCode()).ConfigureAwait(false);
            }
        }
    }
}
