using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Client;
using TGS.GameProviders.ClientSample.Models;
using TGS.GameProviders.Domains.RoundDetail;

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
        /// <b>Example</b><br />
        ///
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
        [HttpGet("GameUrl")]
        public ValueTask<Uri?> GameUrlAsync(
           [FromQuery] GameUrlViewModel model)
        => _client.GameUrlAsync(
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
                    model.BackUrl);

        /// <summary>
        /// Kicks the player asynchronous.
        /// </summary>
        /// <param name="gameProviderCode">The game provider code.</param>
        /// <param name="playerCode">The player code.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Delete ky/KickPlayer/4ucmqplxb029
        /// </remarks>
        [HttpDelete("{gameProviderCode}/KickPlayer/{playerCode}")]
        public ValueTask KickPlayerAsync(string gameProviderCode, string playerCode)
            => _client.KickPlayerAsync(gameProviderCode, playerCode);

        /// <summary>
        /// Players the balance asynchronous.
        /// </summary>
        /// <param name="gameProviderCode">The game provider code.</param>
        /// <param name="playerCode">The player code.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///   Get vr/balance/4ucmqplxb029
        /// </remarks>
        [HttpGet("{gameProviderCode}/balance/{playerCode}")]
        public ValueTask<decimal?> PlayerBalanceAsync(
             string gameProviderCode,
             string playerCode)
            => _client.PlayerBalanceAsync(
                gameProviderCode,
                playerCode
                );

        /// <summary>
        ///
        /// </summary>
        /// <param name="gameProviderCode"></param>
        /// <param name="roundId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        [HttpGet("RoundDetail")]
        public ValueTask<RoundDetailResult?> RoundDetailAsync(
            [FromQuery] string gameProviderCode,
            [FromQuery] Guid roundId,
            [FromQuery] string language)
            => _client.RoundDetailAsync(
                gameProviderCode,
                roundId,
                language);
    }
}
