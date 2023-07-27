using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.VR.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VRController : ControllerBase,
    IGameRecordListHandler<VRGameRecordListQuery, VRGameRecordListQuery>,
    IGameUrlHandler<VRGameUrlQuery>,
    IKickPlayerHandler<VRKickPlayerCommand>,
    IPlayerFundCheckingHandler<VRPlayerFundCheckingQuery, VRPlayerFundCheckingRequest>,
    IPlayerFundHandler<VRPlayerFundInQuery, VRPlayerFundOutQuery>,
    IRoundDetailHandler<VRRoundDetailQuery>
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
    ///             "startTime": "2021-10-28T03:50:00+08:00",
    ///             "endTime": "2021-10-28T04:00:00+08:00",
    ///             "isUpdateTime": true,
    ///             "page": 1,
    ///             "rows": 500
    ///         }
    /// </remarks>
    [HttpPost("gamerecordlist")]
    public async ValueTask<GameRecordListResult?> GameRecordListAsync(
        [FromServices] IGetService<VRGameRecordListQuery, GameRecordListResult> getService,
        VRGameRecordListQuery request)
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
    ///             "gameType": 14,
    ///             "gameId": 1100001,
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
        [FromServices] IGetService<VRGameUrlQuery, GameUrlResult> getService,
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
        [FromServices] ICommandService<VRKickPlayerCommand> commandService,
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
    /// <param name="service">The service.</param>
    /// <param name="playerCode">The player code.</param>
    /// <returns></returns>
    /// <remarks>
    ///   <b>Example</b><br />
    ///
    ///   Get balance/4ucmqplxb029
    /// </remarks>
    [HttpGet("balance/{playerCode}")]
    public async ValueTask<decimal> PlayerBalanceAsync(
         [FromServices] IGetService<VRPlayerBalanceQuery, PlayerBalanceResult> service,
         string playerCode)
    {
        var query = new VRPlayerBalanceQuery
        {
            PlayerCode = playerCode
        };

        var result = await service
         .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
         .ConfigureAwait(false);

        return result.FreeBalance;
    }

    /// <summary>
    /// Players the fund checking asynchronous.
    /// </summary>
    /// <param name="getService">The get service.</param>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///         Post
    ///             {
    ///                 "brandCode": "GPI",
    ///                 "playerCode": "4ucmqplxb029",
    ///                 "currencyCode": "CNY",
    ///                 "orderId": "D07B3FAD0BE5F0B4EE0C2A401FC5C41075ED",
    ///                 "startTime": "2021-10-27T00:00:00.000Z",
    ///                 "endTime": "2021-10-28T00:00:00.000Z"
    ///             }
    /// </remarks>
    [HttpPost("playerfundchecking")]
    public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
        [FromServices] IGetService<VRPlayerFundCheckingQuery, PlayerFundCheckingResult> getService,
        VRPlayerFundCheckingRequest request)
    {
        var result = await getService
            .GetAsync(
                query: new()
                {
                    BrandCode = request.BrandCode,
                    PlayerCode = request.PlayerCode,
                    CurrencyCode = request.CurrencyCode,
                    OrderId = request.OrderId,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime
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
    ///             "currencyCode": "CNY",
    ///             "orderId":"397FE4DF0A2FF0E2820367F0E29CFCAC8F86",
    ///             "amount": 1
    ///         }
    /// </remarks>
    [HttpPost("playerfundin")]
    public async ValueTask<PlayerFundResponse?> PlayerFundInAsync(
        [FromServices] IGetService<VRPlayerFundInQuery, PlayerFundResult> getService,
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
    ///             "currencyCode": "CNY",
    ///             "orderId":"D07B3FAD0BE5F0B4EE0C2A401FC5C41075ED",
    ///             "amount": 1
    ///         }
    /// </remarks>
    [HttpPost("playerfundout")]
    public async ValueTask<PlayerFundResponse?> PlayerFundOutAsync(
        [FromServices] IGetService<VRPlayerFundOutQuery, PlayerFundResult> getService,
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
    ///     Get rounddetail/1fe63a42-2e7a-4ddf-17cf-08d9a5803086?language=zh-CN
    /// </remarks>
    [HttpGet("rounddetail/{roundId:guid}")]
    public ValueTask<RoundDetailResult?> RoundDetailAsync(
        [FromServices] IGetService<VRRoundDetailQuery, RoundDetailResult?> getService,
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
