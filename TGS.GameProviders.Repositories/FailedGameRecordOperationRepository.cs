using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Repositories.Domains.Models.FailedGameRecordOperation;
using TGS.GameProviders.Repositories.Entities;
using TGS.GameProviders.Repositories.ModelMappings;

namespace TGS.GameProviders.Repositories
{
    internal sealed class FailedGameRecordOperationRepository : IFailedGameRecordOperationRepository
    {
        private readonly BetContext _context;
        private readonly ILogger<FailedGameRecordOperationRepository> _logger;
        private readonly IMapper _mapper;

        public FailedGameRecordOperationRepository(
            ILogger<FailedGameRecordOperationRepository> logger,
            BetContext context
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = new MapperConfiguration(configure => configure.AddProfile<FailedGameRecordOperationMapping>()).CreateMapper();
        }

        public async Task AddFailedGameRecordOperationAsync(AddFailedGameRecordOperation source)
        {
            var data = _mapper.Map<FailedGameRecordOperation>(source);
            data.Id = GuidExtensions.NewSequentialGuid();
            _context.FailedGameRecordOperations.Add(data);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<GetFailedGameRecordOperation> GetFailedGameRecordOperationAsync(Guid gameProviderId, string externalRoundId, int gameId, string playerCode)
        {
            var data = await _context.FailedGameRecordOperations.FirstOrDefaultAsync(x =>
                  x.GameProviderId == gameProviderId &&
                  x.ExternalRoundId == externalRoundId &&
                  x.GameId == gameId &&
                  x.PlayerCode == playerCode).ConfigureAwait(false);
            return _mapper.Map<GetFailedGameRecordOperation>(data);
        }

        public async Task UpdataFailedGameRecordOperationAsync(Guid failedGameRecordOperationId, string detail)
        {
            var data = await _context.FailedGameRecordOperations.FirstAsync(x => x.Id == failedGameRecordOperationId).ConfigureAwait(false);
            data.Detail = detail;
            _context.FailedGameRecordOperations.Update(data);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
