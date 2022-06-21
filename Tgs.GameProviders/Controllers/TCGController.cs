using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.TCG.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TCGController : ControllerBase,
        IGameUrlHandler<TCGGameUrlQuery>,
        IKickPlayerHandler<TCGKickPlayerCommand>,
        IRoundDetailHandler<TCGRoundDetailQuery>
    {
        private readonly IMapper _mapper;

        public TCGController(IMapper mapper)
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
        ///         TCG_SEA
        ///         {
        ///             "gameType": 14,
        ///             "gameId": 1800001,
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
        ///     Post
        ///         TCG_LOTTO
        ///         {
        ///             "gameType": 14,
        ///             "gameId": 1800002,
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
        ///     Post
        ///         TCG_LOTTO_VN
        ///         {
        ///             "gameType": 14,
        ///             "gameId": 1800003,
        ///             "brandCode": "GPI",
        ///             "playerCode": "4ucmqplxb029",
        ///             "currencyCode": "VND",
        ///             "loginRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///             "platformType": 0,
        ///             "language": "zh-CN",
        ///             "ipAddress": "127.0.0.1",
        ///             "backUrl": "https://dev-t1.rpgp.cc/"
        ///         }
        /// </remarks>
        [HttpPost("GameUrl")]
        public async ValueTask<GameUrlResponse?> GameUrlAsync(
            [FromServices] IGetService<TCGGameUrlQuery, GameUrlResult> getService,
            GameUrlRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<TCGGameUrlQuery>(request)).ConfigureAwait(false);
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
        ///     Delete kickplayer/4ucmqplxb029
        /// </remarks>
        [HttpDelete("kickplayer/{playerCode}")]
        public ValueTask KickPlayerAsync(
            [FromServices] ICommandService<TCGKickPlayerCommand> commandService,
            string playerCode)
            => commandService.ExecuteAsync(new()
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
        ///     Get rounddetail/d14b1a37-d473-4439-a0a8-08d97f42d936?language=zh-CN
        /// </remarks>
        [HttpGet("RoundDetail/{roundId}")]
        public async ValueTask<RoundDetailResult?> RoundDetailAsync(
            [FromServices] IGetService<TCGRoundDetailQuery, RoundDetailResult?> getService,
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
}
