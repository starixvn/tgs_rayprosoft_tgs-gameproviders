using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders
{
    public interface IGameRecordListHandler<TQuery, TQueryViewModel>
    {
        [HttpPost("GameRecordList")]
        ValueTask<GameRecordListResult?> GameRecordListAsync([FromServices] IGetService<TQuery, GameRecordListResult> getService, TQueryViewModel request);
    }
}
