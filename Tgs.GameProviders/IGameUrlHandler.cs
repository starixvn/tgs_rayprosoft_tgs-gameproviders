using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders;

public interface IGameUrlHandler<TQuery> where TQuery : GameUrlQuery
{
    [HttpPost("gameurl")]
    ValueTask<GameUrlResponse?> GameUrlAsync([FromServices] IGetService<TQuery, GameUrlResult> getService, GameUrlRequest request);
}
