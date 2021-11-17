using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders
{
    public interface IGameUrlHandler<TQuery> where TQuery : GameUrlQuery
    {
        [HttpPost("GameUrl")]
        ValueTask<GameUrlResponse?> GameUrlAsync([FromServices] IGetService<TQuery, GameUrlResult> getService, GameUrlRequest request);
    }
}
