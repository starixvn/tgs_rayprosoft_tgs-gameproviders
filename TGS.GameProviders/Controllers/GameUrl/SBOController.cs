using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.SBO.ApplicationServices;
using TGS.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Controllers.GameUrl
{
    [ApiController]
    public class SBOController : GameUrlController<SBOGameUrlQuery>
    {
        private readonly ILogger<SBOController> _logger;
        private readonly IMapper _mapper;

        public SBOController(
            ILogger<SBOController> logger,
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
        /// {
        ///    "gameType": 1,
        ///    "gameId": 100001,
        ///    "brandCode": "GPI",
        ///    "playerCode": "4ucmqplxb029",
        ///    "currencyCode": "CNY",
        ///    "loginRecordId": "ec5f3c9b-8c38-ec11-8e06-70b5e82b6b8d",
        ///    "platformType": 0,
        ///    "language": "zh-CN",
        ///    "ipAddress": "127.0.0.1",
        ///    "backUrl": "https://dev-t1.rpgp.cc"
        /// }
        /// </remarks>
        public override async ValueTask<GameUrlResponse?> GameUrlAsync(
            [FromServices] IGetService<SBOGameUrlQuery, GameUrlResult> getService,
            GameUrlRequest request)
        {
            var query = _mapper.Map<SBOGameUrlQuery>(request);
            var result = await getService.GetAsync(query).ConfigureAwait(false);

            return result == null ? null : _mapper.Map<GameUrlResponse>(result);
        }
    }
}