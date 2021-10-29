using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.FailedGameRecordOperation;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    internal class FailedGameRecordOperationRepository : IFailedGameRecordOperationRepository
    {
        private readonly BetContext _context;
        private readonly IMapper _mapper;

        public FailedGameRecordOperationRepository(
            BetContext context,
            IMapper mapper
            )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task AddFailedGameRecordOperationAsync(AddFailedGameRecordOperation source)
        {
            var data = _mapper.Map<FailedGameRecordOperation>(source);
            data.Id = GuidExtensions.NewSequentialGuid();
            _context.FailedGameRecordOperations.Add(data);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task<GetFailedGameRecordOperation> GetFailedGameRecordOperationAsync(Guid gameProviderId, string externalRoundId, int gameId, string playerCode)
            => _context.FailedGameRecordOperations
            .Where(x => x.GameProviderId == gameProviderId &&
            x.ExternalRoundId == externalRoundId &&
            x.GameId == gameId &&
            x.PlayerCode == playerCode)
            .Select(x => new GetFailedGameRecordOperation()
            {
                Id = x.Id,
                GameProviderId = x.GameProviderId,
                GameProviderCode = x.GameProviderCode,
                ExternalRoundId = x.ExternalRoundId,
                PlayerCode = x.PlayerCode,
                GameId = x.GameId,
                ExternalId = x.ExternalId,
                Detail = x.Detail,
                AutoStatus = (FailedGameRecordOperationAutoStatus)x.AutoStatus,
                ProcesStatus = (FailedGameRecordOperationProcesStatus)x.ProcesStatus,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
            })
            .FirstOrDefaultAsync();

        public async Task UpdataFailedGameRecordOperationAsync(Guid failedGameRecordOperationId, string detail)
        {
            var data = await _context.FailedGameRecordOperations.FirstAsync(x => x.Id == failedGameRecordOperationId).ConfigureAwait(false);
            data.Detail = detail;
            _context.FailedGameRecordOperations.Update(data);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
