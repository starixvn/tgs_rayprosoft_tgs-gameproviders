using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.Avia.ApplicationServices
{
    public class AviaGameUrlGetService : IGetService<AviaGameUrlQuery, GameUrlResult>
    {
        private readonly IAviaApiClient _client;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<AviaGameUrlGetService> _logger;
        private readonly IAviaService _service;

        public AviaGameUrlGetService(
            IAviaApiClient client,
            IGameRepository gameRepository,
            ILogger<AviaGameUrlGetService> logger,
            IAviaService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameUrlResult> GetAsync(AviaGameUrlQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var gameProviderCultureCode = await _service.GetGameProviderCultureCodeAsync(command.Language).ConfigureAwait(false);


            var response = await _client.GetGameUrlAsync(
                configuration.ApiUrl,
                configuration.Authorization,
                command.PlayerCode,
                gameProviderCultureCode,
                null,
                null).ConfigureAwait(false);

            return (response != null) ? new() { Url = response } : null;
        }
    }
}
