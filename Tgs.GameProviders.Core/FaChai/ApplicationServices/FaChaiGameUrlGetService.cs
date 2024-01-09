using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.FaChai.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.FaChai.ApplicationServices;

public class FaChaiGameUrlGetService(
    IGameProviderRepository gameProvider,
    IPlayerRepository player,
    IBrandRepository brand,
    IGameRepository game,
    IGameInterfaceFaChaiClient client) : IGetService<FaChaiGameUrlQuery, Uri>
{
    public async ValueTask<Uri> GetAsync(FaChaiGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await gameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: GameProviderType.FaChai,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var playerInfo = await player
            .GetAsync(
                playerCode: query.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderConfigurationId = await brand
            .GetBrandGameProviderConfigurationIdAsync(
                gameProviderId: gameProviderId,
                brandId: playerInfo.BrandId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var configuration = await gameProvider
            .GetFaChaiConfigurationAsync(
                gameProviderConfigurationId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var cultureCode = await gameProvider.GetGameProviderCultureCodeAsync(
                gameProviderId: gameProviderId,
                cultureCode: query.Language,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false) ?? "1";
        var gameInfo = await game.GetAsync(
            gameId: query.GameId.Value,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var response = await client.GameUrlAsync(
            apiUrl: configuration.ApiUrl,
            agentCode: configuration.AgentCode,
            agentKey: configuration.AgentKey,
            currency: configuration.Currency,
            playerCode: playerInfo.PlayerCode,
            gameId: int.Parse(gameInfo.GameExternalId),
            languageId: int.Parse(string.IsNullOrEmpty(cultureCode) ? "1" : cultureCode),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new Uri(response);
    }
}
