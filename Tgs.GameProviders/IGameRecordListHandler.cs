using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders;

public interface IGameRecordListHandler<TQuery, TQueryViewModel>
{
    [HttpPost("gamerecordlist")]
    ValueTask<GameRecordListResult?> GameRecordListAsync([FromServices] IGetService<TQuery, GameRecordListResult> getService, TQueryViewModel request);
}
