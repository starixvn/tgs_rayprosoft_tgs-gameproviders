using System;
using System.Collections.Generic;
using System.Linq;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.GameAction;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    internal class GameActionRepository : IGameActionRepository
    {
        private readonly BetContext _context;

        public GameActionRepository(BetContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IAsyncEnumerable<GameActionModel> GetGameActionsByRoundIdAsync(Guid roundId)
         => _context.GameActions
                .AsAsyncEnumerable()
                .Where(ga => ga.RoundId == roundId)
                .Select(ga => new GameActionModel(
                    ga.Id,
                    ga.RoundId,
                    ga.PlayerCode,
                    ga.ExternalTransactionId,
                    (GameActionType)ga.Type,
                    ga.Amount,
                    ga.Turnover,
                    ga.WinLoss,
                    ga.UnsettledBets,
                    ga.BetType,
                    ga.ExternalBetId,
                    ga.ExternalGameActionGroupId,
                    ga.ReferencedGameActionId,
                    ga.GameActionBatchId,
                    ga.RecordedOn,
                    ga.SentOn,
                    ga.ExtraData,
                    ga.JackpotContribution));
    }
}
