using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.AllBet.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ABController : ControllerBase,
    IGameUrlHandler<AllBetGameUrlQuery>,
    IKickPlayerHandler<AllBetKickPlayerCommand>,
    IRoundDetailHandler<AllBetRoundDetailQuery>
{
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
    ///             "gameType": 6,
    ///             "gameId": 1300001,
    ///             "brandCode": "GPI",
    ///             "playerCode": "4ucmqplxb029",
    ///             "currencyCode": "CNY",
    ///             "loginRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///             "platformType": 0,
    ///             "language": "zh-CN",
    ///             "ipAddress": "127.0.0.1",
    ///             "backUrl": "https://dev-t1.rpgp.cc/"
    ///         }
    /// </remarks>
    [HttpPost("gameurl")]
    public async ValueTask<GameUrlResponse?> GameUrlAsync(
        [FromServices] IGetService<AllBetGameUrlQuery, GameUrlResult> getService,
        GameUrlRequest request)
    {
        var result = await getService
            .GetAsync(
                query: new()
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
                    BackUrl = request.BackUrl
                },
                cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);
        return result == null ? null : new GameUrlResponse() { Url = result.Url };
    }

    /// <summary>
    /// Kicks the player asynchronous.
    /// </summary>
    /// <param name="commandService">The command service.</param>
    /// <param name="playerCode">The player code.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Delete kickplayer/4ucmqplxb029
    /// </remarks>
    [HttpDelete("kickplayer/{playerCode}")]
    public ValueTask KickPlayerAsync(
        [FromServices] ICommandService<AllBetKickPlayerCommand> commandService,
        string playerCode)
        => commandService
        .ExecuteAsync(
            command: new()
            {
                PlayerCode = playerCode
            },
            cancellationToken: HttpContext.RequestAborted);

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
    [HttpGet("rounddetail/{roundId:guid}")]
    public ValueTask<RoundDetailResult?> RoundDetailAsync(
        [FromServices] IGetService<AllBetRoundDetailQuery, RoundDetailResult?> getService,
        Guid roundId,
        [FromQuery] string language)
        => getService
        .GetAsync(
            query: new()
            {
                RoundId = roundId,
                Language = language
            },
            cancellationToken: HttpContext.RequestAborted);
}
