using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Repositories.Domains.Models.FailedGameRecordOperation;
using TGS.GameProviders.Repositories.Entities;
using TGS.GameProviders.Repositories.ModelMappings;

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
            .ProjectTo<GetFailedGameRecordOperation>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x =>
                  x.GameProviderId == gameProviderId &&
                  x.ExternalRoundId == externalRoundId &&
                  x.GameId == gameId &&
                  x.PlayerCode == playerCode);

        public async Task UpdataFailedGameRecordOperationAsync(Guid failedGameRecordOperationId, string detail)
        {
            var data = await _context.FailedGameRecordOperations.FirstAsync(x => x.Id == failedGameRecordOperationId).ConfigureAwait(false);
            data.Detail = detail;
            _context.FailedGameRecordOperations.Update(data);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
