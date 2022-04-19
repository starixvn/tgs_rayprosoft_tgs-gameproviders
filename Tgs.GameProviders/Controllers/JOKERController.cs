using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Joker.ApplicationServices;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JOKERController : ControllerBase,
    IGameUrlHandler<JokerGameUrlQuery>,
    IKickPlayerHandler<JokerKickPlayerCommand>,
    IPlayerBalanceHandler<JokerPlayerBalanceQuery>,
    IPlayerFundHandler<JokerPlayerFundInQuery, JokerPlayerFundOutQuery>,
    IPlayerFundCheckingHandler<JokerPlayerFundCheckingQuery, PlayerFundCheckingRequest>
{
    private readonly IMapper _mapper;

    public JOKERController(
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
    ///             "gameType": 15,
    ///             "gameId": 2600001,
    ///             "brandCode": "GPI_HKD",
    ///             "playerCode": "4uf6k014jp5x",
    ///             "currencyCode": "HKD",
    ///             "loginRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///             "platformType": 0,
    ///             "language": "zh-CN",
    ///             "ipAddress": "127.0.0.1",
    ///             "backUrl": "https://dev-t1.rpgp.cc/"
    ///         }
    /// </remarks>
    [HttpPost("GameUrl")]
    public async ValueTask<GameUrlResponse?> GameUrlAsync(
        [FromServices] IGetService<JokerGameUrlQuery, GameUrlResult> getService,
        GameUrlRequest request)
    {
        var result = await getService
            .GetAsync(_mapper.Map<JokerGameUrlQuery>(request))
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
    ///     Delete joker/kickplayer/4uf6k014jp5x
    /// </remarks>
    [HttpDelete("kickplayer/{playerCode}")]
    public ValueTask KickPlayerAsync(
        [FromServices] ICommandService<JokerKickPlayerCommand> commandService,
        string playerCode)
        => commandService.ExecuteAsync(new JokerKickPlayerCommand
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
    ///   Get joker/balance/4uf6k014jp5x
    /// </remarks>
    [HttpGet("Balance/{playerCode}")]
    public ValueTask<decimal?> PlayerBalanceAsync(
        [FromServices] IGetService<JokerPlayerBalanceQuery, decimal?> getService,
        string playerCode)
        => getService.GetAsync(new JokerPlayerBalanceQuery
        {
            PlayerCode = playerCode
        });

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
    ///             "brandCode": "GPI_HKD",
    ///             "playerCode": "4uf6k014jp5x",
    ///             "currencyCode": "HKD",
    ///             "orderId":"397FE4DF0A2FF0E2820367F0E29CFCAC8F86",
    ///             "amount": 1
    ///         }
    /// </remarks>
    [HttpPost("PlayerFundIn")]
    public async ValueTask<PlayerFundResponse?> PlayerFundInAsync(
        [FromServices] IGetService<JokerPlayerFundInQuery, PlayerFundResult> getService,
        PlayerFundRequest request)
    {
        var result = await getService.GetAsync(_mapper.Map<JokerPlayerFundInQuery>(request)).ConfigureAwait(false);
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
    ///             "brandCode": "GPI_HKD",
    ///             "playerCode": "4uf6k014jp5x",
    ///             "currencyCode": "HKD",
    ///             "orderId":"397FE4DF0A2FF0E2820367F0E29CFCAC8F86",
    ///             "amount": 1
    ///         }
    /// </remarks>
    [HttpPost("PlayerFundOut")]
    public async ValueTask<PlayerFundResponse?> PlayerFundOutAsync(
        [FromServices] IGetService<JokerPlayerFundOutQuery, PlayerFundResult> getService,
        PlayerFundRequest request)
    {
        var result = await getService.GetAsync(_mapper.Map<JokerPlayerFundOutQuery>(request)).ConfigureAwait(false);
        return result == null ? null : _mapper.Map<PlayerFundResponse>(result);
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
    ///     Post
    ///         {
    ///             "brandCode": "GPI>HKD",
    ///             "playerCode": "4uf6k014jp5x",
    ///             "currencyCode": "HKD",
    ///             "orderId":"397FE4DF0A2FF0E2820367F0E29CFCAC8F86"
    ///         }
    /// </remarks>
    [HttpPost("PlayerFundChecking")]
    public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
        [FromServices] IGetService<JokerPlayerFundCheckingQuery, PlayerFundCheckingResult> getService,
        PlayerFundCheckingRequest request)
    {
        var result = await getService.GetAsync(_mapper.Map<JokerPlayerFundCheckingQuery>(request)).ConfigureAwait(false);
        return result == null ? null : _mapper.Map<PlayerFundCheckingResponse>(result);
    }
}