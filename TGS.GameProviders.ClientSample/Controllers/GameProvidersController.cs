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
                new()
                {
                    BrandCode = model.BrandCode,
                    PlayerCode = model.PlayerCode,
                    CurrencyCode = model.CurrencyCode
                });

        /// <summary>
        /// Players the balance asynchronous.
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
        [HttpPost("PlayerBalance")]
        public ValueTask<PlayerBalanceResponse?> PlayerBalanceAsync(
            PlayerBalanceViewModel model)
            => _client.PlayerBalanceAsync(
                model.GameProviderCode,
                new()
                {
                    BrandCode = model.BrandCode,
                    PlayerCode = model.PlayerCode,
                    CurrencyCode = model.CurrencyCode
                });
    }
}
