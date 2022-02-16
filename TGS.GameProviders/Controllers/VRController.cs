using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.VR.ApplicationServices;
using TGS.GameProviders.Domains.RoundDetail;
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
        IPlayerFundHandler<VRPlayerFundInQuery, VRPlayerFundOutQuery>,
        IRoundDetailHandler<VRRoundDetailQuery>
    {
        private readonly IMapper _mapper;

        public VRController(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Games the record list asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
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
            return result ?? null;
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
        ///             "gameType": 14,
        ///             "gameId": 1100001,
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
        /// <param name="playerCode">The player code.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Delete vr/kickplayer/4ucmqplxb029
        /// </remarks>
        [HttpDelete("kickplayer/{playerCode}")]
        public ValueTask KickPlayerAsync(
            [FromServices] ICommandService<VRKickPlayerCommand> commandService,
            string playerCode)
            => commandService.ExecuteAsync(new()
            {
                PlayerCode = playerCode
            });

        /// <summary>
        /// Players the balance asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="playerCode">The player code.</param>
        /// <returns></returns>
        /// <remarks>
        ///   <b>Example</b><br />
        ///
        ///   Get vr/balance/4ucmqplxb029
        /// </remarks>
        [HttpGet("Balance/{playerCode}")]
        public ValueTask<decimal?> PlayerBalanceAsync(
             [FromServices] IGetService<VRPlayerBalanceQuery, decimal?> getService,
             string playerCode)
            => getService.GetAsync(new()
            {
                PlayerCode = playerCode
            });

        /// <summary>
        /// Players the fund checking asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///         Post
        ///             {
        ///                 "brandCode": "GPI",
        ///                 "playerCode": "4ucmqplxb029",
        ///                 "currencyCode": "CNY",
        ///                 "orderId": "D07B3FAD0BE5F0B4EE0C2A401FC5C41075ED",
        ///                 "startTime": "2021-10-27T00:00:00.000Z",
        ///                 "endTime": "2021-10-28T00:00:00.000Z"
        ///             }
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
        /// <b>Example</b><br />
        ///
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
        /// <b>Example</b><br />
        ///
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
        ///     Get vr/rounddetail/1fe63a42-2e7a-4ddf-17cf-08d9a5803086?language=zh-CN
        /// </remarks>
        [HttpGet("RoundDetail/{roundId}")]
        public async ValueTask<RoundDetailResult?> RoundDetailAsync(
            [FromServices] IGetService<VRRoundDetailQuery, RoundDetailResult?> getService,
            Guid roundId,
            [FromQuery] string language)
        {
            var result = await getService.GetAsync(new()
            {
                RoundId = roundId,
                Language = language
            }).ConfigureAwait(false);
            return result ?? null;
        }
    }
}
