using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        [HttpPost("BigGamingFishing")]
        public ValueTask<GameRecordListResponse?> BigGamingFishingGameRecordListAsync(
            [FromQuery] string currencyCode,
            [FromQuery] DateTimeOffset startTime,
            [FromQuery] DateTimeOffset endTime,
            [FromQuery] int page,
            [FromQuery] int rows)
            => _client.BigGamingFishingGameRecordListAsync(currencyCode, startTime, endTime, page, rows);

        [HttpPost("KY")]
        public ValueTask<GameRecordListResponse?> KYGameRecordListAsync(
           [FromQuery] string brandCode,
           [FromQuery] string currencyCode,
           [FromQuery] DateTimeOffset startTime,
           [FromQuery] DateTimeOffset endTime)
            => _client.KYGameRecordListAsync(brandCode, currencyCode, startTime, endTime);

        [HttpPost("LY")]
        public ValueTask<GameRecordListResponse?> LYGameRecordListAsync(
           [FromQuery] string brandCode,
           [FromQuery] string currencyCode,
           [FromQuery] DateTimeOffset startTime,
           [FromQuery] DateTimeOffset endTime)
            => _client.LYGameRecordListAsync(brandCode, currencyCode, startTime, endTime);

        [HttpPost("VR")]
        public ValueTask<GameRecordListResponse?> VRGameRecordListAsync(
           [FromQuery] string brandCode,
           [FromQuery] DateTimeOffset startTime,
           [FromQuery] DateTimeOffset endTime,
           [FromQuery] int page,
           [FromQuery] int rows)
            => _client.VRGameRecordListAsync(brandCode, startTime, endTime, page, rows);

        [HttpPost("WGFM")]
        public ValueTask<GameRecordListResponse?> WGFMGameRecordListAsync(
            [FromQuery] bool isSettlement,
            [FromQuery] DateTimeOffset startTime,
            [FromQuery] DateTimeOffset endTime)
            => _client.WGFMGameRecordListAsync(isSettlement, startTime, endTime);

        [HttpPost("WGVIA")]
        public ValueTask<GameRecordListResponse?> WGVIAGameRecordListAsync(
            [FromQuery] DateTimeOffset startTime,
            [FromQuery] DateTimeOffset endTime,
            [FromQuery] int page,
            [FromQuery] int rows)
            => _client.WGVIAGameRecordListAsync(startTime, endTime, page, rows);
    }
}
