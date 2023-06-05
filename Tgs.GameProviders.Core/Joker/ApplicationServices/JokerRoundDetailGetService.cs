using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Joker.Client;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Joker.ApplicationServices;

internal class JokerRoundDetailGetService : IGetService<JokerRoundDetailQuery, RoundDetailResult>
{
    private readonly IGameInterfaceJokerClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IRoundRepository _round;

    public JokerRoundDetailGetService(
        IGameProviderRepository gameProvider,
        IGameInterfaceJokerClient client,
        IRoundRepository round)
    {
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _round = round ?? throw new ArgumentNullException(nameof(round));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        JokerRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundInfo = await _round
            .GetAsync(
                roundId: query.RoundId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var configuration = await _gameProvider
            .GetJokerConfigurationAsync(
                gameProviderConfigurationId: roundInfo.GameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var cultureCode = await _gameProvider
            .GetGameProviderCultureCodeAsync(
                gameProviderId: roundInfo.GameProviderId,
                cultureCode: query.Language,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false) ?? "en";

        var url = await _client
            .GameRecordDetailsAsync(
                apiUrl: configuration.ApiUrl,
                appId: configuration.AppId,
                secretKey: configuration.SecretKey,
                externalRoundId: roundInfo.ExternalRoundId,
                language: cultureCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new RoundDetailResult
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataUrl
                {
                    Url = url??string.Empty
                }
            }
        };
    }
}
