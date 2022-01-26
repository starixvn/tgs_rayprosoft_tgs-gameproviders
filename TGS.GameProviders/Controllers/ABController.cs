using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.AllBet.ApplicationServices;
using TGS.GameProviders.Domains.RoundDetail;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ABController : ControllerBase,
    IGameUrlHandler<AllBetGameUrlQuery>,
    IKickPlayerHandler<AllBetKickPlayerCommand>,
    IRoundDetailHandler<AllBetRoundDetailQuery>
{
    private readonly ILogger<ABController> _logger;
    private readonly IMapper _mapper;

    public ABController(
        ILogger<ABController> logger,
        IMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
    ///             "gameType": 6,
    ///             "gameId": 1300001,
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
        [FromServices] IGetService<AllBetGameUrlQuery, GameUrlResult> getService,
        GameUrlRequest request)
    {
        var result = await getService
            .GetAsync(_mapper.Map<AllBetGameUrlQuery>(request))
            .ConfigureAwait(false);

        return result == null ?
            null :
            _mapper.Map<GameUrlResponse>(result);
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
    ///     Delete ab/kickplayer/4ucmqplxb029
    /// </remarks>
    [HttpDelete("kickplayer/{playerCode}")]
    public ValueTask KickPlayerAsync(
        [FromServices] ICommandService<AllBetKickPlayerCommand> commandService,
        string playerCode)
        => commandService.ExecuteAsync(new AllBetKickPlayerCommand
        {
            PlayerCode = playerCode
        });

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
    ///     Get ab/rounddetail/330256a1-1553-ec11-a743-70b5e859fe4d?language=zh-CN
    /// </remarks>
    [HttpGet("RoundDetail/{roundId}")]
    public async ValueTask<RoundDetailResult?> RoundDetailAsync(
        [FromServices] IGetService<AllBetRoundDetailQuery, RoundDetailResult> getService,
        Guid roundId,
        [FromQuery] string language)
        => await getService
            .GetAsync(new AllBetRoundDetailQuery
            {
                RoundId = roundId,
                Language = language
            })
            .ConfigureAwait(false);
}