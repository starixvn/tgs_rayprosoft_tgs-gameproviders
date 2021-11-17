using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.WGSports.ApplicationServices;
using TGS.GameProviders.Domains.RoundDetail;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WGFMController : ControllerBase,
        IGameRecordListHandler<WGSportsGameRecordListQuery, WGSportsGameRecordListQuery>,
        IGameUrlHandler<WGSportsGameUrlQuery>,
        IKickPlayerHandler<WGSportsKickPlayerCommand>,
        IPlayerBalanceHandler<WGSportsPlayerBalanceQuery>,
        IPlayerFundCheckingHandler<WGSportsPlayerFundCheckingQuery, PlayerFundCheckingRequest>,
        IPlayerFundHandler<WGSportsPlayerFundInQuery, WGSportsPlayerFundOutQuery>,
        IRoundDetailHandler<WGSportsRoundDetailQuery>
    {
        private readonly ILogger<WGFMController> _logger;
        private readonly IMapper _mapper;

        public WGFMController(
            ILogger<WGFMController> logger,
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
        ///             "isSettlement": true,
        ///             "startTime": "2021-10-26T03:34:00-04:00",
        ///             "endTime": "2021-10-26T03:44:00-04:00"
        ///         }
        /// </remarks>
        [HttpPost("GameRecordList")]
        public async ValueTask<GameRecordListResult?> GameRecordListAsync(
            [FromServices] IGetService<WGSportsGameRecordListQuery, GameRecordListResult> getService,
            WGSportsGameRecordListQuery request)
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
        ///         "gameType": 1,
        ///         "gameId": 1500001,
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
            [FromServices] IGetService<WGSportsGameUrlQuery, GameUrlResult> getService,
            GameUrlRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<WGSportsGameUrlQuery>(request)).ConfigureAwait(false);
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
            [FromServices] ICommandService<WGSportsKickPlayerCommand> commandService,
            KickPlayerRequest request)
            => commandService.ExecuteAsync(_mapper.Map<WGSportsKickPlayerCommand>(request));

        /// <summary>
        /// Players the balance asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="playerCode">The player code.</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <returns></returns>
        /// <remarks>
        ///   <b>Example</b><br />
        ///   
        ///   Get wgfm/balance
        ///   brandCode = GPI
        ///   playerCode = 4ucmqplxb029
        ///   currencyCode = CNY
        /// </remarks>
        [HttpGet("Balance")]
        public ValueTask<decimal?> PlayerBalanceAsync(
            [FromServices] IGetService<WGSportsPlayerBalanceQuery, decimal?> getService,
            [FromQuery] string brandCode,
            [FromQuery] string playerCode,
            [FromQuery] string currencyCode)
            => getService.GetAsync(new()
            {
                BrandCode = brandCode,
                PlayerCode = playerCode,
                CurrencyCode = currencyCode
            });

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
        ///             "orderId":"397FE4DF0A2FF0E2820367F0E29CFCAC8F86"
        ///         }
        /// </remarks>
        [HttpPost("PlayerFundChecking")]
        public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(
            [FromServices] IGetService<WGSportsPlayerFundCheckingQuery, PlayerFundCheckingResult> getService,
            PlayerFundCheckingRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<WGSportsPlayerFundCheckingQuery>(request)).ConfigureAwait(false);
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
            [FromServices] IGetService<WGSportsPlayerFundInQuery, PlayerFundResult> getService,
            PlayerFundRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<WGSportsPlayerFundInQuery>(request)).ConfigureAwait(false);
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
            [FromServices] IGetService<WGSportsPlayerFundOutQuery, PlayerFundResult> getService,
            PlayerFundRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<WGSportsPlayerFundOutQuery>(request)).ConfigureAwait(false);
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
        ///     Get wgfm/rounddetail/f6dd64ee-ce12-4bef-9ddb-08d9676f4495?language=zh-CN
        /// </remarks>
        [HttpGet("RoundDetail/{roundId}")]
        public async ValueTask<RoundDetailResult?> RoundDetailAsync(
            [FromServices] IGetService<WGSportsRoundDetailQuery, RoundDetailResult> getService,
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
