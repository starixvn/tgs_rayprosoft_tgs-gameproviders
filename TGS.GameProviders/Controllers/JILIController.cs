using System;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Avia.ApplicationServices;
using TGS.GameProviders.Core.Jili.ApplicationServices;
using TGS.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Controllers;

public class JILIController : ControllerBase,
    IGameUrlHandler<JiliGameUrlQuery>,
    IKickPlayerHandler<JiliKickPlayerCommand>
{
    private readonly ILogger<JILIController> _logger;
    private readonly IMapper _mapper;

    public JILIController(
        ILogger<JILIController> logger,
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
    ///             "gameType": 1,
    ///             "gameId": 2200001,
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
        [FromServices] IGetService<JiliGameUrlQuery, GameUrlResult> getService,
        GameUrlRequest request)
    {
        var query = _mapper.Map<JiliGameUrlQuery>(request);

        var result = await getService
            .GetAsync(query)
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
    ///     Delete ab/kickplayer/4ucmqplxb029
    /// </remarks>
    [HttpDelete("kickplayer/{playerCode}")]
    public ValueTask KickPlayerAsync(
        [FromServices] ICommandService<JiliKickPlayerCommand> commandService,
        string playerCode)
        => commandService.ExecuteAsync(new JiliKickPlayerCommand
        {
            PlayerCode = playerCode
        });
}