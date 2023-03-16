using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.LY.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LYController : ControllerBase,
    IGameRecordListHandler<LYGameRecordListQuery, LYGameRecordListQuery>,
    IGameUrlHandler<LYGameUrlQuery>,
    IKickPlayerHandler<LYKickPlayerCommand>,
    IPlayerBalanceHandler<LYPlayerBalanceQuery>,
    IPlayerFundCheckingHandler<LYPlayerFundCheckingQuery, PlayerFundCheckingRequest>,
    IPlayerFundHandler<LYPlayerFundInQuery, LYPlayerFundOutQuery>,
    IRoundDetailHandler<LYRoundDetailQuery>

{
    /// <summary>
    /// Games the record list asynchronous.
    /// </summary>
    /// <param name="getService">The get service.</param>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///         {
    ///             "brandCode": "Default",
    ///             "currencyCode": "HKD",
    ///             "startTime": "2021-11-02T03:36:03.1588192+00:00",
    ///             "endTime": "2021-11-02T03:46:03.1588192+00:00"
    ///         }
    /// </remarks>
    [HttpPost("gamerecordlist")]
    public async ValueTask<GameRecordListResult?> GameRecordListAsync(
        [FromServices] IGetService<LYGameRecordListQuery, GameRecordListResult> getService,
        LYGameRecordListQuery request)
    {
        var result = await getService.GetAsync(request).ConfigureAwait(false);
        return result ?? null;
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
    ///             "gameType": 8,
    ///             "gameId": 2000001,
    ///             "brandCode": "GPI",
    ///             "playerCode": "4ucmqplxb029",
    ///             "currencyCode": "HKD",
    ///             "loginRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///             "platformType": 0,
    ///             "language": "zh-CN",
    ///             "ipAddress": "127.0.0.1",
    ///             "backUrl": "https://dev-t1.rpgp.cc/"
    ///         }
    /// </remarks>
    [HttpPost("gameurl")]
    public async ValueTask<GameUrlResponse?> GameUrlAsync(
        [FromServices] IGetService<LYGameUrlQuery, GameUrlResult> getService,
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
        [FromServices] ICommandService<LYKickPlayerCommand> commandService,
        string playerCode)
        => commandService
        .ExecuteAsync(
            command: new()
            {
                PlayerCode = playerCode
            },
            cancellationToken: HttpContext.RequestAborted);

    /// <summary>
    /// Players the balance asynchronous.
    /// </summary>
    /// <param name="getService">The get service.</param>
    /// <param name="playerCode">The player code.</param>
    /// <returns></returns>
    /// <remarks>
    ///   <b>Example</b><br />
    ///
    ///   Get balance/4ucmqplxb029
    /// </remarks>
    [HttpGet("balance/{playerCode}")]
    public ValueTask<decimal?> PlayerBalanceAsync(
         [FromServices] IGetService<LYPlayerBalanceQuery, decimal?> getService,
         string playerCode)
        => getService
        .GetAsync(
            query: new()
            {
                PlayerCode = playerCode
            },
            cancellationToken: HttpContext.RequestAborted);

    /// <summary>
    /// Players the fund checking asynchronous.
    /// </summary>
    /// <param name="getService">The get service.</param>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///         {
    ///             "brandCode": "GPI",
    ///             "playerCode": "4ucmqplxb029",
    ///             "currencyCode": "HKD",
    ///             "orderId":"72412202110271546265054ucmqplxb029"
    ///         }
    /// </remarks>
    [HttpPost("playerfundchecking")]
    public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
        [FromServices] IGetService<LYPlayerFundCheckingQuery, PlayerFundCheckingResult> getService,
        PlayerFundCheckingRequest request)
    {
        var result = await getService
            .GetAsync(
                query: new()
                {
                    BrandCode = request.BrandCode,
                    PlayerCode = request.PlayerCode,
                    CurrencyCode = request.CurrencyCode,
                    OrderId = request.OrderId,
                },
                cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return result == null ? null : new PlayerFundCheckingResponse()
        {
            OrderStatus = result.OrderStatus,
        };
    }

    /// <summary>
    /// Players the fund in asynchronous.
    /// </summary>
    /// <param name="getService">The get service.</param>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///         {
    ///             "brandCode": "GPI",
    ///             "playerCode": "4ucmqplxb029",
    ///             "currencyCode": "HKD",
    ///             "orderId":"397FE4DF0A2FF0E2820367F0E29CFCAC8F86",
    ///             "amount": 1
    ///         }
    /// </remarks>
    [HttpPost("playerfundin")]
    public async ValueTask<PlayerFundResponse?> PlayerFundInAsync(
        [FromServices] IGetService<LYPlayerFundInQuery, PlayerFundResult> getService,
        PlayerFundRequest request)
    {
        var result = await getService
            .GetAsync(
                query: new()
                {
                    BrandCode = request.BrandCode,
                    PlayerCode = request.PlayerCode,
                    CurrencyCode = request.CurrencyCode,
                    OrderId = request.OrderId,
                    Amount = request.Amount,
                },
                cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);
        return result == null ? null : new PlayerFundResponse()
        {
            OrderId = request.OrderId
        };
    }

    /// <summary>
    /// Players the fund out asynchronous.
    /// </summary>
    /// <param name="getService">The get service.</param>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///         {
    ///             "brandCode": "GPI",
    ///             "playerCode": "4ucmqplxb029",
    ///             "currencyCode": "HKD",
    ///             "orderId":"D07B3FAD0BE5F0B4EE0C2A401FC5C41075ED",
    ///             "amount": 1
    ///         }
    /// </remarks>
    [HttpPost("playerfundout")]
    public async ValueTask<PlayerFundResponse?> PlayerFundOutAsync(
        [FromServices] IGetService<LYPlayerFundOutQuery, PlayerFundResult> getService,
        PlayerFundRequest request)
    {
        var result = await getService
            .GetAsync(
                query: new()
                {
                    BrandCode = request.BrandCode,
                    PlayerCode = request.PlayerCode,
                    CurrencyCode = request.CurrencyCode,
                    OrderId = request.OrderId,
                    Amount = request.Amount,
                },
                cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);
        return result == null ? null : new PlayerFundResponse()
        {
            OrderId = request.OrderId
        };
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
    ///     Get rounddetail/b53d5576-8f61-417d-17da-08d9a5803086?language=zh-CN
    /// </remarks>
    [HttpGet("rounddetail/{roundId:guid}")]
    public ValueTask<RoundDetailResult?> RoundDetailAsync(
        [FromServices] IGetService<LYRoundDetailQuery, RoundDetailResult?> getService,
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
