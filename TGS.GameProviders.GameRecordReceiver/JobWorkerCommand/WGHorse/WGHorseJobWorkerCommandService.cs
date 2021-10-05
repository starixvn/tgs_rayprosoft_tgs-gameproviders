using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.GameRecordReceiver.ApplicationServices.WGHorse;
using TGS.GameProviders.RedisRepositories;
using TGS.GameProviders.RedisRepositories.Models;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecordReceiver.JobWorkerCommand.WGHorse
{
    internal class WGHorseJobWorkerCommandService : ICommandService<WGHorseJobWorkerCommand>
    {
        private readonly IGameProviderScheduleLockRepository _gameProviderScheduleLockRepository;
        private readonly IWGHorseService _service;
        private IGetService<WGHorseGameRecordHandlerQuery, WGHorseGameRecordHandlerResult> _getService;

        public WGHorseJobWorkerCommandService(
            IWGHorseService service,
            IGetService<WGHorseGameRecordHandlerQuery, WGHorseGameRecordHandlerResult> getService,
            IGameProviderScheduleLockRepository gameProviderScheduleLockRepository)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
            _gameProviderScheduleLockRepository = gameProviderScheduleLockRepository ?? throw new ArgumentNullException(nameof(gameProviderScheduleLockRepository));
        }

        async ValueTask ICommandService<WGHorseJobWorkerCommand>.ExecuteAsync(WGHorseJobWorkerCommand command, CancellationToken cancellationToken)
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

                            var responses = await _getService.GetAsync(new WGHorseGameRecordHandlerQuery()
                            {
                                StartTime = config.StartTime,
                                EndTime = config.EndTime
                            }).ConfigureAwait(false);

                            if (responses.Success)
                            {
                                gameProviderSchedule = await _service.GameProviderScheduleUpdateAsync(gameProviderSchedule, responses.LastGameRecorTime, sysTime).ConfigureAwait(false);
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
