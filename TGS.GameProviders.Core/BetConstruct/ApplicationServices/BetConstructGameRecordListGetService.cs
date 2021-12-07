using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Core.BetConstruct.ApplicationServices
{
    public class BetConstructGameRecordListGetService : IGetService<BetConstructGameRecordListQuery, GameRecordListResult>
    {
        private readonly ILogger<BetConstructGameRecordListGetService> _logger;

        public BetConstructGameRecordListGetService(
            ILogger<BetConstructGameRecordListGetService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ValueTask<GameRecordListResult> GetAsync(BetConstructGameRecordListQuery query, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}