using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Repositories.Domains.Models.Rounds;
using TGS.GameProviders.Repositories.Entities;
using TGS.GameProviders.Repositories.ModelMappings;

namespace TGS.GameProviders.Repositories
{
    internal sealed class RoundRepository : IRoundRepository
    {
        private readonly BetContext _context;
        private readonly ILogger<RoundRepository> _logger;
        private readonly IMapper _mapper;

        public RoundRepository(
            ILogger<RoundRepository> logger,
            BetContext context
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = new MapperConfiguration(configure => configure.AddProfile<RoundMapping>()).CreateMapper();
        }

        public async Task AddRoundAsync(AddRound source)
        {
            Guid roundId = GuidExtensions.NewSequentialGuid();
            var round = _mapper.Map<Round>(source);
            round.Id = roundId;
            _context.Rounds.Add(round);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            var getRound = await _context.Rounds.FirstAsync(x => x.Id == roundId).ConfigureAwait(false);

            _context.RoundDetails.Add(new()
            {
                RoundId = getRound.Id,
                AdditionalData = source.AdditionalData
            });
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<GetRound?> GetRoundAsync(Guid RoundId)
        {
            var round = await _context.Rounds.FirstOrDefaultAsync(x => x.Id == RoundId).ConfigureAwait(false);
            if (round == null)
            {
                return null;
            }
            var roundDetail = await _context.RoundDetails.FirstOrDefaultAsync(x => x.RoundId == round.Id).ConfigureAwait(false);

            var data = _mapper.Map<GetRound>(round);

            data.AdditionalData = roundDetail!.AdditionalData;

            return data;
        }

        public async Task<GetRound?> GetRoundAsync(Guid gameProviderId, string externalRoundId, int gameId)
        {
            var round = await _context.Rounds.FirstOrDefaultAsync(x =>
            x.GameProviderId == gameProviderId
            && x.ExternalRoundId == externalRoundId
            && x.GameId == gameId).ConfigureAwait(false);
            if (round == null)
            {
                return null;
            }
            var roundDetail = await _context.RoundDetails.FirstOrDefaultAsync(x => x.RoundId == round.Id).ConfigureAwait(false);

            var data = _mapper.Map<GetRound>(round);

            data.AdditionalData = roundDetail!.AdditionalData;

            return data;
        }

        public async Task<GetRound?> GetRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, string playerCode)
        {
            var round = await _context.Rounds.FirstOrDefaultAsync(x =>
            x.GameProviderId == gameProviderId
            && x.ExternalRoundId == externalRoundId
            && x.GameId == gameId
            && x.PlayerCode == playerCode).ConfigureAwait(false);
            if (round == null)
            {
                return null;
            }
            var roundDetail = await _context.RoundDetails.FirstOrDefaultAsync(x => x.RoundId == round.Id).ConfigureAwait(false);

            var data = _mapper.Map<GetRound>(round);

            data.AdditionalData = roundDetail!.AdditionalData;

            return data;
        }

        public async Task UpdateRoundAsync(UpdateRound source)
        {
            var round = await _context.Rounds.SingleAsync(x => x.Id == source.RoundId).ConfigureAwait(false);
            round.Status = (short)source.Status;
            round.ClosedOn = source.ClosedOn.HasValue ? source.ClosedOn.Value.DateTime : null;
            round.UpdatedOn = source.UpdatedOn.DateTime;
            round.ExternalClosedOn = source.ExternalClosedOn.HasValue ? source.ExternalClosedOn.Value.DateTime : null;
            round.BetAmount = source.BetAmount;
            round.ValidBetAmount = source.ValidBetAmount;
            round.SettleAmount = source.SettleAmount;
            round.Commission = source.Commission;
            round.Turnover = source.Turnover;
            round.WinLoss = source.WinLoss;
            round.TieBetAdjustment = source.TieBetAdjustment;
            round.ClientStatus = (short)source.ClientStatus;
            round.ReopenTurnoverAdjustment = source.ReopenTurnoverAdjustment;
            round.ReopenedOn = source.ReopenedOn.HasValue ? source.ReopenedOn.Value.DateTime : null;
            var roundDetail = await _context.RoundDetails.SingleAsync(x => x.RoundId == source.RoundId).ConfigureAwait(false);
            roundDetail.AdditionalData = source.AdditionalData;
            _context.Rounds.Update(round);
            _context.RoundDetails.Update(roundDetail);

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateRoundByExternalRoundIdAsync(UpdateRoundByExternalRoundId source)
        {
            var round = await _context.Rounds.AsQueryable().SingleAsync(x =>
            x.ExternalRoundId == source.ExternalRoundId
            && x.GameProviderId == source.GameProviderId
            && x.GameId == source.GameId).ConfigureAwait(false);
            round.Status = (short)source.Status;
            round.ClosedOn = source.ClosedOn.HasValue ? source.ClosedOn.Value.DateTime : null;
            round.UpdatedOn = source.UpdatedOn.DateTime;
            round.ExternalClosedOn = source.ExternalClosedOn.HasValue ? source.ExternalClosedOn.Value.DateTime : null;
            round.BetAmount = source.BetAmount;
            round.ValidBetAmount = source.ValidBetAmount;
            round.SettleAmount = source.SettleAmount;
            round.Commission = source.Commission;
            round.Turnover = source.Turnover;
            round.WinLoss = source.WinLoss;
            round.TieBetAdjustment = source.TieBetAdjustment;
            round.ClientStatus = (short)source.ClientStatus;
            round.ReopenTurnoverAdjustment = source.ReopenTurnoverAdjustment;
            round.ReopenedOn = source.ReopenedOn.HasValue ? source.ReopenedOn.Value.DateTime : null;

            _context.Rounds.Update(round);
            _context.RoundDetails.Update(new()
            {
                RoundId = round.Id,
                AdditionalData = source.AdditionalData
            });

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateRoundDetailAsync(Guid roundId, string additionalData)
        {
            _context.RoundDetails.Update(new()
            {
                RoundId = roundId,
                AdditionalData = additionalData
            });

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
