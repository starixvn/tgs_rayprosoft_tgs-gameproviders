using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PPTFController : ControllerBase,
    IGameUrlHandler<PragmaticPlayTransferGameUrlQuery>,
    IKickPlayerHandler<PragmaticPlayTransferKickPlayerCommand>,
    IPlayerBalanceHandler<PragmaticPlayTransferPlayerBalanceQuery>,
    IPlayerFundCheckingHandler<PragmaticPlayTransferPlayerFundCheckingQuery, PlayerFundCheckingRequest>,
    IPlayerFundHandler<PragmaticPlayTransferPlayerFundInQuery, PragmaticPlayTransferPlayerFundOutQuery>,
    IRoundDetailHandler<PragmaticPlayTransferRoundDetailQuery>
{
    private readonly IMapper _mapper;

    public PPTFController(IMapper mapper)
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
    ///  Post
    ///         {
    ///             "gameType": 15,
    ///             "gameId": 2700001,
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
        [FromServices] IGetService<PragmaticPlayTransferGameUrlQuery, GameUrlResult> getService,
        GameUrlRequest request)
    {
        var result = await getService
            .GetAsync(_mapper.Map<PragmaticPlayTransferGameUrlQuery>(request))
            .ConfigureAwait(false);

        return result == null ? null : _mapper.Map<GameUrlResponse>(result);
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
    ///     Delete pptf/kickplayer/4ucmqplxb029
    /// </remarks>
    [HttpDelete("kickplayer/{playerCode}")]
    public ValueTask KickPlayerAsync(
        [FromServices] ICommandService<PragmaticPlayTransferKickPlayerCommand> commandService,
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
    ///   Get pptf/balance/4ucmqplxb029
    /// </remarks>
    [HttpGet("Balance/{playerCode}")]
    public ValueTask<decimal?> PlayerBalanceAsync(
        [FromServices] IGetService<PragmaticPlayTransferPlayerBalanceQuery, decimal?> getService,
        string playerCode)
        => getService.GetAsync(new PragmaticPlayTransferPlayerBalanceQuery
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
    ///             "orderId":"4c3415e7138942e1b9b48a5fd10e1d47"
    ///         }
    /// </remarks>
    [HttpPost("PlayerFundChecking")]
    public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
        [FromServices] IGetService<PragmaticPlayTransferPlayerFundCheckingQuery, PlayerFundCheckingResult> getService,
        PlayerFundCheckingRequest request)
    {
        var result = await getService
            .GetAsync(_mapper.Map<PragmaticPlayTransferPlayerFundCheckingQuery>(request))
            .ConfigureAwait(false);

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
        [FromServices] IGetService<PragmaticPlayTransferPlayerFundInQuery, PlayerFundResult> getService,
        PlayerFundRequest request)
    {
        var result = await getService
            .GetAsync(_mapper.Map<PragmaticPlayTransferPlayerFundInQuery>(request))
            .ConfigureAwait(false);

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
    ///             "orderId":"397FE4DF0A2FF0E2820367F0E29CFCAC8F86",
    ///             "amount": 1
    ///         }
    /// </remarks>
    [HttpPost("PlayerFundOut")]
    public async ValueTask<PlayerFundResponse?> PlayerFundOutAsync(
        [FromServices] IGetService<PragmaticPlayTransferPlayerFundOutQuery, PlayerFundResult> getService,
        PlayerFundRequest request)
    {
        var result = await getService
            .GetAsync(_mapper.Map<PragmaticPlayTransferPlayerFundOutQuery>(request))
            .ConfigureAwait(false);

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
    ///     Get pptf/rounddetail/d9ca80d4-7a71-46df-341d-08d99f74a980?language=zh-CN
    /// </remarks>
    [HttpGet("RoundDetail/{roundId}")]
    public ValueTask<RoundDetailResult?> RoundDetailAsync(
        [FromServices] IGetService<PragmaticPlayTransferRoundDetailQuery, RoundDetailResult?> getService,
        Guid roundId,
        string language)
        => getService.GetAsync(new PragmaticPlayTransferRoundDetailQuery
        {
            RoundId = roundId,
            Language = language
        });
}