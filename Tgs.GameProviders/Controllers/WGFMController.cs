using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WGFMController : ControllerBase
{
    /// <summary>
    /// Games the URL asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///         {
    ///             "gameType": 1,
    ///             "gameId": 1500001,
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
    public async ValueTask<GameUrlResponse> GameUrlAsync(
        [FromServices] IGetService<WGFmGamingGameUrlQuery, Uri> service,
        GameUrlRequest request)
    {
        var result = await service
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
        return new GameUrlResponse() { Url = result };
    }

    /// <summary>
    /// Kicks the player asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="playerCode">The player code.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Delete kickplayer/4ucmqplxb029
    /// </remarks>
    [HttpDelete("kickplayer/{playerCode}")]
    public ValueTask KickPlayerAsync(
        [FromServices] ICommandService<WGFmGamingKickPlayerCommand> service,
        string playerCode)

    {
        var command = new WGFmGamingKickPlayerCommand
        {
            PlayerCode = playerCode
        };
        return service
        .ExecuteAsync(
            command: command,
            cancellationToken: HttpContext.RequestAborted);
    }

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
    public async ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(
        [FromServices] IGetService<WGFmGamingPlayerBalanceQuery, PlayerBalanceResult> service,
        string playerCode)
    {
        var query = new WGFmGamingPlayerBalanceQuery
        {
            PlayerCode = playerCode
        };

        var result = await service
         .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
         .ConfigureAwait(false);

        return new PlayerBalanceResponse(
            result.Balance,
            result.FreeBalance,
            result.OutstandingBalance);
    }

    /// <summary>
    /// Players the fund checking asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
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
    ///             "orderId":"397FE4DF0A2FF0E2820367F0E29CFCAC8F86"
    ///         }
    /// </remarks>
    [HttpPost("playerfundchecking")]
    public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
        [FromServices] IGetService<WGFmGamingPlayerFundCheckingQuery, bool> service,
        PlayerFundCheckingRequest request)
    {
        var query = new WGFmGamingPlayerFundCheckingQuery()
        {
            BrandCode = request.BrandCode,
            PlayerCode = request.PlayerCode,
            CurrencyCode = request.CurrencyCode,
            OrderId = request.OrderId,
        };
        var result = await service
            .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new PlayerFundCheckingResponse()
        {
            OrderStatus = result,
        };
    }

    /// <summary>
    /// Players the fund in asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
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
    public async ValueTask<PlayerFundResponse> PlayerFundInAsync(
        [FromServices] IGetService<WGFmGamingPlayerFundInQuery, PlayerFundResult> service,
        PlayerFundRequest request)
    {
        var query = new WGFmGamingPlayerFundInQuery()
        {
            BrandCode = request.BrandCode,
            PlayerCode = request.PlayerCode,
            CurrencyCode = request.CurrencyCode,
            OrderId = request.OrderId,
            Amount = request.Amount,
        };

        var result = await service
            .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new PlayerFundResponse()
        {
            OrderId = result.OrderId
        };
    }

    /// <summary>
    /// Players the fund out asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
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
        [FromServices] IGetService<WGFmGamingPlayerFundOutQuery, PlayerFundResult> service,
        PlayerFundRequest request)
    {
        var query = new WGFmGamingPlayerFundOutQuery()
        {
            BrandCode = request.BrandCode,
            PlayerCode = request.PlayerCode,
            CurrencyCode = request.CurrencyCode,
            OrderId = request.OrderId,
            Amount = request.Amount,
        };

        var result = await service
            .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new PlayerFundResponse()
        {
            OrderId = result.OrderId
        };
    }

    /// <summary>
    /// Rounds the detail asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="roundId">The round identifier.</param>
    /// <param name="language">The language.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Get rounddetail/f6dd64ee-ce12-4bef-9ddb-08d9676f4495?language=zh-CN
    /// </remarks>
    [HttpGet("rounddetail/{roundId:guid}")]
    public ValueTask<RoundDetailResult> RoundDetailAsync(
        [FromServices] IGetService<WGFmGamingRoundDetailQuery, RoundDetailResult> service,
        Guid roundId,
        [FromQuery] string language)
    {
        var query = new WGFmGamingRoundDetailQuery
        {
            RoundId = roundId,
            Language = language
        };
        return service
            .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted);
    }
}
