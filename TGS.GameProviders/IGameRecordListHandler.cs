using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;

namespace TGS.GameProviders
{
    public interface IGameRecordListHandler<TQuery, TQueryViewModel>
    {
        [HttpPost("GameRecordList")]
        ValueTask<GameRecordListResult?> GameRecordListAsync([FromServices] IGetService<TQuery, GameRecordListResult> getService, TQueryViewModel request);
    }
}
