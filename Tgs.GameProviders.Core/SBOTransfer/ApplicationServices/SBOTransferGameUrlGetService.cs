using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameInterface.SBOService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SBOTransfer.ApplicationServices;

public class SBOTransferGameUrlGetService : IGetService<SBOTransferGameUrlQuery, Uri>
{
    private readonly IGameInterfaceSBOClient _client;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly IGameRepository _gameRepository;

    public SBOTransferGameUrlGetService(
        IGameInterfaceSBOClient client,
        IGameProviderRepository gameProviderRepository,
        IPlayerRepository playerRepository,
        IBrandRepository brandRepository,
        IGameRepository gameRepository)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
    }

    public async ValueTask<Uri> GetAsync(SBOTransferGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.SBOTransfer).ConfigureAwait(false);
        var player = await _playerRepository.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProviderRepository.GetSBOTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);
        var game = await _gameRepository.GetAsync(query.GameId.Value).ConfigureAwait(false);

        var cultureCode = await _gameProviderRepository
            .GetGameProviderCultureCodeAsync(
                gameProviderId,
                query.Language)
            .ConfigureAwait(false) ?? "en";

        var reply = await GetGameUrlReply(
                configuration.ApiUrl,
                configuration.CompanyKey,
                configuration.Agent,
                player.PlayerCode,
                cultureCode,
                query.PlatformType == PlatformType.Mobile,
                game.GameType,
                cancellationToken)
            .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            await _client
               .PlayerCreateAsync(
                    configuration.ApiUrl,
                    configuration.CompanyKey,
                    configuration.Agent,
                    player.PlayerCode,
                    cancellationToken)
               .ConfigureAwait(false);

            reply = await GetGameUrlReply(
                    configuration.ApiUrl,
                    configuration.CompanyKey,
                    configuration.Agent,
                    player.PlayerCode,
                    cultureCode,
                    query.PlatformType == PlatformType.Mobile,
                    game.GameType,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        return new(reply.GameUrl);
    }

    private ValueTask<GameUrlReply> GetGameUrlReply(
        Uri apiUrl,
        string companyKey,
        string agent,
        string playerCode,
        string cultureCode,
        bool isMobile,
        GameType gameType,
        CancellationToken cancellationToken)
        => gameType is GameType.VirtualSportsLobby or GameType.VirtualSports
            ? _client
                .GameUrlByVirtualSportsAsync(
                    apiUrl,
                    companyKey,
                    agent,
                    playerCode,
                    cultureCode,
                    cancellationToken)
            : _client
                .GameUrlBySportsBookAsync(
                    apiUrl,
                    companyKey,
                    agent,
                    playerCode,
                    cultureCode,
                    isMobile,
                    cancellationToken);
}
