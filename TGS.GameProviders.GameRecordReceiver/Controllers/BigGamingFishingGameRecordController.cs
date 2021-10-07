using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.BigGaming;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecordReceiver.Controllers
{
    public class BigGamingFishingGameRecordController : BaseController<BigGamingFishingGameRecordHandlerQuery, BigGamingFishingGameRecordHandlerResult>
    {
        /// <summary>
        /// Kicks the player asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        /// <remarks>
        ///     Post
        ///     Parameter
        ///     {
        ///         "currencyCode":"CNY",
        ///         "startTime":"2021-08-12T01:30:08.1413497+00:00",
        ///         "endTime":"2021-08-12T01:35:08.1413497+00:00"
        ///     }
        /// </remarks>
        public override ValueTask<BigGamingFishingGameRecordHandlerResult> KickPlayerAsync(
            [FromServices] IGetService<BigGamingFishingGameRecordHandlerQuery, BigGamingFishingGameRecordHandlerResult> service,
            BigGamingFishingGameRecordHandlerQuery query)
            => service.GetAsync(query);
    }
}
