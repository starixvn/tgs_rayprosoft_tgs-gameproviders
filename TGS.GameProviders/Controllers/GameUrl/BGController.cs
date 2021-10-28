using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.BigGaming.ApplicationServices;
using TGS.GameProviders.ViewModels;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers.GameUrl
{
    public class BGController : GameUrlController<BigGamingGameUrlQuery>
    {
        private readonly ILogger<BGController> _logger;
        private readonly IMapper _mapper;

        public BGController(
            ILogger<BGController> logger,
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
        /// Example
        /// Post
        ///     LiveDealerLobby
        ///     {
        ///         "gameType": 6,
        ///         "gameId": 800001,
        ///         "brandCode": "GPI",
        ///         "playerCode": "4ucmqplxb029",
        ///         "currencyCode": "CNY",
        ///         "loginRecordId": "1c4ef822-e85a-4eda-581e-08d99762447f",
        ///         "platformType": 0,
        ///         "language": "zh-CN",
        ///         "ipAddress": "127.0.0.1",
        ///         "backUrl": "https://dev-t1.rpgp.cc/"
        ///     }
        ///     Fishing
        ///     {
        ///         "gameType": 9,
        ///         "gameId": 800017,
        ///         "brandCode": "GPI",
        ///         "playerCode": "4ucmqplxb029",
        ///         "currencyCode": "CNY",
        ///         "loginRecordId": "1c4ef822-e85a-4eda-581e-08d99762447f",
        ///         "platformType": 0,
        ///         "language": "zh-CN",
        ///         "ipAddress": "127.0.0.1",
        ///         "backUrl": "https://dev-t1.rpgp.cc/"
        ///     }
        /// </remarks>
        public override async ValueTask<GameUrlResponse?> GameUrlAsync(
            [FromServices] IGetService<BigGamingGameUrlQuery, GameUrlResult> getService,
            GameUrlRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<BigGamingGameUrlQuery>(request)).ConfigureAwait(false);
            return result == null ? null : _mapper.Map<GameUrlResponse>(result);
        }
    }
}
