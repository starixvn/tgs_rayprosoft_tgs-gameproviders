using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.SexyGaming.ApplicationServices;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.Controllers.GameUrl
{
    public class SGController : GameUrlController<SexyGamingGameUrlQuery>
    {
        private readonly ILogger<SGController> _logger;
        private readonly IMapper _mapper;

        public SGController(
            ILogger<SGController> logger,
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
        ///     {
        ///         "gameType": 6,
        ///         "gameId": 500010,
        ///         "brandCode": "GPI",
        ///         "playerCode": "4ucmqplxb029",
        ///         "currencyCode": "CNY",
        ///         "loginRecordId": "f325f1e8-c877-427c-5196-08d99762447f",
        ///         "platformType": 0,
        ///         "language": "zh-CN",
        ///         "ipAddress": "127.0.0.1",
        ///         "backUrl": "https://dev-t1.rpgp.cc/"
        ///     }
        /// </remarks>
        public override async ValueTask<GameUrlResponse?> GameUrlAsync(
            [FromServices] IGetService<SexyGamingGameUrlQuery, GameUrlResult> getService,
            GameUrlRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<SexyGamingGameUrlQuery>(request)).ConfigureAwait(false);
            return result == null ? null : _mapper.Map<GameUrlResponse>(result);
        }
    }
}
