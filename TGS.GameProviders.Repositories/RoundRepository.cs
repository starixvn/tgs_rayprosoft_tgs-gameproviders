using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Rounds;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    internal class RoundRepository : IRoundRepository
    {
        private readonly BetContext _context;
        private readonly IMapper _mapper;

        public RoundRepository(
            BetContext context,
            IMapper mapper
            )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async ValueTask<RoundModel?> GetRoundAsync(Guid RoundId)
        {
            var round = await _context.Rounds.FirstOrDefaultAsync(x => x.Id == RoundId).ConfigureAwait(false);
            if (round == null)
            {
                return null;
            }
            var roundDetail = await _context.RoundDetails.FirstOrDefaultAsync(x => x.RoundId == round.Id).ConfigureAwait(false);

            var data = _mapper.Map<RoundModel>(round);
            if (roundDetail != null)
            {
                data.AdditionalData = roundDetail!.AdditionalData;
            }
            return data;
        }

        public async ValueTask<RoundModel?> GetRoundAsync(Guid gameProviderId, string externalRoundId, int gameId)
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

            var data = _mapper.Map<RoundModel>(round);

            if (roundDetail != null)
            {
                data.AdditionalData = roundDetail!.AdditionalData;
            }

            return data;
        }

        public async ValueTask<RoundModel?> GetRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, string playerCode)
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

            var data = _mapper.Map<RoundModel>(round);

            if (roundDetail != null)
            {
                data.AdditionalData = roundDetail!.AdditionalData;
            }

            return data;
        }

        public async ValueTask<string?> GetRoundDetails(Guid roundId)
        {
            var roundDetail = await _context.RoundDetails.FirstOrDefaultAsync(x => x.RoundId == roundId).ConfigureAwait(false);

            return roundDetail?.AdditionalData;
        }
    }
}
