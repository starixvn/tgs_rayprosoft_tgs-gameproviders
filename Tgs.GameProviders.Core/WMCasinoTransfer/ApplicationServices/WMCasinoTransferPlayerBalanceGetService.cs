using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices;

public class WMCasinoTransferPlayerBalanceGetService : IGetService<WMCasinoTransferPlayerBalanceQuery, PlayerBalanceResult>
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

    public async ValueTask<PlayerBalanceResult> GetAsync(WMCasinoTransferPlayerBalanceQuery command, CancellationToken cancellationToken = default)
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
            .GetWMCasinoTransferConfigurationAsync(
                gameProviderConfigurationId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var user = $"wmtf_{command.PlayerCode}_{player.BrandCode}";
        var reply = await _client.GetBalanceAsync(
            apiUrl: configuration.ApiUrl,
            vendorId: configuration.VendorId,
            signature: configuration.Signature,
            user: user,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        _logger.LogDebug("GameInterfaceWMCasinoServiceBalanceReply response Success:{Success}, IsPlayerExist:{IsPlayerExist}", reply.Success, reply.IsPlayerExist);

        if (reply.Success && !reply.IsPlayerExist)
        {
            await _createPlayerCommandService.ExecuteAsync(new WMCasinoTransferCreatePlayerCommand(
                Configuration: configuration,
                PlayerCode: player.PlayerCode,
                Password: "TgsP@ssword",
                CurrencyCode: player.DefaultWalletCurrencyCode),
                cancellationToken).ConfigureAwait(false);

            reply = await _client.GetBalanceAsync(
                apiUrl: configuration.ApiUrl,
                vendorId: configuration.VendorId,
                signature: configuration.Signature,
                user: user,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        return !reply.IsPlayerExist
            ? throw new Exception($"Player does not exist.{player.PlayerCode}")
            : new PlayerBalanceResult
            {
                FreeBalance = reply.Balance.ToDecimal().GetValueOrDefault(0),
                OccupyBalance = 0
            };
    }
}
