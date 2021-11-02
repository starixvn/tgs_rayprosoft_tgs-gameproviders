using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.VR.ApplicationServices;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VRController : ControllerBase,
        IGameRecordListHandler<VRGameRecordListQuery, VRGameRecordListQuery>,
        IGameUrlHandler<VRGameUrlQuery>,
        IKickPlayerHandler<VRKickPlayerCommand>,
        IPlayerBalanceHandler<VRPlayerBalanceQuery>,
        IPlayerFundCheckingHandler<VRPlayerFundCheckingQuery, VRPlayerFundCheckingRequest>,
        IPlayerFundHandler<VRPlayerFundInQuery, VRPlayerFundOutQuery>
    {
        private readonly ILogger<VRController> _logger;
        private readonly IMapper _mapper;

        public VRController(
            ILogger<VRController> logger,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Games the record list asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandCode": "Default",
        ///             "startTime": "2021-10-28T03:50:00+08:00",
        ///             "endTime": "2021-10-28T04:00:00+08:00",
        ///             "isUpdateTime": true,
        ///             "page": 1,
        ///             "rows": 500
        ///         }
        /// </remarks>
        [HttpPost("GameRecordList")]
        public async ValueTask<GameRecordListResult?> GameRecordListAsync(
            [FromServices] IGetService<VRGameRecordListQuery, GameRecordListResult> getService,
            VRGameRecordListQuery request)
        {
            var result = await getService.GetAsync(request).ConfigureAwait(false);
            return result == null ? null : result;
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
        ///         "gameType": 14,
        ///         "gameId": 1100001,
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
        [HttpPost("GameUrl")]
        public async ValueTask<GameUrlResponse?> GameUrlAsync(
            [FromServices] IGetService<VRGameUrlQuery, GameUrlResult> getService,
            GameUrlRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<VRGameUrlQuery>(request)).ConfigureAwait(false);
            return result == null ? null : _mapper.Map<GameUrlResponse>(result);
        }

        /// <summary>
        /// Kicks the player asynchronous.
        /// </summary>
        /// <param name="commandService">The command service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        /// Post
        ///     {
        ///         "brandCode": "GPI",
        ///         "playerCode": "4ucmqplxb029",
        ///         "currencyCode": "CNY"
        ///     }
        /// </remarks>
        [HttpPost("KickPlayer")]
        public ValueTask KickPlayerAsync(
            [FromServices] ICommandService<VRKickPlayerCommand> commandService,
            KickPlayerRequest request)
            => commandService.ExecuteAsync(_mapper.Map<VRKickPlayerCommand>(request));

        /// <summary>
        /// Players the balance asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        /// Post
        ///     {
        ///         "brandCode": "GPI",
        ///         "playerCode": "4ucmqplxb029",
        ///         "currencyCode": "CNY"
        ///     }
        /// </remarks>
        [HttpPost("Balance")]
        public async ValueTask<PlayerBalanceResponse?> PlayerBalanceAsync(
            [FromServices] IGetService<VRPlayerBalanceQuery, PlayerBalanceResult> getService,
            PlayerBalanceRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<VRPlayerBalanceQuery>(request)).ConfigureAwait(false);
            return result == null ? null : _mapper.Map<PlayerBalanceResponse>(result);
        }

        /// <summary>
        /// Players the fund checking asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="request">The request.</param>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandCode": "GPI",
        ///             "playerCode": "4ucmqplxb029",
        ///             "currencyCode": "CNY",
        ///             "orderId": "D07B3FAD0BE5F0B4EE0C2A401FC5C41075ED",
        ///             "startTime": "2021-10-27T00:00:00.000Z",
        ///             "endTime": "2021-10-28T00:00:00.000Z"
        ///         }
        /// </remarks>
        [HttpPost("PlayerFundChecking")]
        public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
            [FromServices] IGetService<VRPlayerFundCheckingQuery, PlayerFundCheckingResult> getService,
            VRPlayerFundCheckingRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<VRPlayerFundCheckingQuery>(request)).ConfigureAwait(false);
            return result == null ? null : _mapper.Map<PlayerFundCheckingResponse>(result);
        }

        /// <summary>
        /// Players the fund in asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandCode": "GPI",
        ///             "playerCode": "4ucmqplxb029",
        ///             "currencyCode": "CNY",
        ///             "orderId":"397FE4DF0A2FF0E2820367F0E29CFCAC8F86",
        ///             "amount": 1
        ///         }
        /// </remarks>
        [HttpPost("PlayerFundIn")]
        public async ValueTask<PlayerFundResponse?> PlayerFundInAsync(
            [FromServices] IGetService<VRPlayerFundInQuery, PlayerFundResult> getService,
            PlayerFundRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<VRPlayerFundInQuery>(request)).ConfigureAwait(false);
            return result == null ? null : _mapper.Map<PlayerFundResponse>(result);
        }

        /// <summary>
        /// Players the fund out asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "brandCode": "GPI",
        ///             "playerCode": "4ucmqplxb029",
        ///             "currencyCode": "CNY",
        ///             "orderId":"D07B3FAD0BE5F0B4EE0C2A401FC5C41075ED",
        ///             "amount": 1
        ///         }
        /// </remarks>
        [HttpPost("PlayerFundOut")]
        public async ValueTask<PlayerFundResponse?> PlayerFundOutAsync(
            [FromServices] IGetService<VRPlayerFundOutQuery, PlayerFundResult> getService,
            PlayerFundRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<VRPlayerFundOutQuery>(request)).ConfigureAwait(false);
            return result == null ? null : _mapper.Map<PlayerFundResponse>(result);
        }
    }
}
