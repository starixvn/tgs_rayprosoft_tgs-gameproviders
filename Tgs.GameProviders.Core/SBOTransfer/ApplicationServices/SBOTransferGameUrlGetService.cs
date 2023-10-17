using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameInterface.SBOService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SBOTransfer.ApplicationServices;

public class SBOTransferGameUrlGetService : IGetService<SBOTransferGameUrlQuery, Uri>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IGameInterfaceSBOClient _client;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;

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
        var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.SBOTransfer,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var player = await _playerRepository.GetAsync(
            playerCode: query.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: gameProviderId,
            player.BrandId,
            cancellationToken)
            .ConfigureAwait(false);
        var configuration = await _gameProviderRepository.GetSBOTransferConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var game = await _gameRepository.GetAsync(
            gameId: query.GameId.Value,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var cultureCode = await _gameProviderRepository.GetGameProviderCultureCodeAsync(
            gameProviderId: gameProviderId,
            cultureCode: query.Language,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false) ?? "en";

        var reply = await GetGameUrlReply(
            apiUrl: configuration.ApiUrl,
            companyKey: configuration.CompanyKey,
            agent: configuration.Agent,
            playerCode: player.PlayerCode,
            cultureCode: cultureCode,
            isMobile: query.PlatformType == PlatformType.Mobile,
            gameType: game.Type,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            await _client.PlayerCreateAsync(
                apiUrl: configuration.ApiUrl,
                companyKey: configuration.CompanyKey,
                agent: configuration.Agent,
                playerCode: player.PlayerCode,
                cancellationToken: cancellationToken)
               .ConfigureAwait(false);

            reply = await GetGameUrlReply(
                apiUrl: configuration.ApiUrl,
                companyKey: configuration.CompanyKey,
                agent: configuration.Agent,
                playerCode: player.PlayerCode,
                cultureCode: cultureCode,
                isMobile: query.PlatformType == PlatformType.Mobile,
                gameType: game.Type,
                cancellationToken: cancellationToken)
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
        ? _client.GameUrlByVirtualSportsAsync(
            apiUrl: apiUrl,
            companyKey: companyKey,
            agent: agent,
            playerCode: playerCode,
            lang: cultureCode,
            cancellationToken: cancellationToken)
        : _client.GameUrlBySportsBookAsync(
            apiUrl: apiUrl,
            companyKey: companyKey,
            agent: agent,
            playerCode: playerCode,
            lang: cultureCode,
            isMobile: isMobile,
            cancellationToken: cancellationToken);
}
