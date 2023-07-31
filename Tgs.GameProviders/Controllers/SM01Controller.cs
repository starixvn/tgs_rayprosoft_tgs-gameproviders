using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.SM01.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/sm01")]
public class SM01Controller : ControllerBase
{
    /// <summary>
    /// Platforms the login asynchronous.
    /// </summary>
    /// <param name="getService">The get service.</param>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    /// <remarks>
    ///   <b>Example</b><br />
    ///
    ///   POST
    ///     {
    ///         "playerCode": "4ucmqplxb029"
    ///     }
    /// </remarks>
    [HttpPost("gameurl")]
    public async ValueTask<SM01PlatformLoginResponse?> PlatformLoginAsync(
        [FromServices] IGetService<SM01PlatformLoginQuery, SM01PlatformLoginInfo> getService,
        GameSM01PlatformLoginRequest request)
    {
        var query = new SM01PlatformLoginQuery
        {
            PlayerCode = request.PlayerCode
        };

        var result = await getService
            .GetAsync(query)
            .ConfigureAwait(false);

        return result == null
            ? null
            : new SM01PlatformLoginResponse(
                result.ApiUrl,
                result.MerchantId,
                result.CustomerName,
                result.Timestamp,
                result.Sign);
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
         [FromServices] IGetService<SM01PlayerBalanceQuery, PlayerBalanceResult> service,
         string playerCode)
    {
        var query = new SM01PlayerBalanceQuery
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
    public async ValueTask<PlayerFundResponse?> PlayerFundInAsync(
        [FromServices] IGetService<SM01PlayerFundInQuery, PlayerFundResult> service,
        PlayerFundRequest request)
    {
        var query = new SM01PlayerFundInQuery()
        {
            BrandCode = request.BrandCode,
            PlayerCode = request.PlayerCode,
            CurrencyCode = request.CurrencyCode,
            OrderId = request.OrderId,
            Amount = request.Amount
        };
        var result = await service.GetAsync(query).ConfigureAwait(false);

        return result == null ? null : new PlayerFundResponse()
        {
            OrderId = result.OrderId,
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
        [FromServices] IGetService<SM01PlayerFundOutQuery, PlayerFundResult> service,
        PlayerFundRequest request)
    {
        var query = new SM01PlayerFundOutQuery()
        {
            BrandCode = request.BrandCode,
            PlayerCode = request.PlayerCode,
            CurrencyCode = request.CurrencyCode,
            OrderId = request.OrderId,
            Amount = request.Amount
        };
        var result = await service.GetAsync(query).ConfigureAwait(false);

        return result == null ? null : new PlayerFundResponse()
        {
            OrderId = result.OrderId,
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
    public async ValueTask<RoundDetailResult?> RoundDetailAsync(
        [FromServices] IGetService<SM01RoundDetailQuery, RoundDetailResult> service,
        Guid roundId,
        [FromQuery] string language)
    {
        var query = new SM01RoundDetailQuery()
        {
            RoundId = roundId,
            Language = language
        };

        var result = await service
                .GetAsync(
                    query)
                .ConfigureAwait(false);
        return result ?? null;
    }
}
