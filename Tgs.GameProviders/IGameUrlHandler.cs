using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders;

public interface IGameUrlHandler<TQuery> where TQuery : GameUrlQuery
{
    [HttpPost("GameUrl")]
    ValueTask<GameUrlResponse?> GameUrlAsync([FromServices] IGetService<TQuery, GameUrlResult> getService, GameUrlRequest request);
}
