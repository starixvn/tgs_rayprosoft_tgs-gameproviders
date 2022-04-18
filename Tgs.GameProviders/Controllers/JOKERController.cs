using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Joker.ApplicationServices;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JOKERController : ControllerBase,
        IGameUrlHandler<JokerGameUrlQuery>
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
            [FromServices] IGetService<JokerGameUrlQuery, GameUrlResult> getService,
            GameUrlRequest request)
        {
            var result = await getService
                .GetAsync(_mapper.Map<JokerGameUrlQuery>(request))
                .ConfigureAwait(false);

            return result == null ? null : _mapper.Map<GameUrlResponse>(result);
        }
    }
}