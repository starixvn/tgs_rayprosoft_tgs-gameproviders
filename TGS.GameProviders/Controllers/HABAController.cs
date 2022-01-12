using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Habanero.ApplicationServices;
using TGS.GameProviders.Domains.RoundDetail;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HABAController : ControllerBase,
        IGameUrlHandler<HabaneroGameUrlQuery>,
        IRoundDetailHandler<HabaneroRoundDetailQuery>,
        IKickPlayerHandler<HabaneroKickPlayerCommand>
    {
        private readonly ILogger<HABAController> _logger;
        private readonly IMapper _mapper;

        public HABAController(
            ILogger<HABAController> logger,
            IMapper mapper)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
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
        ///             "gameId": 200001,
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
            [FromServices] IGetService<HabaneroGameUrlQuery, GameUrlResult> getService,
            GameUrlRequest request)
        {
            var result = await getService
                .GetAsync(_mapper.Map<HabaneroGameUrlQuery>(request))
                .ConfigureAwait(false);

            return result == null ?
                null :
                _mapper.Map<GameUrlResponse>(result);
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
        ///     Get haba/rounddetail/d9ca80d4-7a71-46df-341d-08d99f74a980?language=zh-CN
        /// </remarks>
        [HttpGet("RoundDetail/{roundId}")]
        public async ValueTask<RoundDetailResult?> RoundDetailAsync(
            [FromServices] IGetService<HabaneroRoundDetailQuery, RoundDetailResult> getService,
            Guid roundId,
            [FromQuery] string language)
        {
            var result = await getService.GetAsync(new HabaneroRoundDetailQuery
            {
                RoundId = roundId,
                Language = language
            }).ConfigureAwait(false);

            return result ?? null;
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
        ///     Delete haba/kickplayer/4ucmqplxb029
        /// </remarks>
        [HttpDelete("kickplayer/{playerCode}")]
        public ValueTask KickPlayerAsync(
            [FromServices] ICommandService<HabaneroKickPlayerCommand> commandService,
            string playerCode)
            => commandService.ExecuteAsync(new()
            {
                PlayerCode = playerCode
            });
    }
}