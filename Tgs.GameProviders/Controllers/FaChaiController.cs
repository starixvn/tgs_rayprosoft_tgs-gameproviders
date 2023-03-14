using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.FaChai.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[Route("api/fachai")]
[ApiController]
public class FaChaiController : ControllerBase
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
    ///             "gameType": 16,
    ///             "gameId": 3000001,
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
    [HttpPost("GameUrl")]
    public async ValueTask<GameUrlResponse?> GameUrlAsync(
        [FromServices] IGetService<FaChaiGameUrlQuery, Uri> getService,
        GameUrlRequest request)
    {
        var result = await getService.GetAsync(new FaChaiGameUrlQuery()
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
        }).ConfigureAwait(false);
        return result == null ? null : new GameUrlResponse() { Url = result };
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
        [FromServices] ICommandService<FaChaiKickPlayerCommand> commandService,
        string playerCode)
        => commandService.ExecuteAsync(new()
        {
            PlayerCode = playerCode
        });

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
    [HttpGet("Balance/{playerCode}")]
    public ValueTask<decimal?> PlayerBalanceAsync(
         [FromServices] IGetService<FaChaiPlayerBalanceQuery, decimal?> getService,
         string playerCode)
        => getService.GetAsync(new()
        {
            PlayerCode = playerCode
        });

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
    ///             "currencyCode": "CNY",
    ///             "orderId":"7a7147218501000000010000af506b2c"
    ///         }
    /// </remarks>
    [HttpPost("PlayerFundChecking")]
    public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
        [FromServices] IGetService<FaChaiPlayerFundCheckingQuery, PlayerFundCheckingResult> getService,
        PlayerFundCheckingRequest request)
    {
        var result = await getService.GetAsync(new()
        {
            BrandCode = request.BrandCode,
            PlayerCode = request.PlayerCode,
            CurrencyCode = request.CurrencyCode,
            OrderId = request.OrderId,
        }).ConfigureAwait(false);
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
    ///             "orderId":"7a3f35bf271247b998f6750d448ed757",
    ///             "amount": 100
    ///         }
    /// </remarks>
    [HttpPost("PlayerFundIn")]
    public async ValueTask<PlayerFundResponse?> PlayerFundInAsync(
        [FromServices] IGetService<FaChaiPlayerFundInQuery, PlayerFundResult> getService,
        PlayerFundRequest request)
    {
        var result = await getService.GetAsync(new()
        {
            BrandCode = request.BrandCode,
            PlayerCode = request.PlayerCode,
            CurrencyCode = request.CurrencyCode,
            OrderId = request.OrderId,
            Amount = request.Amount,
        }).ConfigureAwait(false);
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
    ///             "orderId":"7a7147218501000000010000af506b2c",
    ///             "amount": 1000
    ///         }
    /// </remarks>
    [HttpPost("PlayerFundOut")]
    public async ValueTask<PlayerFundResponse?> PlayerFundOutAsync(
        [FromServices] IGetService<FaChaiPlayerFundOutQuery, PlayerFundResult> getService,
        PlayerFundRequest request)
    {
        var result = await getService.GetAsync(new()
        {
            BrandCode = request.BrandCode,
            PlayerCode = request.PlayerCode,
            CurrencyCode = request.CurrencyCode,
            OrderId = request.OrderId,
            Amount = request.Amount,
        }).ConfigureAwait(false);
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
    ///     Get rounddetail/c3011003-6357-4cff-9404-97312397a791?language=zh-CN
    /// </remarks>
    [HttpGet("RoundDetail/{roundId}")]
    public async ValueTask<RoundDetailResult?> RoundDetailAsync(
        [FromServices] IGetService<FaChaiRoundDetailQuery, RoundDetailResult?> getService,
        Guid roundId,
        [FromQuery] string language)
        => await getService
        .GetAsync(new FaChaiRoundDetailQuery
        {
            RoundId = roundId,
            Language = language
        })
        .ConfigureAwait(false);
}
