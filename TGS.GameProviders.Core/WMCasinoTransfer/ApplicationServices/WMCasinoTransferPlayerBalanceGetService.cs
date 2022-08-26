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
    private readonly IBrandService _brand;
    private readonly ICommandService<WMCasinoTransferCreatePlayerCommand> _createPlayerCommandService;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameInterfaceWMCasinoServiceGrpc _grpc;
    private readonly ILogger<WMCasinoTransferPlayerBalanceGetService> _logger;
    private readonly IPlayerRepository _player;
    private readonly ISettingRepository _setting;

    public WMCasinoTransferPlayerBalanceGetService(
        IBrandService brand,
        ICommandService<WMCasinoTransferCreatePlayerCommand> createPlayerCommandService,
        IGameProviderRepository gameProvider,
        IGameInterfaceWMCasinoServiceGrpc grpc,
        IPlayerRepository player,
        ISettingRepository setting,
        ILogger<WMCasinoTransferPlayerBalanceGetService> logger)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _createPlayerCommandService = createPlayerCommandService ?? throw new ArgumentNullException(nameof(createPlayerCommandService));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<decimal?> GetAsync(WMCasinoTransferPlayerBalanceQuery command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.WMCasinoTransfer).ConfigureAwait(false);
        var configuration = await _setting.GetWMCasinoTransferConfigurationAsync().ConfigureAwait(false);
        var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configurationData = await _gameProvider.GetWMCasinoTransferConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

        var user = $"wmtf_{command.PlayerCode}_{player.BrandCode}";
        var response = await _grpc.GetBalanceAsync(
            apiUrl: configuration.ApiUrl,
            vendorId: configurationData.VendorId,
            signature: configurationData.Signature,
            user: user,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("GameInterfaceWMCasinoServiceBalanceReply response Success:{Success}, IsPlayerExist:{IsPlayerExist}", response.Success, response.IsPlayerExist);

        if (response.Success && !response.IsPlayerExist)
        {
            await _createPlayerCommandService.ExecuteAsync(new WMCasinoTransferCreatePlayerCommand(
                Configuration: configuration,
                ConfigurationData: configurationData,
                PlayerCode: player.PlayerCode,
                Password: "TgsP@ssword",
                CurrencyCode: player.DefaultWalletCurrencyCode),
                cancellationToken).ConfigureAwait(false);

            response = await _grpc.GetBalanceAsync(
                apiUrl: configuration.ApiUrl,
                vendorId: configurationData.VendorId,
                signature: configurationData.Signature,
                user: user,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        return !response.Success
            ? throw new Exception($"This game product has failed to get the balance..{response.Message}")
            : response.Balance;
    }
}
