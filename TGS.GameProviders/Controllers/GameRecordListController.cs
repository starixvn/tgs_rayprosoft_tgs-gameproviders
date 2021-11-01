using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;

namespace TGS.GameProviders.Controllers
{
    [ApiController]
    [Route("api/gamerecordlist/[controller]")]
    public abstract class GameRecordListController<TQuery, TQueryViewModel> : ControllerBase
    {
        [HttpPost]
        public abstract ValueTask<GameRecordListResult?> GameRecordListAsync(
            [FromServices] IGetService<TQuery, GameRecordListResult> getService,
            TQueryViewModel request);
    }
}
