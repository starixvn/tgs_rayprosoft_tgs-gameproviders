using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.BetConstruct.ApplicationServices;
using TGS.GameProviders.Core.TCG.ApplicationServices;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BCController : ControllerBase,
        IGameUrlHandler<BetConstructGameUrlQuery>,
        IGameRecordListHandler<BetConstructGameRecordListQuery, BetConstructGameRecordListQuery>
    {
        private readonly ILogger<BCController> _logger;
        private readonly IMapper _mapper;

        public BCController(
            ILogger<BCController> logger,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ValueTask<GameRecordListResult?> GameRecordListAsync(
            [FromServices] IGetService<BetConstructGameRecordListQuery, GameRecordListResult> getService,
            BetConstructGameRecordListQuery request)
        {
            
        }


        /// <summary>
        /// Games the URL asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// # Example
        /// ## Post
        ///      
        /// 1. BC_Sportsbook
        /// 
        /// `
        ///     {
        ///         "gameType": 1,
        ///         "gameId": 2100001,
        ///         "brandCode": "GPI",
        ///         "playerCode": "4ucmqplxb029",
        ///         "currencyCode": "CNY",
        ///         "loginRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "platformType": 0,
        ///         "language": "zh-CN",
        ///         "ipAddress": "127.0.0.1",
        ///         "backUrl": "https://dev-t1.rpgp.cc/"
        ///     }
        /// `
        ///         
        /// </remarks>
        [HttpPost("GameUrl")]
        public async ValueTask<GameUrlResponse?> GameUrlAsync(
            [FromServices] IGetService<BetConstructGameUrlQuery, GameUrlResult> getService, 
            GameUrlRequest request)
        {
            var result = await getService
                .GetAsync(_mapper.Map<BetConstructGameUrlQuery>(request))
                .ConfigureAwait(false);

            return result == null ? null : _mapper.Map<GameUrlResponse>(result);
        }

        
    }
}
