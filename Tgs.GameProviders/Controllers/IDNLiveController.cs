using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.IDNLive.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Controllers
{
    [Route("api/idnlive")]
    [ApiController]
    public class IDNLiveController : ControllerBase,
        IGameUrlHandler<IDNLiveGameUrlQuery>,
        IRoundDetailHandler<IDNLiveRoundDetailQuery>
    {
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
        ///             "gameType": 5,
        ///             "gameId": 4500001,
        ///             "brandCode": "IDR1",
        ///             "playerCode": "4uhgz11ysxij",
        ///             "currencyCode": "IDR",
        ///             "loginRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///             "platformType": 0,
        ///             "language": "zh-CN",
        ///             "ipAddress": "127.0.0.1",
        ///             "backUrl": "https://dev-t4-idr.rpgp.cc/"
        ///         }
        /// </remarks>
        [HttpPost("GameUrl")]
        public async ValueTask<GameUrlResponse?> GameUrlAsync(
            [FromServices] IGetService<IDNLiveGameUrlQuery, GameUrlResult> getService,
            GameUrlRequest request)
        {
            var result = await getService.GetAsync(new IDNLiveGameUrlQuery()
            {
                GameType = request.GameType,
                GameId = request.GameId,
                BrandCode = request.BrandCode,
                PlayerCode = request.PlayerCode,
                CurrencyCode = request.CurrencyCode,
                LoginRecordId = request.LoginRecordId,
                PlatformType = request.PlatformType,
                Language = request.Language,
                IPAddress = request.IPAddress,
                BackUrl = request.BackUrl
            }).ConfigureAwait(false);
            return result == null ? null : new GameUrlResponse() { Url = result.Url };
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
        ///     Get rounddetail/c3011003-6357-4cff-9404-97312397a791?language=zh-CN
        /// </remarks>
        [HttpGet("RoundDetail/{roundId}")]
        public async ValueTask<RoundDetailResult?> RoundDetailAsync(
            [FromServices] IGetService<IDNLiveRoundDetailQuery, RoundDetailResult?> getService,
            Guid roundId,
            [FromQuery] string language)
            => await getService
            .GetAsync(new IDNLiveRoundDetailQuery
            {
                RoundId = roundId,
                Language = language
            })
            .ConfigureAwait(false);
    }
}
