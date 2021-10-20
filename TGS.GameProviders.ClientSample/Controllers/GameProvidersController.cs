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
    public class GameProvidersController : ControllerBase
    {
        private readonly ILogger<GameProvidersController> _logger;
        private readonly IGameProvidersClient _client;
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
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("KickPlayer")]
        public ValueTask KickPlayerAsync(
            KickPlayerRequest request)
            => _client.KickPlayer(request);
    }
}
