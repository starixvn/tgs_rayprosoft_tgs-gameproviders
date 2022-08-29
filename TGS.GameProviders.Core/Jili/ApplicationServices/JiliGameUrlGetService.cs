using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using Tgs.GameProviders.Core.Jili.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Jili.ApplicationServices;

internal class JiliGameUrlGetService : IGetService<JiliGameUrlQuery, GameUrlResult>
{
    private readonly IJiliApiClient _client;
    private readonly IGameRepository _gameRepository;
    private readonly IJiliMerchantMapper _merchantMapper;
    private readonly IJiliService _service;
    private readonly ITokenProvider _tokenProvider;

    public JiliGameUrlGetService(
        IGameRepository gameRepository,
        ITokenProvider tokenProvider,
        IJiliService service,
        IJiliMerchantMapper merchantMapper,
        IJiliApiClient client)
    {
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _merchantMapper = merchantMapper ?? throw new ArgumentNullException(nameof(merchantMapper));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameUrlResult> GetAsync(JiliGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var token = $"{query.PlayerCode},{query.LoginRecordId},{query.PlatformType}";
        var encryptedToken = await _tokenProvider.EncryptAsync(token).ConfigureAwait(false);

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var merchant = _merchantMapper.GetMerchant(configuration.Merchants, query.BrandCode);

        var game = await _gameRepository.GetAsync(query.GameId!.Value).ConfigureAwait(false);
        var gameProviderCultureCode =
            await _service.GetGameProviderLanguageCodeAsync(game!.GameProviderId, query.Language);

        var response = await _client
            .LoginWithoutRedirectAsync(
                configuration.ApiUrl,
                merchant.AgentId,
                merchant.AgentKey,
                new JiliLoginWithoutRedirectRequest
                {
                    GameExternalId = game.GameExternalId,
                    Language = gameProviderCultureCode,
                    Token = encryptedToken
                })
            .ConfigureAwait(false);

        if (response.ErrorCode != 0)
        {
            return null;
        }

        return new GameUrlResult
        {
            Url = new Uri(response.Url)
        };
    }
}
