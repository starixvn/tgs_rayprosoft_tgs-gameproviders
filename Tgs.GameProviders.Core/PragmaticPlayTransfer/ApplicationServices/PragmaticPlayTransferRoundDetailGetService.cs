#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.PragmaticPlay.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

internal class PragmaticPlayTransferRoundDetailGetService
    : IGetService<PragmaticPlayTransferRoundDetailQuery, RoundDetailResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfacePragmaticPlayClient _client;
    private readonly IGameRepository _game;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;
    private readonly IRoundService _round;

    public PragmaticPlayTransferRoundDetailGetService(
        IRoundService round,
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IBrandRepository brand,
        IGameRepository game,
        IGameInterfacePragmaticPlayClient client)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _game = game ?? throw new ArgumentNullException(nameof(game));
        _round = round ?? throw new ArgumentNullException(nameof(round));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        PragmaticPlayTransferRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundInfo = await _round
            .GetAsync(
                query.RoundId)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game round number could not be found.{query.RoundId}");

        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: GameProviderType.PragmaticPlayTransfer,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var player = await _player
            .GetAsync(
                playerCode: roundInfo.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand
            .GetBrandGameProviderConfigurationIdAsync(
                gameProviderId: gameProviderId,
                brandId: player.BrandId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var configuration = await _gameProvider
            .GetPragmaticPlayTransferConfigurationAsync(
                gameProviderConfigurationId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var cultureCode = await _gameProvider
            .GetGameProviderCultureCodeAsync(
                gameProviderId: gameProviderId,
                cultureCode: query.Language,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false) ?? "en";

        var gmae = await _game
            .GetAsync(
                roundInfo.GameId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game does not exist.{roundInfo.GameId}");

        var reply = await _client
            .GameRecordDetailAsync(
                apiUrl: configuration.ApiUrl,
                secureLogin: configuration.SecureLogin,
                secretKey: configuration.SecretKey,
                playerCode: roundInfo.PlayerCode,
                externalRoundId: roundInfo.ExternalRoundId,
                externalGameId: gmae.GameExternalId,
                language: cultureCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new RoundDetailResult
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataUrl
                {
                    Url = reply.Url
                }
            }
        };
    }
}
