using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Rounds;

namespace TGS.GameProviders.Core.GameRecordProcessor
{
    internal sealed class GameRecordProcessorSuccess : IGameRecordProcessorSuccess
    {
        private readonly IRoundRepository _roundRepository;

        public GameRecordProcessorSuccess(IRoundRepository roundRepository)
        {
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
        }

        public async Task ProcessorAsync(GameRoundsData data)
        {
            var round = await _roundRepository.GetRoundAsync(data.GameProviderId, data.ExternalRoundId, data.GameId, data.PlayerCode).ConfigureAwait(false);

            if (round == null)
            {
                await SuccessInsertAsync(data).ConfigureAwait(false);
            }
            else
            {
                if (round.Status == RoundStatus.Open && data.Status == RoundStatus.Closed)
                {
                    await SuccessUpdateColseAsync(round, data).ConfigureAwait(false);
                }
                else
                {
                    // 改單
                    if (round.Status == RoundStatus.Closed)
                    {
                        if (round.BetAmount != data.BetAmount ||
                            round.ValidBetAmount != data.ValidBetAmount ||
                            round.SettleAmount != data.SettleAmount ||
                            round.WinLoss != (data.PlayerWinLoss ?? 0).Negate()
                        )
                        {
                            await SuccessUpdateReopenedAsync(round, data).ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        private Task SuccessInsertAsync(GameRoundsData data)
         => _roundRepository.AddRoundAsync(new AddRound
         {
             ExternalRoundId = data.ExternalRoundId,
             GameProviderId = data.GameProviderId,
             PlayerCode = data.PlayerCode,
             GameId = data.GameId,
             CurrencyCode = data.CurrencyCode,
             Status = data.Status,
             GameProviderConfigurationId = data.GameProviderConfigurationId,
             WalletId = data.WalletId,
             CreatedOn = data.CreatedOn,
             ClosedOn = data.ClosedOn,
             UpdatedOn = data.UpdatedOn,
             BrandId = data.BrandId,
             ExternalCreatedOn = data.ExternalCreatedOn,
             ExternalClosedOn = data.ExternalClosedOn,
             BetAmount = data.BetAmount,
             ValidBetAmount = data.ValidBetAmount,
             SettleAmount = data.SettleAmount,
             Commission = data.Commission,
             Turnover = data.Turnover,
             WinLoss = (data.PlayerWinLoss ?? 0).Negate(),
             TieBetAdjustment = data.TieBetAdjustment,
             ClientStatus = data.ClientStatus,
             AdditionalData = data.AdditionalData
         });

        private Task SuccessUpdateColseAsync(GetRound round, GameRoundsData data)
         => _roundRepository.UpdateRoundAsync(new UpdateRound(
             RoundId: round.Id,
             Status: data.Status,
             ClosedOn: data.ClosedOn,
             UpdatedOn: data.UpdatedOn,
             ExternalClosedOn: data.ExternalClosedOn,
             BetAmount: data.BetAmount,
             ValidBetAmount: data.ValidBetAmount,
             SettleAmount: data.SettleAmount,
             Commission: data.Commission,
             Turnover: data.Turnover,
             WinLoss: (data.PlayerWinLoss ?? 0).Negate(),
             TieBetAdjustment: data.TieBetAdjustment,
             ClientStatus: data.ClientStatus,
             ReopenTurnoverAdjustment: null,
             ReopenedOn: null,
             AdditionalData: data.AdditionalData
             ));

        private Task SuccessUpdateReopenedAsync(GetRound round, GameRoundsData data)
            => _roundRepository.UpdateRoundAsync(new UpdateRound(
                RoundId: round.Id,
                Status: data.Status,
                ClosedOn: data.ClosedOn,
                UpdatedOn: data.UpdatedOn,
                ExternalClosedOn: data.ExternalClosedOn,
                BetAmount: data.BetAmount,
                ValidBetAmount: data.ValidBetAmount,
                SettleAmount: data.SettleAmount,
                Commission: data.Commission,
                Turnover: data.Turnover,
                WinLoss: (data.PlayerWinLoss ?? 0).Negate(),
                TieBetAdjustment: data.TieBetAdjustment,
                ClientStatus: data.ClientStatus,
                ReopenTurnoverAdjustment: null,
                ReopenedOn: DateTimeOffset.UtcNow,
                AdditionalData: data.AdditionalData
                ));
    }
}
