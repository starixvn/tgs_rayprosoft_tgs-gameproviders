using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Core.DreamGame.ApplicationServices
{
    public class DreamGameGameUrlGetService : IGetService<DreamGameGameUrlQuery, GameUrlResult>
    {
        private readonly IDreamGameApiClient _client;
        private readonly IDreamGameService _service;

        public DreamGameGameUrlGetService(
            IDreamGameService service,
            IDreamGameApiClient client
            )
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<GameUrlResult> GetAsync(DreamGameGameUrlQuery command, CancellationToken cancellationToken = default)
        {
            var gameProviderCultureCode = await _service.GetGameProviderLanguageCodeAsync(command.Language).ConfigureAwait(false);
            var gameProviderCurrencyCode = await _service.GetGameProviderCurrencyCodeAsync(command.CurrencyCode).ConfigureAwait(false);
            var gameProviderConfiguration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var loginResult = await _client.LoginAsync(
                 gameProviderConfiguration.ApiUrl,
                 gameProviderConfiguration.ApiKey,
                 gameProviderConfiguration.AgentName,
                 command.PlayerCode,
                 gameProviderConfiguration.PlayerPassword,
                 command.PlatformType,
                 gameProviderCultureCode
                 ).ConfigureAwait(false);

            if (!loginResult.Success)
            {
                if (loginResult.CodeId is 102)
                {
                    var codeId = await _client.SignUpAsync(
                        gameProviderConfiguration.ApiUrl,
                        gameProviderConfiguration.ApiKey,
                        gameProviderConfiguration.AgentName,
                        command.PlayerCode,
                        gameProviderConfiguration.PlayerPassword,
                        gameProviderCurrencyCode).ConfigureAwait(false);
                    loginResult = codeId is 0 or 116
                        ? await _client.LoginAsync(
                        gameProviderConfiguration.ApiUrl,
                        gameProviderConfiguration.ApiKey,
                        gameProviderConfiguration.AgentName,
                        command.PlayerCode,
                        gameProviderConfiguration.PlayerPassword,
                        command.PlatformType,
                        gameProviderCultureCode).ConfigureAwait(false)
                        : throw new Exception($"The manufacturer responded incorrectly.({codeId})");
                }
                else
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
