using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.TFGaming.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TFController : ControllerBase,
    IRoundDetailHandler<TFGamingRoundDetailQuery>,
    IKickPlayerHandler<TFKickPlayerCommand>
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
    ///             "gameType": 4,
    ///             "gameId": 2400001,
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
        [FromServices] IGetService<TFGamingGameUrlQuery, GameUrlResult> getService,
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
        [FromServices] ICommandService<TFKickPlayerCommand> commandService,
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
    ///     Get rounddetail/09eeecb8-9b64-ec11-a744-ac82478e2af1
    /// </remarks>
    [HttpGet("rounddetail/{roundId:guid}")]
    public ValueTask<RoundDetailResult?> RoundDetailAsync(
        [FromServices] IGetService<TFGamingRoundDetailQuery, RoundDetailResult?> getService,
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
