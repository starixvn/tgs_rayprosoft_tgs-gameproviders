using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core.Card365.Model;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Card365.ApplicationServices;

internal class Card365GameUrlQueryGetService : IGetService<Card365GameUrlQuery, GameUrlResult>
{
    private readonly ICard365AgentMapper _agentMapper;
    private readonly ICard365ApiClient _client;
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ICard365Service _service;
    private readonly ITokenProvider _tokenProvider;

    public Card365GameUrlQueryGetService(
        IPlayerRepository playerRepository,
        IGameRepository gameRepository,
        ICard365Service service,
        ICard365AgentMapper agentMapper,
        ICard365ApiClient client,
        ITokenProvider tokenProvider)
    {
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
    }

    public async ValueTask<GameUrlResult> GetAsync(
        Card365GameUrlQuery query,
        CancellationToken cancellationToken = default)
    {
        var player = await _playerRepository.GetAsync(query.PlayerCode).ConfigureAwait(false);

        var game = await _gameRepository.GetAsync(query.GameId!.Value).ConfigureAwait(false);

        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var agent = _agentMapper.GetAgent(configuration.BrandAgents, player!.BrandCode);

        var gameProviderId = game!.GameProviderId;

        var accessKey = await GetKeyAsync(
                configuration.ApiUrl,
                configuration.Cert,
                configuration.Website,
                agent.AgentId,
                gameProviderId,
                player)
            .ConfigureAwait(false);

        if (accessKey == null)
        {
            return null;
        }

        var gameProviderLanguageCode = await _service
            .GetGameProviderLanguageCodeAsync(gameProviderId, query.Language)
            .ConfigureAwait(false);

        var url = await GetGameUrlAsync(
                configuration.ApiUrl,
                configuration.Website,
                agent.AgentId,
                accessKey,
                player,
                gameProviderLanguageCode,
                query.LoginRecordId)
            .ConfigureAwait(false);

        return new GameUrlResult
        {
            Url = new Uri(url)
        };
    }

    private async ValueTask<string> GetGameUrlAsync(
        Uri apiUrl,
        string website,
        string agentId,
        string accessKey,
        PlayerInfo player,
        string gameProviderLanguageCode,
        Guid loginRecordId)
    {
        var encryptedToken = await _tokenProvider.EncryptAsync(loginRecordId);

        var request = new Card365LoginRequest
        {
            PlayerCode = player.PlayerCode,
            AccessKey = accessKey,
            AgentId = agentId,
            Username = player.Username,
            Token = encryptedToken,
            Language = gameProviderLanguageCode,
            EnableFreeGame = false,
            EnableJackpot = false,
            EnableReward = false,
            EnableFullscreen = false
        };

        var json = JsonSerializer.Serialize(request);
        var collection = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        var builder = new StringBuilder();

        foreach (var (key, value) in collection!)
        {
            if (builder.Length > 0)
            {
                builder.Append('&');
            }

            builder
                .Append(key)
                .Append('=')
                .Append(Uri.EscapeDataString(value));
        }

        var queryString = builder.ToString();

        return $"{apiUrl.AbsoluteUri.TrimEnd('/')}/{website}/loginV2?{queryString}";
    }

    private async ValueTask<string> GetKeyAsync(
        Uri apiUrl,
        string cert,
        string website,
        string agentId,
        Guid gameProviderId,
        PlayerInfo playerModel)
    {
        var gameProviderCurrencyCode = await _service
            .GetGameProviderCurrencyCodeAsync(gameProviderId, playerModel.DefaultWalletCurrencyCode)
            .ConfigureAwait(false);

        var request = new Card365GetKeyRequest
        {
            Cert = cert,
            AgentId = agentId,
            PlayerCode = playerModel.PlayerCode,
            Username = playerModel.Username,
            CurrencyCode = gameProviderCurrencyCode
        };

        var response = await _client.GetAccessKeyAsync(apiUrl, website, request)
            .ConfigureAwait(false);

        return response.Status == 1 ? response.Key : null;
    }
}
