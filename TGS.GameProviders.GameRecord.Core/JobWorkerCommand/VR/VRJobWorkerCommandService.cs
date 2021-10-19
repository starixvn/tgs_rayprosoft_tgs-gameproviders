using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.VR;
using TGS.GameProviders.RedisRepositories;
using TGS.GameProviders.RedisRepositories.Models;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecord.Core.JobWorkerCommand.VR
{
    internal class VRJobWorkerCommandService : ICommandService<VRJobWorkerCommand>
    {
        private readonly IGameProviderScheduleLockRepository _gameProviderScheduleLockRepository;
        private readonly IGetService<VRGameRecordHandlerQuery, VRGameRecordHandlerResult> _getService;
        private readonly ILogger<VRJobWorkerCommandService> _logger;
        private readonly IVRService _service;

        public VRJobWorkerCommandService(
            IVRService service,
            IGetService<VRGameRecordHandlerQuery, VRGameRecordHandlerResult> getService,
            IGameProviderScheduleLockRepository gameProviderScheduleLockRepository,
            ILogger<VRJobWorkerCommandService> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
            _gameProviderScheduleLockRepository = gameProviderScheduleLockRepository ?? throw new ArgumentNullException(nameof(gameProviderScheduleLockRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        async ValueTask ICommandService<VRJobWorkerCommand>.ExecuteAsync(VRJobWorkerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

                var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

                var brandMerchants = configuration.BrandMerchants.Where(x => x.Status);

                foreach (var brand in configuration.BrandMerchants.Where(x => x.Status))
                {
                    var gameProviderScheduleId = string.Empty;
                    try
                    {
                        var gameProviderSchedule = await _service.GetGameProviderScheduleAsync(gameProviderId, brand.BrandCode, brand.Merchantid).ConfigureAwait(false);
                        if (gameProviderSchedule == null)
                        {
                            gameProviderSchedule = await _service.GameProviderScheduleInitAsync(gameProviderId, brand.BrandCode, brand.Merchantid).ConfigureAwait(false);
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
            }
            catch (Exception e)
            {
                await _gameProviderScheduleLockRepository.RemoveGameProviderCodeAsync(_service.GetGameProviderCode()).ConfigureAwait(false);
            }
        }
    }
}
