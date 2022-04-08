using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGame.ApplicationServices
{
    public class DreamGameGameUrlGetService : IGetService<DreamGameGameUrlQuery, GameUrlResult>
    {
        private readonly IDreamGameApiClient _client;
        private readonly IGetService<DreamGameCreatePlayerQuery, int> _createPlayerService;
        private readonly IDreamGameService _service;

        public DreamGameGameUrlGetService(
            IDreamGameService service,
            IDreamGameApiClient client,
            IGetService<DreamGameCreatePlayerQuery, int> createPlayerService
            )
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _createPlayerService = createPlayerService ?? throw new ArgumentNullException(nameof(createPlayerService));
        }

        public async ValueTask<GameUrlResult> GetAsync(DreamGameGameUrlQuery query, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var gameProviderCultureCode = await _service.GetGameProviderLanguageCodeAsync(query.Language).ConfigureAwait(false);
            var gameProviderCurrencyCode = await _service.GetGameProviderCurrencyCodeAsync(query.CurrencyCode).ConfigureAwait(false);

            var loginResult = await _client.LoginAsync(
                 configuration.ApiUrl,
                 configuration.ApiKey,
                 configuration.AgentName,
                 query.PlayerCode,
                 configuration.PlayerPassword,
                 query.PlatformType,
                 gameProviderCultureCode
                 ).ConfigureAwait(false);

            if (!loginResult.Success && loginResult.CodeId is 102)
            {
                var codeId = await _createPlayerService.GetAsync(new DreamGameCreatePlayerQuery(
                    configuration.ApiUrl,
                    configuration.ApiKey,
                    configuration.AgentName,
                    query.PlayerCode,
                    configuration.PlayerPassword,
                    query.CurrencyCode,
                    gameProviderCurrencyCode,
                    configuration.BetLimits
                    )).ConfigureAwait(false);
                loginResult = codeId is 0 or 116
                    ? await _client.LoginAsync(
                        configuration.ApiUrl,
                       configuration.ApiKey,
                       configuration.AgentName,
                       query.PlayerCode,
                       configuration.PlayerPassword,
                       query.PlatformType,
                       gameProviderCultureCode).ConfigureAwait(false)
                    : throw new Exception($"The manufacturer responded incorrectly.({codeId})");
            }
            else
            {
                if (!loginResult.Success)
                {
                    throw new Exception($"The manufacturer responded incorrectly.({loginResult.CodeId})");
                }
            }

            return (loginResult.Success) ?
                new() { Url = loginResult.Url } :
                throw new Exception($"The manufacturer responded incorrectly.({loginResult.CodeId})"); ;
        }
    }
}
