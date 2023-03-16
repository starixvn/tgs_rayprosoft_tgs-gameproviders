using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders;

public interface IRoundDetailHandler<TQuery> where TQuery : RoundDetailQuery
{
    [HttpGet("rounddetail/{roundId:guid}")]
    ValueTask<RoundDetailResult?> RoundDetailAsync([FromServices] IGetService<TQuery, RoundDetailResult?> getService, Guid roundId, [FromQuery] string language);
}
