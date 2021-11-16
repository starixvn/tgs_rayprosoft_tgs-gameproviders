using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.LY.ApplicationServices;
using TGS.GameProviders.Domains.RoundDetail;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LYController : ControllerBase,
        IGameRecordListHandler<LYGameRecordListQuery, LYGameRecordListQuery>,
        IGameUrlHandler<LYGameUrlQuery>,
        IKickPlayerHandler<LYKickPlayerCommand>,
        IPlayerBalanceHandler<LYPlayerBalanceQuery>,
        IPlayerFundCheckingHandler<LYPlayerFundCheckingQuery, PlayerFundCheckingRequest>,
        IPlayerFundHandler<LYPlayerFundInQuery, LYPlayerFundOutQuery>,
        IRoundDetailHandler<LYRoundDetailQuery>

    {
        private readonly ILogger<LYController> _logger;
        private readonly IMapper _mapper;

        public LYController(
            ILogger<LYController> logger,
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
        ///             "currencyCode": "HKD",
        ///             "startTime": "2021-11-02T03:36:03.1588192+00:00",
        ///             "endTime": "2021-11-02T03:46:03.1588192+00:00"
        ///         }
        /// </remarks>
        [HttpPost("GameRecordList")]
        public async ValueTask<GameRecordListResult?> GameRecordListAsync(
            [FromServices] IGetService<LYGameRecordListQuery, GameRecordListResult> getService,
            LYGameRecordListQuery request)
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
        ///         "gameType": 8,
        ///         "gameId": 2000001,
        ///         "brandCode": "GPI",
        ///         "playerCode": "4ucmqplxb029",
        ///         "currencyCode": "HKD",
        ///         "loginRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "platformType": 0,
        ///         "language": "zh-CN",
        ///         "ipAddress": "127.0.0.1",
        ///         "backUrl": "https://dev-t1.rpgp.cc/"
        ///     }
        /// </remarks>
        [HttpPost("GameUrl")]
        public async ValueTask<GameUrlResponse?> GameUrlAsync(
            [FromServices] IGetService<LYGameUrlQuery, GameUrlResult> getService,
            GameUrlRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<LYGameUrlQuery>(request)).ConfigureAwait(false);
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
        ///         "currencyCode": "HKD"
        ///     }
        /// </remarks>
        [HttpPost("KickPlayer")]
        public ValueTask KickPlayerAsync(
            [FromServices] ICommandService<LYKickPlayerCommand> commandService,
            KickPlayerRequest request)
            => commandService.ExecuteAsync(_mapper.Map<LYKickPlayerCommand>(request));

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
        ///         "currencyCode": "HKD"
        ///     }
        /// </remarks>
        [HttpPost("Balance")]
        public async ValueTask<PlayerBalanceResponse?> PlayerBalanceAsync(
            [FromServices] IGetService<LYPlayerBalanceQuery, PlayerBalanceResult> getService,
            PlayerBalanceRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<LYPlayerBalanceQuery>(request)).ConfigureAwait(false);
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
        ///             "currencyCode": "HKD",
        ///             "orderId":"72412202110271546265054ucmqplxb029"
        ///         }
        /// </remarks>
        [HttpPost("PlayerFundChecking")]
        public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
            [FromServices] IGetService<LYPlayerFundCheckingQuery, PlayerFundCheckingResult> getService,
            PlayerFundCheckingRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<LYPlayerFundCheckingQuery>(request)).ConfigureAwait(false);
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
        ///             "currencyCode": "HKD",
        ///             "orderId":"397FE4DF0A2FF0E2820367F0E29CFCAC8F86",
        ///             "amount": 1
        ///         }
        /// </remarks>
        [HttpPost("PlayerFundIn")]
        public async ValueTask<PlayerFundResponse?> PlayerFundInAsync(
            [FromServices] IGetService<LYPlayerFundInQuery, PlayerFundResult> getService,
            PlayerFundRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<LYPlayerFundInQuery>(request)).ConfigureAwait(false);
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
        ///             "currencyCode": "HKD",
        ///             "orderId":"D07B3FAD0BE5F0B4EE0C2A401FC5C41075ED",
        ///             "amount": 1
        ///         }
        /// </remarks>
        [HttpPost("PlayerFundOut")]
        public async ValueTask<PlayerFundResponse?> PlayerFundOutAsync(
            [FromServices] IGetService<LYPlayerFundOutQuery, PlayerFundResult> getService,
            PlayerFundRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<LYPlayerFundOutQuery>(request)).ConfigureAwait(false);
            return result == null ? null : _mapper.Map<PlayerFundResponse>(result);
        }

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
        ///     Get ky/rounddetail/b53d5576-8f61-417d-17da-08d9a5803086?language=zh-CN
        /// </remarks>
        [HttpGet("RoundDetail/{roundId}")]
        public async ValueTask<RoundDetailResult?> RoundDetailAsync(
            [FromServices] IGetService<LYRoundDetailQuery, RoundDetailResult> getService,
            Guid roundId,
            [FromQuery] string language)
        {
            var result = await getService.GetAsync(new()
            {
                RoundId = roundId,
                Language = language
            }).ConfigureAwait(false);
            return result == null ? null : result;
        }
    }
}
