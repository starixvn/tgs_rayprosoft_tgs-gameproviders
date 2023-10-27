using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.AwcAws.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[Route("api/awc_aws")]
[ApiController]
public class AwcAwsController : ControllerBase
{
    /// <summary>
    /// Brands the player balances asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="brandCode">The brand code.</param>
    /// <returns></returns>
    /// <remarks>
    ///   <b>Example</b><br />
    ///
    ///   Get GPI/player/balances
    /// </remarks>
    [HttpGet("{brandCode}/player/balances")]
    public IAsyncEnumerable<BalanceResult> BrandPlayerBalancesAsync(
        [FromServices] IQueryService<AwcAwsBalancesQuery, BalanceResult> service,
        string brandCode)
        => service.QueryAsync(
            query: new(BrandCode: brandCode),
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
    public async ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(
         [FromServices] IGetService<AwcAwsPlayerBalanceQuery, PlayerBalanceResult> service,
         string playerCode)
    {
        var query = new AwcAwsPlayerBalanceQuery
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
        [FromServices] IGetService<AwcAwsPlayerFundInQuery, PlayerFundResult> getService,
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
    ///     Get rounddetail/c3011003-6357-4cff-9404-97312397a791?language=zh-CN
    /// </remarks>
    [HttpGet("rounddetail/{roundId:guid}")]
    public ValueTask<RoundDetailResult?> RoundDetailAsync(
        [FromServices] IGetService<AwcAwsRoundDetailQuery, RoundDetailResult?> getService,
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
