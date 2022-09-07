using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;
using Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WGFMController : ControllerBase
{
    private readonly IMapper _mapper;

    public WGFMController(
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
    [HttpPost("GameUrl")]
    public async ValueTask<GameUrlResponse> GameUrlAsync(
        [FromServices] IGetService<WGFmGamingGameUrlQuery, Uri> getService,
        GameUrlRequest request)
    {
        var query = new WGFmGamingGameUrlQuery()
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

        var gameUrl = await getService.GetAsync(query).ConfigureAwait(false);

        return new GameUrlResponse()
        {
            Url = gameUrl
        };
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
        [FromServices] ICommandService<WGFmGamingKickPlayerCommand> commandService,
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
         [FromServices] IGetService<WGFmGamingPlayerBalanceQuery, decimal?> getService,
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
    ///             "orderId":"397FE4DF0A2FF0E2820367F0E29CFCAC8F86"
    ///         }
    /// </remarks>
    [HttpPost("PlayerFundChecking")]
    public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
        [FromServices] IGetService<WGFmGamingPlayerFundCheckingQuery, PlayerFundCheckingResult> getService,
        PlayerFundCheckingRequest request)
    {
        var result = await getService.GetAsync(_mapper.Map<WGFmGamingPlayerFundCheckingQuery>(request)).ConfigureAwait(false);
        return result == null ? null : _mapper.Map<PlayerFundCheckingResponse>(result);
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
    [HttpPost("PlayerFundIn")]
    public async ValueTask<PlayerFundResponse?> PlayerFundInAsync(
        [FromServices] IGetService<WGFmGamingPlayerFundInQuery, PlayerFundResult> getService,
        PlayerFundRequest request)
    {
        var result = await getService.GetAsync(_mapper.Map<WGFmGamingPlayerFundInQuery>(request)).ConfigureAwait(false);
        return result == null ? null : _mapper.Map<PlayerFundResponse>(result);
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
    [HttpPost("PlayerFundOut")]
    public async ValueTask<PlayerFundResponse?> PlayerFundOutAsync(
        [FromServices] IGetService<WGFmGamingPlayerFundOutQuery, PlayerFundResult> getService,
        PlayerFundRequest request)
    {
        var result = await getService.GetAsync(_mapper.Map<WGFmGamingPlayerFundOutQuery>(request)).ConfigureAwait(false);
        return result == null ? null : _mapper.Map<PlayerFundResponse>(result);
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
    ///     Get rounddetail/f6dd64ee-ce12-4bef-9ddb-08d9676f4495?language=zh-CN
    /// </remarks>
    [HttpGet("RoundDetail/{roundId}")]
    public async ValueTask<RoundDetailResult?> RoundDetailAsync(
        [FromServices] IGetService<WGFmGamingRoundDetailQuery, RoundDetailResult?> getService,
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
