using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.SBOTransfer.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/sbotf")]
public class SBOTransferController : ControllerBase
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
    ///     Post Sportsbook
    ///         {
    ///             "gameType": 1,
    ///             "gameId": 4800001,
    ///             "brandCode": "GPI",
    ///             "playerCode": "4ucmqplxb029",
    ///             "currencyCode": "CNY",
    ///             "loginRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///             "platformType": 0,
    ///             "language": "zh-CN",
    ///             "ipAddress": "127.0.0.1",
    ///             "backUrl": "https://dev-t1.rpgp.cc/"
    ///         }
    ///
    ///     Post VirtualSportsLobby
    ///         {
    ///             "gameType": 3,
    ///             "gameId": 4800002,
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
    public async ValueTask<GameUrlResponse> GameUrlAsync(
        [FromServices] IGetService<SBOTransferGameUrlQuery, Uri> service,
        GameUrlRequest request)
    {
        var query = new SBOTransferGameUrlQuery()
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
        };

        var gameUrl = await service.GetAsync(query).ConfigureAwait(false);

        return new GameUrlResponse()
        {
            Url = gameUrl
        };
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
        [FromServices] ICommandService<SBOTransferKickPlayerCommand> service,
        string playerCode)
        => service.ExecuteAsync(new()
        {
            PlayerCode = playerCode
        });

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
    public ValueTask<decimal?> PlayerBalanceAsync(
         [FromServices] IGetService<SBOTransferPlayerBalanceQuery, decimal?> service,
         string playerCode)
        => service.GetAsync(new()
        {
            PlayerCode = playerCode
        });

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
    [HttpPost("PlayerFundChecking")]
    public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
        [FromServices] IGetService<SBOTransferPlayerFundCheckingQuery, PlayerFundCheckingResult> service,
        PlayerFundCheckingRequest request)
    {
        var query = new SBOTransferPlayerFundCheckingQuery()
        {
            BrandCode = request.BrandCode,
            PlayerCode = request.PlayerCode,
            CurrencyCode = request.CurrencyCode,
            OrderId = request.OrderId
        };
        var result = await service.GetAsync(query).ConfigureAwait(false);

        return result == null ? null : new PlayerFundCheckingResponse()
        {
            OrderStatus = result.OrderStatus,
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
    [HttpPost("PlayerFundIn")]
    public async ValueTask<PlayerFundResponse?> PlayerFundInAsync(
        [FromServices] IGetService<SBOTransferPlayerFundInQuery, PlayerFundResult> service,
        PlayerFundRequest request)
    {
        var query = new SBOTransferPlayerFundInQuery()
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
    [HttpPost("PlayerFundOut")]
    public async ValueTask<PlayerFundResponse?> PlayerFundOutAsync(
        [FromServices] IGetService<SBOTransferPlayerFundOutQuery, PlayerFundResult> service,
        PlayerFundRequest request)
    {
        var query = new SBOTransferPlayerFundOutQuery()
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
    [HttpGet("RoundDetail/{roundId}")]
    public async ValueTask<RoundDetailResult?> RoundDetailAsync(
        [FromServices] IGetService<SBOTransferRoundDetailQuery, RoundDetailResult> service,
        Guid roundId,
        [FromQuery] string language)
    {
        var query = new SBOTransferRoundDetailQuery()
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
