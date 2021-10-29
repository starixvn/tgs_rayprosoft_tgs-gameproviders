using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.GameRecord.Core.ApplicationServices.BigGaming;

namespace TGS.GameProviders.GameRecord.ReceiverServer.Controllers
{
    public class BigGamingFishingGameRecordController : BaseController<BigGamingFishingGameRecordHandlerQuery, BigGamingFishingGameRecordHandlerResult>
    {
        /// <summary>
        /// Games the record asynchronous.
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
        public override ValueTask<BigGamingFishingGameRecordHandlerResult> GameRecordAsync(
            [FromServices] IGetService<BigGamingFishingGameRecordHandlerQuery, BigGamingFishingGameRecordHandlerResult> service,
            BigGamingFishingGameRecordHandlerQuery query)
            => service.GetAsync(query);
    }
}
