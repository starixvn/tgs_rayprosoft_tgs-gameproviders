using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Client;
using TGS.GameProviders.ClientSample.Models;

namespace TGS.GameProviders.ClientSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameProvidersController : ControllerBase
    {
        private readonly IGameProvidersClient _client;
        private readonly ILogger<GameProvidersController> _logger;

        public GameProvidersController(
            ILogger<GameProvidersController> logger,
            IGameProvidersClient client
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Kicks the player asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "gameProviderCode": "ky",
        ///             "brandCode": "GPI",
        ///             "playerCode": "4ucmqplxb029",
        ///             "currencyCode": "CNY"
        ///         }
        /// </remarks>
        [HttpPost("KickPlayer")]
        public ValueTask KickPlayerAsync(
            KickPlayerViewModel model)
            => _client.KickPlayerAsync(
                model.GameProviderCode,
                model.BrandCode,
                model.PlayerCode,
                model.CurrencyCode
                );

        /// <summary>
        /// Kicks the player asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Get
        ///         {
        ///             "gameProviderCode": "ky",
        ///             "gameType": 8,
        ///             "gameId": 2000001,
        ///             "brandCode": "GPI",
        ///             "playerCode": "4ucmqplxb029",
        ///             "currencyCode": "HKD",
        ///             "loginRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///             "platformType": 0,
        ///             "language": "zh-CN",
        ///             "ipAddress": "127.0.0.1",
        ///             "backUrl": "https://dev-t1.rpgp.cc/"
        ///         }
        /// </remarks>
        [HttpGet("GameURL")]
        public ValueTask<Uri?> GameURLAsync(
           [FromQuery] GameUrlViewModel model)
        => _client.GameURLAsync(
                    model.GameProviderCode,
                    model.GameType,
                    model.GameId,
                    model.BrandCode,
                    model.PlayerCode,
                    model.CurrencyCode,
                    model.LoginRecordId,
                    model.PlatformType,
                    model.Language,
                    model.IPAddress,
                    model.BackUrl
                    );


        /// <summary>
        /// Players the balance asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Get
        ///         {
        ///             "gameProviderCode": "ky",
        ///             "brandCode": "GPI",
        ///             "playerCode": "4ucmqplxb029",
        ///             "currencyCode": "CNY"
        ///         }
        /// </remarks>
        [HttpGet("PlayerBalance")]
        public ValueTask<decimal?> PlayerBalanceAsync(
            [FromQuery] PlayerBalanceViewModel model)
            => _client.PlayerBalanceAsync(
                model.GameProviderCode,
                model.BrandCode,
                model.PlayerCode,
                model.CurrencyCode
                );
    }
}
