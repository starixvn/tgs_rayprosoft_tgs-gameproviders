using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Rounds;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    internal class RoundRepository : IRoundRepository
    {
        private readonly BetContext _context;

        public RoundRepository(
            BetContext context
            )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<RoundModel?> GetAsync(Guid roundId)
            => (from round in _context.Rounds.AsQueryable()
                join roundDetail in _context.RoundDetails.AsQueryable()
                on round.Id equals roundDetail.RoundId
                into roundDetailData
                from data in roundDetailData.DefaultIfEmpty()
                where round.Id == roundId
                select new RoundModel(
                    round.Id,
                    round.ExternalRoundId,
                    round.GameProviderId,
                    round.PlayerCode,
                    round.GameId,
                    round.CurrencyCode,
                    (RoundStatus)round.Status,
                    round.GameProviderConfigurationId,
                    round.WalletId,
                    round.CreatedOn,
                    round.ClosedOn,
                    round.UpdatedOn,
                    round.BrandId,
                    round.ExternalCreatedOn,
                    round.ExternalClosedOn,
                    round.BetAmount,
                    round.ValidBetAmount,
                    round.SettleAmount,
                    round.Commission,
                    round.Turnover,
                    round.WinLoss,
                    round.TieBetAdjustment,
                    (ClientRoundStatus)round.ClientStatus,
                    round.ReopenTurnoverAdjustment,
                    round.ReopenedOn,
                    round.EffectiveBettingPercentage,
                    data.AdditionalData
                    )).FirstOrDefaultAsync();

        public Task<RoundModel?> GetAsync(Guid gameProviderId, string externalRoundId, int gameId)
            => (from round in _context.Rounds.AsQueryable()
                join roundDetail in _context.RoundDetails.AsQueryable()
                on round.Id equals roundDetail.RoundId
                into roundDetailData
                from data in roundDetailData.DefaultIfEmpty()
                where round.GameProviderId == gameProviderId
                && round.ExternalRoundId == externalRoundId
                && round.GameId == gameId
                select new RoundModel(
                    round.Id,
                    round.ExternalRoundId,
                    round.GameProviderId,
                    round.PlayerCode,
                    round.GameId,
                    round.CurrencyCode,
                    (RoundStatus)round.Status,
                    round.GameProviderConfigurationId,
                    round.WalletId,
                    round.CreatedOn,
                    round.ClosedOn,
                    round.UpdatedOn,
                    round.BrandId,
                    round.ExternalCreatedOn,
                    round.ExternalClosedOn,
                    round.BetAmount,
                    round.ValidBetAmount,
                    round.SettleAmount,
                    round.Commission,
                    round.Turnover,
                    round.WinLoss,
                    round.TieBetAdjustment,
                    (ClientRoundStatus)round.ClientStatus,
                    round.ReopenTurnoverAdjustment,
                    round.ReopenedOn,
                    round.EffectiveBettingPercentage,
                    data.AdditionalData
                    )).FirstOrDefaultAsync();

        public Task<RoundModel?> GetAsync(Guid gameProviderId, string externalRoundId, int gameId, string playerCode)
            => (from round in _context.Rounds.AsQueryable()
                join roundDetail in _context.RoundDetails.AsQueryable()
                on round.Id equals roundDetail.RoundId
                into roundDetailData
                from data in roundDetailData.DefaultIfEmpty()
                where round.GameProviderId == gameProviderId
                && round.ExternalRoundId == externalRoundId
                && round.GameId == gameId
                && round.PlayerCode == playerCode
                select new RoundModel(
                    round.Id,
                    round.ExternalRoundId,
                    round.GameProviderId,
                    round.PlayerCode,
                    round.GameId,
                    round.CurrencyCode,
                    (RoundStatus)round.Status,
                    round.GameProviderConfigurationId,
                    round.WalletId,
                    round.CreatedOn,
                    round.ClosedOn,
                    round.UpdatedOn,
                    round.BrandId,
                    round.ExternalCreatedOn,
                    round.ExternalClosedOn,
                    round.BetAmount,
                    round.ValidBetAmount,
                    round.SettleAmount,
                    round.Commission,
                    round.Turnover,
                    round.WinLoss,
                    round.TieBetAdjustment,
                    (ClientRoundStatus)round.ClientStatus,
                    round.ReopenTurnoverAdjustment,
                    round.ReopenedOn,
                    round.EffectiveBettingPercentage,
                    data.AdditionalData
                    )).FirstOrDefaultAsync();
    }
}
