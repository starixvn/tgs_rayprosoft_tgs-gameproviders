using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.BetConstruct.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BCController : ControllerBase,
    IRoundDetailHandler<BetConstructRoundDetailQuery>
{
    private readonly IMapper _mapper;

    public BCController(
        IMapper mapper)
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
    ///         {
    ///             "gameType": 1,
    ///             "gameId": 2100001,
    ///             "brandCode": "GPI",
    ///             "playerCode": "4ucmqplxb029",
    ///             "currencyCode": "CNY",
    ///             "loginRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///             "platformType": 0,
    ///             "language": "zh-CN",
    ///             "ipAddress": "127.0.0.1",
    ///             "backUrl": "https://dev-t1.rpgp.cc/",
    ///             "lobbyDomain": "https://devsportsbook.srvttt.com"
    ///         }
    /// </remarks>
    [HttpPost("GameUrl")]
    public async ValueTask<GameUrlResponse?> GameUrlAsync(
        [FromServices] IGetService<BetConstructGameUrlQuery, GameUrlResult> getService,
        GameBCUrlRequest request)
    {
        var query = new BetConstructGameUrlQuery
        {
            GameType = request.GameType,
            GameId = request.GameId,
            BrandCode = request.BrandCode,
            PlayerCode = request.PlayerCode,
            CurrencyCode = request.CurrencyCode,
            LoginRecordId = request.LoginRecordId,
            PlatformType = request.PlatformType,
            Language = request.Language,
            IPAddress = request.IPAddress,
            BackUrl = request.BackUrl,
            LobbyDomain = request.LobbyDomain,
        };

        var result = await getService
            .GetAsync(query)
            .ConfigureAwait(false);

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
    ///     Get rounddetail/330256a1-1553-ec11-a743-70b5e859fe4d?language=zh-CN
    /// </remarks>
    [HttpGet("RoundDetail/{roundId}")]
    public async ValueTask<RoundDetailResult?> RoundDetailAsync(
        [FromServices] IGetService<BetConstructRoundDetailQuery, RoundDetailResult?> getService,
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
