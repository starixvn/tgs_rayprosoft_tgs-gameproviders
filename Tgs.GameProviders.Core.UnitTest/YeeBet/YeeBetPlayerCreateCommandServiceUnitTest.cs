using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.YeeBet.Client;
using Tgs.GameProviders.Core.YeeBet.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.YeeBet;

public class YeeBetPlayerCreateCommandServiceUnitTest
{
    [Fact]
    public async Task Create_Player_By_Configuration_SetCurrency()
    {
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandRepository = Substitute.For<IBrandRepository>();
        var fakeClient = Substitute.For<IGameInterfaceYeeBetClient>();

        var command = new YeeBetPlayerCreateCommand(PlayerCode: "4ucmqplxb029");

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository
            .GetGameProviderIdAsync(
                gameProviderCode: Arg.Is(GameProviderType.YeeBet))
            .Returns(gameProviderId);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");

        _ = fakePlayerRepository
            .GetAsync(Arg.Is(command.PlayerCode))
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandRepository
            .GetBrandGameProviderConfigurationIdAsync(
                 gameProviderId: Arg.Is(gameProviderId),
                 brandId: Arg.Is(player.BrandId))
            .Returns(gameProviderConfigurationId);

        var configuration = new YeeBetConfiguration()
        {
            ApiUrl = new("http://api.98kbus.top/"),
            AppId = "xtdA15OTVGEP",
            SecretKey = "4885366DF30D880224FF6AA46FBDE4F0",
            SetCurrency = "PHP",
            BetLimit = new int[] { 7, 8, 9 }
        };
        _ = fakeGameProviderRepository
            .GetYeeBetConfigurationAsync(Arg.Is(gameProviderConfigurationId))
            .Returns(configuration);

        var currencyCode = configuration.SetCurrency;

        var gameProviderCurrencyCode = "PHP";

        _ = fakeGameProviderRepository
           .GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode)
           .Returns(gameProviderCurrencyCode);

        var sut = new YeeBetPlayerCreateCommandService(
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandRepository,
            fakeClient);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .PlayerCreateByBetLimitAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                appId: Arg.Is(configuration.AppId),
                secretKey: Arg.Is(configuration.SecretKey),
                playerCode: Arg.Is(player.PlayerCode),
                clientType: Arg.Is(1),
                currency: Arg.Is(gameProviderCurrencyCode),
                languageId: Arg.Is(2),
                quotas: Arg.Is(configuration.BetLimit));
    }

    [Fact]
    public async Task Create_Player_By_Default_Wallet_Id()
    {
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandRepository = Substitute.For<IBrandRepository>();
        var fakeClient = Substitute.For<IGameInterfaceYeeBetClient>();

        var command = new YeeBetPlayerCreateCommand(PlayerCode: "4ucmqplxb029");

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository
            .GetGameProviderIdAsync(
                gameProviderCode: Arg.Is(GameProviderType.YeeBet))
            .Returns(gameProviderId);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");

        _ = fakePlayerRepository
            .GetAsync(Arg.Is(command.PlayerCode))
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandRepository
            .GetBrandGameProviderConfigurationIdAsync(
                 gameProviderId: Arg.Is(gameProviderId),
                 brandId: Arg.Is(player.BrandId))
            .Returns(gameProviderConfigurationId);

        var configuration = new YeeBetConfiguration()
        {
            ApiUrl = new("http://api.98kbus.top/"),
            AppId = "xtdA15OTVGEP",
            SecretKey = "4885366DF30D880224FF6AA46FBDE4F0",
            BetLimit = new int[] { 7, 8, 9 }
        };
        _ = fakeGameProviderRepository
            .GetYeeBetConfigurationAsync(Arg.Is(gameProviderConfigurationId))
            .Returns(configuration);

        var currencyCode = player.DefaultWalletCurrencyCode;

        var gameProviderCurrencyCode = "CNY";

        _ = fakeGameProviderRepository
           .GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode)
           .Returns(gameProviderCurrencyCode);

        var sut = new YeeBetPlayerCreateCommandService(
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandRepository,
            fakeClient);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .PlayerCreateByBetLimitAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                appId: Arg.Is(configuration.AppId),
                secretKey: Arg.Is(configuration.SecretKey),
                playerCode: Arg.Is(player.PlayerCode),
                clientType: Arg.Is(1),
                currency: Arg.Is(gameProviderCurrencyCode),
                languageId: Arg.Is(2),
                quotas: Arg.Is(configuration.BetLimit));
    }
}
