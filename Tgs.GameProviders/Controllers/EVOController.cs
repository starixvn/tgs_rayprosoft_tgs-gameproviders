using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Evolution.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EVOController : ControllerBase,
    IGameUrlHandler<EvolutionGameUrlQuery>,
    IRoundDetailHandler<EvolutionRoundDetailQuery>
{
    private readonly IMapper _mapper;

    public EVOController(IMapper mapper)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Games the URL asynchronous.
    /// </summary>
    /// <param name="getService">The get service.</param>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///         LiveDealerLobby
    ///         {
    ///             "gameType": 6,
    ///             "gameId": 1400001,
    ///             "brandCode": "GPI",
    ///             "playerCode": "4ucmqplxb029",
    ///             "currencyCode": "CNY",
    ///             "loginRecordId": "5bf848a4-ad3a-4cfb-53f6-08d99762447f",
    ///             "platformType": 0,
    ///             "language": "zh-CN",
    ///             "ipAddress": "127.0.0.1",
    ///             "backUrl": "https://dev-t1.rpgp.cc/"
    ///         }
    ///
    ///         RT Slot
    ///         {
    ///             "gameType": 15,
    ///             "gameId": 1400225,
    ///             "brandCode": "GPI",
    ///             "playerCode": "4ucmqplxb029",
    ///             "currencyCode": "CNY",
    ///             "loginRecordId": "5bf848a4-ad3a-4cfb-53f6-08d99762447f",
    ///             "platformType": 0,
    ///             "language": "zh-CN",
    ///             "ipAddress": "127.0.0.1",
    ///             "backUrl": "https://dev-t1.rpgp.cc/"
    ///         }
    ///
    ///         NT Slot
    ///         {
    ///             "gameType": 15,
    ///             "gameId": 1400074,
    ///             "brandCode": "GPI",
    ///             "playerCode": "4ucmqplxb029",
    ///             "currencyCode": "CNY",
    ///             "loginRecordId": "5bf848a4-ad3a-4cfb-53f6-08d99762447f",
    ///             "platformType": 0,
    ///             "language": "zh-CN",
    ///             "ipAddress": "127.0.0.1",
    ///             "backUrl": "https://dev-t1.rpgp.cc/"
    ///         }
    /// </remarks>
    [HttpPost("GameUrl")]
    public async ValueTask<GameUrlResponse?> GameUrlAsync(
        [FromServices] IGetService<EvolutionGameUrlQuery, GameUrlResult> getService,
        GameUrlRequest request)
    {
        var result = await getService.GetAsync(_mapper.Map<EvolutionGameUrlQuery>(request)).ConfigureAwait(false);
        return result == null ? null : _mapper.Map<GameUrlResponse>(result);
    }

    /// <summary>
    /// Rounds the detail asynchronous.
    /// </summary>
    /// <param name="getService">The get service.</param>
    /// <param name="roundId">The round identifier.</param>
    /// <param name="language">The language.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     LiveDealer Get rounddetail/a2b1bc1f-3845-4f92-a382-08d99f799e5a?language=zh-CN
    ///
    ///     Slot Get rounddetail/a05bf300-79fa-47ca-a432-08d99f799e5a?language=zh-CN
    /// </remarks>
    [HttpGet("RoundDetail/{roundId}")]
    public async ValueTask<RoundDetailResult?> RoundDetailAsync(
        [FromServices] IGetService<EvolutionRoundDetailQuery, RoundDetailResult?> getService,
        Guid roundId,
        [FromQuery] string language)
    {
        var result = await getService.GetAsync(new()
        {
            RoundId = roundId,
            Language = language
        }).ConfigureAwait(false);
        return result ?? null;
    }
}
