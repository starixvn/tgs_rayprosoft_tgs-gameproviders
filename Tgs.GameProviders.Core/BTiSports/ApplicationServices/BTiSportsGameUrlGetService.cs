using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.BTiService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.BTiSports.ApplicationServices;

public class BTiSportsGameUrlGetService : IGetService<BTiSportsGameUrlQuery, Uri>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceBTiClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public BTiSportsGameUrlGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceBTiClient client,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<Uri> GetAsync(BTiSportsGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.BTiSports).ConfigureAwait(false);
        var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetBTiSportsConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);
        var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, query.Language) ?? "en";

        var response = await _client
            .GameAuthTokenAsync(
                apiUrl: configuration.ApiUrl,
                agentUserName: configuration.AgentUserName,
                agentPassword: configuration.AgentPassword,
                merchantCustomerCode: player.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsPlayerExist)
        {
            var gameProviderCurrencyCode = await _gameProvider
                    .GetGameProviderCurrencyCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode)
                    .ConfigureAwait(false)
                ?? throw new Exception($"This game product currency does not exist.({player.DefaultWalletCurrencyCode})");

            await _client
                .PlayerCreateAsync(
                    apiUrl: configuration.ApiUrl,
                    agentUserName: configuration.AgentUserName,
                    agentPassword: configuration.AgentPassword,
                    brandCode: player.BrandCode,
                    merchantCustomerCode: player.PlayerCode,
                    currency: gameProviderCurrencyCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            response = await _client
               .GameAuthTokenAsync(
                   apiUrl: configuration.ApiUrl,
                   agentUserName: configuration.AgentUserName,
                   agentPassword: configuration.AgentPassword,
                   merchantCustomerCode: player.PlayerCode,
                   cancellationToken: cancellationToken)
               .ConfigureAwait(false);
        }

        if (!response.IsPlayerExist || string.IsNullOrWhiteSpace(response.AuthToken))
        {
            throw new Exception($"Game provider failed to obtain Token.");
        }

        var layout = query.PlatformType == PlatformType.Desktop ? configuration.DesktopLayout : configuration.MobileLayout;

        return new Uri(configuration.SportsbookURL, $"/{cultureCode}/{layout}/?operatorToken={response.AuthToken}");
    }
}
