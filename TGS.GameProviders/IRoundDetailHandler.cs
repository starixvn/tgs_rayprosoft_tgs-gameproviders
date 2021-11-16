using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders
{
    public interface IRoundDetailHandler<TQuery> where TQuery : RoundDetailQuery
    {
        ValueTask<RoundDetailResult?> RoundDetailAsync([FromServices] IGetService<TQuery, RoundDetailResult> getService, Guid roundId, [FromQuery] string language);
    }
}
