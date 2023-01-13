using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGamingTransfer.ApplicationServices;

public class DreamGamingTransferPlayerBalanceGetService : IGetService<DreamGamingTransferPlayerBalanceQuery, decimal?>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceDreamGamingClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public DreamGamingTransferPlayerBalanceGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceDreamGamingClient client,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<decimal?> GetAsync(DreamGamingTransferPlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.DreamGamingTransfer).ConfigureAwait(false);
        var player = await _player.GetAsync(query.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetDreamGamingTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);
        var gameProviderCurrencyCode = await _gameProvider.GetGameProviderCurrencyCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode).ConfigureAwait(false);

        var reply = await _client
            .PlayerBalanceAsync(
                apiUrl: configuration.ApiUrl,
                apiKey: configuration.ApiKey,
                agentName: configuration.AgentName,
                username: player.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            await _client
                .PlayerCreateAsync(
                    apiUrl: configuration.ApiUrl,
                    apiKey: configuration.ApiKey,
                    agentName: configuration.AgentName,
                    username: player.PlayerCode,
                    password: "DGTFP@ssw0rd",
                    currencyName: gameProviderCurrencyCode,
                    tableLimit: configuration.TableLimit,
                    winLimit: 0,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            reply = await _client
               .PlayerBalanceAsync(
                    apiUrl: configuration.ApiUrl,
                    apiKey: configuration.ApiKey,
                    agentName: configuration.AgentName,
                    username: player.PlayerCode,
                    cancellationToken: cancellationToken)
               .ConfigureAwait(false);
        }

        return reply.IsPlayerExist ? reply.Balance : null;
    }
}
