using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.WGHorse.ApplicationServices;
using TGS.GameProviders.ViewModels;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Controllers.GameUrl
{
    public class WGVIAController : GameUrlController<WGHorseGameUrlQuery, GameUrlResult>
    {
        private readonly ILogger<WGVIAController> _logger;
        private readonly IMapper _mapper;

        public WGVIAController(
            ILogger<WGVIAController> logger,
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
        ///         "gameType": 1,
        ///         "gameId": 1600001,
        ///         "brandCode": "GPI",
        ///         "playerCode": "4ucmqplxb029",
        ///         "currencyCode": "CNY",
        ///         "loginRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "platformType": 0,
        ///         "language": "zh-CN",
        ///         "ipAddress": "127.0.0.1",
        ///         "backUrl": "https://dev-t1.rpgp.cc/"
        ///     }
        /// </remarks>
        public override async ValueTask<GameUrlResponse> GameUrlAsync(
            [FromServices] IGetService<WGHorseGameUrlQuery, GameUrlResult> getService,
            GameUrlRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<WGHorseGameUrlQuery>(request)).ConfigureAwait(false);
            return result == null ? null : _mapper.Map<GameUrlResponse>(result);
        }
    }
}
