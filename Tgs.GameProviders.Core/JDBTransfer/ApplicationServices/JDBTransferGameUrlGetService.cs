using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.JDBService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JDBTransfer.ApplicationServices;

public class JDBTransferGameUrlGetService : IGetService<JDBTransferGameUrlQuery, Uri>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IGameInterfaceJDBClient _client;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;

    public JDBTransferGameUrlGetService(
        IGameInterfaceJDBClient client,
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

    public async ValueTask<Uri> GetAsync(JDBTransferGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.JDBTransfer).ConfigureAwait(false);
        var player = await _playerRepository.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProviderRepository.GetJDBTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);
        var game = await _gameRepository.GetAsync(query.GameId.Value).ConfigureAwait(false);

        var cultureCode = await _gameProviderRepository
            .GetGameProviderCultureCodeAsync(
                gameProviderId,
                query.Language)
            .ConfigureAwait(false) ?? "cn";

        var gameSplits = game.GameExternalId.Split("_");

        var reply = await _client
            .GameUrlAsync(
                configuration.ApiUrl,
                configuration.Domain,
                configuration.AesKey,
                configuration.AesIV,
                player.PlayerCode,
                gameSplits[0],
                gameSplits[1],
                cultureCode,
                cancellationToken)
            .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            await _client
               .PlayerCreateAsync(
                   configuration.ApiUrl,
                   configuration.Domain,
                   configuration.AesKey,
                   configuration.AesIV,
                   configuration.Agent,
                   player.PlayerCode,
                   cancellationToken)
               .ConfigureAwait(false);

            reply = await _client
                .GameUrlAsync(
                    configuration.ApiUrl,
                    configuration.Domain,
                    configuration.AesKey,
                    configuration.AesIV,
                    player.PlayerCode,
                    gameSplits[0],
                    gameSplits[1],
                    cultureCode,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        return new(reply.GameUrl);
    }
}
