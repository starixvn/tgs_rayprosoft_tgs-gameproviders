using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.TCG;

namespace TGS.GameProviders.GameRecord.Core.JobWorkerCommand.TCG
{
    internal class TCGJobWorkerCommandService : ICommandService<TCGJobWorkerCommand>
    {
        private readonly IGetService<TCGGameRecordHandlerQuery, TCGGameRecordHandlerResult> _getService;
        private readonly ILogger<TCGJobWorkerCommandService> _logger;
        private readonly ITCGService _service;

        public TCGJobWorkerCommandService(
            ITCGService service,
            IGetService<TCGGameRecordHandlerQuery, TCGGameRecordHandlerResult> getService,
            ILogger<TCGJobWorkerCommandService> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        async ValueTask ICommandService<TCGJobWorkerCommand>.ExecuteAsync(TCGJobWorkerCommand command, CancellationToken cancellationToken)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var gameProviderCode = _service.GetGameProviderCode();
            var gameLotteryLobby = await _service.GetGameLotteryLobbyAsync(gameProviderId).ConfigureAwait(false);

            if (gameLotteryLobby == null)
            {
                return;
            }

            foreach (var game in gameLotteryLobby)
            {
                var merchants = configuration.Merchants;
                if (game.AvailableCurrencies != null && game.AvailableCurrencies.Length > 0)
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
                    try
                    {
                        var gameProviderSchedule = await _service.GetGameProviderScheduleAsync(gameProviderId, game.Code, merchant.CurrencyCode, merchant.MerchantCode).ConfigureAwait(false);
                        if (gameProviderSchedule == null)
                        {
                            gameProviderSchedule = await _service.GameProviderScheduleInitAsync(gameProviderId, game.Code, merchant.CurrencyCode, merchant.MerchantCode).ConfigureAwait(false);
                        }
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
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"發生例外{JsonSerializer.Serialize(merchant)}");
                    }
                };
            }
        }
    }
}
