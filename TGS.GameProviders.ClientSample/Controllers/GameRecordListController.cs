using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Client;

namespace TGS.GameProviders.ClientSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameRecordListController : ControllerBase
    {
        private readonly IGameRecordListClient _client;
        private readonly ILogger<GameRecordListController> _logger;

        public GameRecordListController(
            ILogger<GameRecordListController> logger,
            IGameRecordListClient client
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Bigs the gaming fishing game record list asynchronous.
        /// </summary>
        /// <param name="currencyCode">The currency code.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="page">The page.</param>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Get
        ///         {
        ///             "currencyCode": "CNY",
        ///             "startTime": "2021-11-01T01:30:08.1413497+00:00",
        ///             "endTime": "2021-11-01T01:35:08.1413497+00:00",
        ///             "page": 1,
        ///             "rows": 500
        ///         }
        /// </remarks>
        [HttpGet("BigGamingFishing")]
        public ValueTask<GameRecordListResponse?> BigGamingFishingGameRecordListAsync(
            [FromQuery] string currencyCode,
            [FromQuery] DateTimeOffset startTime,
            [FromQuery] DateTimeOffset endTime,
            [FromQuery] int page,
            [FromQuery] int rows)
            => _client.BigGamingFishingGameRecordListAsync(currencyCode, startTime, endTime, page, rows);

        /// <summary>
        /// Kies the game record list asynchronous.
        /// </summary>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Get
        ///         {
        ///             "brandCode": "Default",
        ///             "currencyCode": "CNY",
        ///             "startTime": "2021-08-12T05:50:00+00:00",
        ///             "endTime": "2021-08-12T06:00:00+00:00"
        ///         }
        /// </remarks>
        [HttpGet("KY")]
        public ValueTask<GameRecordListResponse?> KYGameRecordListAsync(
           [FromQuery] string brandCode,
           [FromQuery] string currencyCode,
           [FromQuery] DateTimeOffset startTime,
           [FromQuery] DateTimeOffset endTime)
            => _client.KYGameRecordListAsync(brandCode, currencyCode, startTime, endTime);

        /// <summary>
        /// Lies the game record list asynchronous.
        /// </summary>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Get
        ///         {
        ///             "brandCode": "Default",
        ///             "currencyCode": "HKD",
        ///             "startTime": "2021-11-01T07:00:00+08:00",
        ///             "endTime": "2021-11-01T07:40:00+08:00"
        ///         }
        /// </remarks>
        [HttpGet("LY")]
        public ValueTask<GameRecordListResponse?> LYGameRecordListAsync(
           [FromQuery] string brandCode,
           [FromQuery] string currencyCode,
           [FromQuery] DateTimeOffset startTime,
           [FromQuery] DateTimeOffset endTime)
            => _client.LYGameRecordListAsync(brandCode, currencyCode, startTime, endTime);

        /// <summary>
        /// Vrs the game record list asynchronous.
        /// </summary>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="isUpdateTime">if set to <c>true</c> [is update time].</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="page">The page.</param>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Get
        ///         {
        ///             "brandCode": "Default",
        ///             "isUpdateTime": true,
        ///             "startTime": "2021-10-28T03:50:00+08:00",
        ///             "endTime": "2021-10-28T04:00:00+08:00",
        ///             "page": 1,
        ///             "rows": 500
        ///         }
        /// </remarks>
        [HttpGet("VR")]
        public ValueTask<GameRecordListResponse?> VRGameRecordListAsync(
           [FromQuery] string brandCode,
           [FromQuery] bool isUpdateTime,
           [FromQuery] DateTimeOffset startTime,
           [FromQuery] DateTimeOffset endTime,
           [FromQuery] int page,
           [FromQuery] int rows)
            => _client.VRGameRecordListAsync(brandCode, isUpdateTime, startTime, endTime, page, rows);

        /// <summary>
        /// WGFMs the game record list asynchronous.
        /// </summary>
        /// <param name="isSettlement">if set to <c>true</c> [is settlement].</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Get
        ///         {
        ///             "isSettlement": true,
        ///             "startTime": "2021-10-26T03:34:00-04:00",
        ///             "endTime": "2021-10-26T03:44:00-04:00"
        ///         }
        /// </remarks>
        [HttpPost("WGFM")]
        public ValueTask<GameRecordListResponse?> WGSportsGameRecordListAsync(
            [FromQuery] bool isSettlement,
            [FromQuery] DateTimeOffset startTime,
            [FromQuery] DateTimeOffset endTime)
            => _client.WGSportsGameRecordListAsync(isSettlement, startTime, endTime);

        /// <summary>
        /// Wgvias the game record list asynchronous.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="page">The page.</param>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Get
        ///         {
        ///             "startTime": "2021-08-18T00:00:00+00:00",
        ///             "endTime": "2021-08-19T00:00:00+00:00",
        ///             "page": 1,
        ///             "rows": 500
        ///         }
        /// </remarks>
        [HttpGet("WGVIA")]
        public ValueTask<GameRecordListResponse?> WGHorseGameRecordListAsync(
            [FromQuery] DateTimeOffset startTime,
            [FromQuery] DateTimeOffset endTime,
            [FromQuery] int page,
            [FromQuery] int rows)
            => _client.WGHorseGameRecordListAsync(startTime, endTime, page, rows);
    }
}
