using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.GameRecordReceiver.ApplicationServices.TCG;
using TGS.GameProviders.RedisRepositories;
using TGS.GameProviders.RedisRepositories.Models;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecordReceiver.JobWorkerCommand.TCG
{
    internal class TCGJobWorkerCommandService : ICommandService<TCGJobWorkerCommand>
    {
        private readonly IGameProviderScheduleLockRepository _gameProviderScheduleLockRepository;
        private readonly ITCGService _service;
        private IGetService<TCGGameRecordHandlerQuery, TCGGameRecordHandlerResult> _getService;

        public TCGJobWorkerCommandService(
            ITCGService service,
            IGetService<TCGGameRecordHandlerQuery, TCGGameRecordHandlerResult> getService,
            IGameProviderScheduleLockRepository gameProviderScheduleLockRepository)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
            _gameProviderScheduleLockRepository = gameProviderScheduleLockRepository ?? throw new ArgumentNullException(nameof(gameProviderScheduleLockRepository));
        }

        async ValueTask ICommandService<TCGJobWorkerCommand>.ExecuteAsync(TCGJobWorkerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
                var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
                var gameProviderCode = _service.GetGameProviderCode();
                var gameLotteryLobby = await _service.GetGameLotteryLobbyAsync(gameProviderId).ConfigureAwait(false);

                if (gameLotteryLobby == null)
                {
                    return;
                }
                var tasks = new List<Task>();
                foreach (var game in gameLotteryLobby)
                {
                    var merchants = configuration.Merchants;
                    if (game.AvailableCurrencies.Length > 0)
                    {
                        merchants = (from merchant in merchants
                                     join currency in game.AvailableCurrencies
                                     on merchant.CurrencyCode equals currency
                                     select merchant).ToArray();
                    }
                    if (merchants == null)
                    {
                        continue;
                    }

                    foreach (var merchant in merchants)
                    {
                        Task task = Task.Run(async () =>
                        {
                            var gameProviderSchedule = await _service.GetGameProviderScheduleAsync(gameProviderId, game.Code, merchant.CurrencyCode, merchant.MerchantCode).ConfigureAwait(false);
                            if (gameProviderSchedule == null)
                            {
                                gameProviderSchedule = await _service.GameProviderScheduleInitAsync(gameProviderId, game.Code, merchant.CurrencyCode, merchant.MerchantCode).ConfigureAwait(false);
                            }
                            var gameProviderScheduleId = gameProviderSchedule.Id.ToString("N");
                            if (!await _gameProviderScheduleLockRepository.IsLockAsync(gameProviderCode, gameProviderScheduleId).ConfigureAwait(false))
                            {
                                await _gameProviderScheduleLockRepository.SetAsync(
                                    gameProviderCode,
                                    gameProviderScheduleId,
                                    new GameProviderScheduleLockData()
                                    {
                                        GameProviderCode = gameProviderCode,
                                        GameProviderId = gameProviderId,
                                        GameProviderScheduleId = gameProviderSchedule.Id,
                                        LockDate = DateTimeOffset.UtcNow
                                    }).ConfigureAwait(false);

                                var config = _service.GetGameProviderScheduleConfig(gameProviderSchedule);
                                (string subGP, string merchantCode) = _service.GetSubGPAndMerchantCode(gameProviderSchedule.AgentId);
                                var result = await _getService.GetAsync(new()
                                {
                                    SubGP = subGP,
                                    MerchantCode = merchantCode,
                                    IsDesignation = false,
                                    LastGameRecorDirectory = config.LastGameRecorDirectory,
                                    LastGameRecorFile = config.LastGameRecorFile
                                }).ConfigureAwait(false);

                                if (result.Success)
                                {
                                    await _service.GameProviderScheduleUpdateAsync(gameProviderSchedule, result.LastGameRecorDirectory, result.LastGameRecorFile).ConfigureAwait(false);
                                }
                                await _gameProviderScheduleLockRepository.RemoveAsync(gameProviderCode, gameProviderScheduleId).ConfigureAwait(false);
                            }
                        });
                        tasks.Add(task);
                    }
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await _gameProviderScheduleLockRepository.RemoveGameProviderCodeAsync(_service.GetGameProviderCode()).ConfigureAwait(false);
            }
        }
    }
}
