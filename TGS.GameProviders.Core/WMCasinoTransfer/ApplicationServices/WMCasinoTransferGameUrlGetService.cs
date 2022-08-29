using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices;

public class WMCasinoTransferGameUrlGetService : IGetService<WMCasinoTransferGameUrlQuery, GameUrlResult>
{
    private readonly IBrandService _brand;
    private readonly ICommandService<WMCasinoTransferCreatePlayerCommand> _createPlayerCommandService;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameInterfaceWMCasinoServiceGrpc _grpc;
    private readonly IPlayerRepository _player;
    private readonly ISettingRepository _setting;

    public WMCasinoTransferGameUrlGetService(
        IBrandService brand,
        ICommandService<WMCasinoTransferCreatePlayerCommand> createPlayerCommandService,
        IGameProviderRepository gameProvider,
        IGameInterfaceWMCasinoServiceGrpc grpc,
        IPlayerRepository player,
        ISettingRepository setting)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _createPlayerCommandService = createPlayerCommandService ?? throw new ArgumentNullException(nameof(createPlayerCommandService));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
    }

    public async ValueTask<GameUrlResult> GetAsync(WMCasinoTransferGameUrlQuery command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.WMCasinoTransfer).ConfigureAwait(false);
        var configuration = await _setting.GetWMCasinoTransferConfigurationAsync().ConfigureAwait(false);
        var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configurationData = await _gameProvider.GetWMCasinoTransferConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);
        var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, command.Language) ?? "0";

        var password = "TgsP@ssword";
        var user = $"wmtf_{command.PlayerCode}_{player.BrandCode}";

        var response = await _grpc.GameUrlAsync(
            apiUrl: configuration.ApiUrl,
            vendorId: configurationData.VendorId,
            signature: configurationData.Signature,
            user: user,
            password: password,
            language: int.Parse(cultureCode),
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (response.Success && !response.IsPlayerExist)
        {
            await _createPlayerCommandService.ExecuteAsync(new WMCasinoTransferCreatePlayerCommand(
                Configuration: configuration,
                ConfigurationData: configurationData,
                PlayerCode: player.PlayerCode,
                CurrencyCode: player.DefaultWalletCurrencyCode),
                cancellationToken).ConfigureAwait(false);

            response = await _grpc.GameUrlAsync(
                apiUrl: configuration.ApiUrl,
                vendorId: configurationData.VendorId,
                signature: configurationData.Signature,
                user: user,
                password: password,
                language: 0,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        return (response != null) ? new() { Url = new(response.GameUrl) } : null;
    }
}
