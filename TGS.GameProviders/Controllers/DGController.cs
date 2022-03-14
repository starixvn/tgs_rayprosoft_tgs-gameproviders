using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.DreamGame.ApplicationServices;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DGController : ControllerBase,
        IGameUrlHandler<DreamGameGameUrlQuery>,
        IKickPlayerHandler<DreamGameKickPlayerCommand>
    {
        private readonly IMapper _mapper;

        public DGController(
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
        ///             "gameType": 6,
        ///             "gameId": 2500001,
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
            [FromServices] IGetService<DreamGameGameUrlQuery, GameUrlResult> getService,
            GameUrlRequest request)
        {
            var result = await getService
                .GetAsync(_mapper.Map<DreamGameGameUrlQuery>(request))
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
        ///     Delete dg/kickplayer/4ucmqplxb029
        /// </remarks>
        [HttpDelete("kickplayer/{playerCode}")]
        public ValueTask KickPlayerAsync([FromServices] ICommandService<DreamGameKickPlayerCommand> commandService, string playerCode)
            => commandService.ExecuteAsync(new()
            {
                PlayerCode = playerCode
            });
    }
}
