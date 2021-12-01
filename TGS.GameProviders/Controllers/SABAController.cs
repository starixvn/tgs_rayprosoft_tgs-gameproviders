using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Saba.ApplicationServices;
using TGS.GameProviders.Domains.RoundDetail;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SABAController : ControllerBase,
        IGameUrlHandler<SabaGameUrlQuery>,
        IKickPlayerHandler<SabaKickPlayerCommand>,
        IRoundDetailHandler<SabaRoundDetailQuery>
    {
        private readonly ILogger<SABAController> _logger;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SABAController(
            ILogger<SABAController> logger,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
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
        ///             "gameId": 600001,
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
            [FromServices] IGetService<SabaGameUrlQuery, GameUrlResult> getService,
            GameUrlRequest request)
        {
            var map = _mapper.Map<SabaGameUrlQuery>(request);
            var query = map with
            {
                EnvironmentName = _webHostEnvironment.EnvironmentName
            };

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
        ///     Delete saba/kickplayer/4ucmqplxb029
        /// </remarks>
        [HttpDelete("kickplayer/{playerCode}")]
        public ValueTask KickPlayerAsync(
            [FromServices] ICommandService<SabaKickPlayerCommand> commandService,
            string playerCode)
            => commandService.ExecuteAsync(new SabaKickPlayerCommand
            {
                PlayerCode = playerCode,
                EnvironmentName = _webHostEnvironment.EnvironmentName
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
        ///     Get bg/rounddetail/d9ca80d4-7a71-46df-341d-08d99f74a980?language=zh-CN
        /// </remarks>
        [HttpGet("RoundDetail/{roundId}")]
        public async ValueTask<RoundDetailResult?> RoundDetailAsync(
            [FromServices] IGetService<SabaRoundDetailQuery, RoundDetailResult> getService,
            Guid roundId,
            [FromQuery] string language)
        {
            var result = await getService.GetAsync(new SabaRoundDetailQuery
            {
                RoundId = roundId,
                Language = language
            }).ConfigureAwait(false);

            return result ?? null;
        }
    }
}