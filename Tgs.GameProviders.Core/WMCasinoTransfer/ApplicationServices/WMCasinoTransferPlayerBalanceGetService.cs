using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices;

public class WMCasinoTransferPlayerBalanceGetService : IGetService<WMCasinoTransferPlayerBalanceQuery, decimal?>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceWMCasinoServiceClient _client;
    private readonly ICommandService<WMCasinoTransferCreatePlayerCommand> _createPlayerCommandService;
    private readonly IGameProviderRepository _gameProvider;
    private readonly ILogger<WMCasinoTransferPlayerBalanceGetService> _logger;
    private readonly IPlayerRepository _player;

    public WMCasinoTransferPlayerBalanceGetService(
        IBrandRepository brand,
        ICommandService<WMCasinoTransferCreatePlayerCommand> createPlayerCommandService,
        IGameProviderRepository gameProvider,
        IGameInterfaceWMCasinoServiceClient client,
        IPlayerRepository player,
        ILogger<WMCasinoTransferPlayerBalanceGetService> logger)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _createPlayerCommandService = createPlayerCommandService ?? throw new ArgumentNullException(nameof(createPlayerCommandService));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<decimal?> GetAsync(WMCasinoTransferPlayerBalanceQuery command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: GameProviderType.WMCasinoTransfer,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var player = await _player
            .GetAsync(
                playerCode: command.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand
            .GetBrandGameProviderConfigurationIdAsync(
                gameProviderId: gameProviderId,
                brandId: player.BrandId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var configuration = await _gameProvider
            .GetWMCasinoTransferConfiguration(
                gameProviderConfigurationId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var user = $"wmtf_{command.PlayerCode}_{player.BrandCode}";
        var response = await _client.GetBalanceAsync(
            apiUrl: configuration.ApiUrl,
            vendorId: configuration.VendorId,
            signature: configuration.Signature,
            user: user,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("GameInterfaceWMCasinoServiceBalanceReply response Success:{Success}, IsPlayerExist:{IsPlayerExist}", response.Success, response.IsPlayerExist);

        if (response.Success && !response.IsPlayerExist)
        {
            await _createPlayerCommandService.ExecuteAsync(new WMCasinoTransferCreatePlayerCommand(
                Configuration: configuration,
                PlayerCode: player.PlayerCode,
                Password: "TgsP@ssword",
                CurrencyCode: player.DefaultWalletCurrencyCode),
                cancellationToken).ConfigureAwait(false);

            response = await _client.GetBalanceAsync(
                apiUrl: configuration.ApiUrl,
                vendorId: configuration.VendorId,
                signature: configuration.Signature,
                user: user,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        return !response.Success
            ? throw new Exception($"This game product has failed to get the balance..{response.Message}")
            : response.Balance;
    }
}
