using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.WGViaGaming.Client;
using Tgs.GameInterface.WGViaGaming.Protos;
using Tgs.GameProviders.Core.WGViaGaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.WGViaGaming;

public class WGViaGamingPlayerBalanceGetServiceUnitTest
{
    [Fact]
    public void Create_Player()
    {
        var fakeClient = Substitute.For<IGameInterfaceWGViaGamingClient>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandRepository = Substitute.For<IBrandRepository>();

        var query = new WGViaGamingPlayerBalanceQuery
        {
            PlayerCode = "test"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.WGViaGaming),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(gameProviderId));

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "test",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakePlayerRepository
            .GetAsync(
            Arg.Is(query.PlayerCode),
            Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandRepository
            .GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: Arg.Is(gameProviderId),
            brandId: Arg.Is(player.BrandId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(gameProviderConfigurationId));

        var configurationData = new WGViaGamingConfiguration
        {
            ApiUrl = new("http://localhost/"),
            PartnerCode = "PartnerCode",
            PartnerKey = "PartnerKey",
            IV = "IV"
        };
        _ = fakeGameProviderRepository
            .GetWGViaGamingConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>()).
            Returns(ValueTask.FromResult(configurationData));

        var playerBalanceReply = new PlayerBalanceReply
        {
            IsPlayerExist = false,
            Balance = 0
        };
        _ = fakeClient
            .PlayerBalanceAsync(
            apiUrl: Arg.Is(configurationData.ApiUrl),
            partnerCode: Arg.Is(configurationData.PartnerCode),
            secretKey: Arg.Is(configurationData.PartnerKey),
            secretIV: Arg.Is(configurationData.IV),
            playerCode: Arg.Is(player.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(playerBalanceReply);

        var currencyCode = "RMB";
        _ = fakeGameProviderRepository
            .GetGameProviderCurrencyCodeAsync(
            gameProviderId: Arg.Is(gameProviderId),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(currencyCode);

        var sut = new WGViaGamingPlayerBalanceGetService(
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandRepository,
            fakeClient);

        _ = Assert.ThrowsAsync<Exception>(async ()
            => await sut
            .GetAsync(
                query: query)
            .ConfigureAwait(false));

        _ = fakeClient
            .Received(1)
            .PlayerCreateAsync(
            apiUrl: Arg.Is(configurationData.ApiUrl),
            partnerCode: Arg.Is(configurationData.PartnerCode),
            secretKey: Arg.Is(configurationData.PartnerKey),
            secretIV: Arg.Is(configurationData.IV),
            playerCode: Arg.Is(player.PlayerCode),
            currencyCode: Arg.Is(currencyCode),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameInterfaceWGViaGamingClient>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandRepository = Substitute.For<IBrandRepository>();

        var query = new WGViaGamingPlayerBalanceQuery
        {
            PlayerCode = "test"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.WGViaGaming),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(gameProviderId));

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "test",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakePlayerRepository
            .GetAsync(
            Arg.Is(query.PlayerCode),
            Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandRepository
            .GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: Arg.Is(gameProviderId),
            brandId: Arg.Is(player.BrandId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(gameProviderConfigurationId));

        var configurationData = new WGViaGamingConfiguration
        {
            ApiUrl = new("http://localhost/"),
            PartnerCode = "PartnerCode",
            PartnerKey = "PartnerKey",
            IV = "IV"
        };
        _ = fakeGameProviderRepository
            .GetWGViaGamingConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>()).
            Returns(ValueTask.FromResult(configurationData));

        var playerBalanceReply = new PlayerBalanceReply
        {
            IsPlayerExist = true,
            Balance = 1
        };
        _ = fakeClient
            .PlayerBalanceAsync(
            apiUrl: Arg.Is(configurationData.ApiUrl),
            partnerCode: Arg.Is(configurationData.PartnerCode),
            secretKey: Arg.Is(configurationData.PartnerKey),
            secretIV: Arg.Is(configurationData.IV),
            playerCode: Arg.Is(player.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(playerBalanceReply);

        var sut = new WGViaGamingPlayerBalanceGetService(
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandRepository,
            fakeClient);

        var actual = await sut
            .GetAsync(
            query: query)
            .ConfigureAwait(false);

        Assert.Equal(1, actual.Balance);
    }
}
