using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.CrPati.Client;
using Tgs.GameInterface.CrPati.Protos;
using Tgs.GameProviders.Core.CrPati.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.CrPati;

public class CrPatiGameUrlGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameInterfaceCrPatiClient>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakePlayer = Substitute.For<IPlayerRepository>();
        var fakeBrand = Substitute.For<IBrandRepository>();

        var query = new CrPatiGameUrlQuery
        {
            GameType = GameType.LotteryLobby,
            GameId = 1,
            BrandCode = "GPI",
            PlayerCode = "test",
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("http://localhost/")
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider.GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.CrPati),
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
            DefaultWalletCurrencyCode: "HKD");
        _ = fakePlayer
            .GetAsync(
            Arg.Is(query.PlayerCode),
            Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrand
            .GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: Arg.Is(gameProviderId),
            brandId: Arg.Is(player.BrandId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(gameProviderConfigurationId));

        var configuration = new CrPatiConfiguration
        {
            ApiUrl = new("http://localhost/"),
            ApiKey = "apiKey",
            ApiSecret = "apiSecret"
        };
        _ = fakeGameProvider
            .GetCrPatiConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>()).
            Returns(ValueTask.FromResult(configuration));

        var cultureCode = "0";
        _ = fakeGameProvider
            .GetGameProviderCultureCodeAsync(
                gameProviderId: Arg.Is(gameProviderId),
                cultureCode: Arg.Is(query.Language),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(cultureCode);

        var gameurl = new GameUrlReply
        {
            IsPlayerExist = true,
            GameUrl = "http://localhost/"
        };
        _ = fakeClient
            .GameUrlAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            apiKey: Arg.Is(configuration.ApiKey),
            apiSecret: Arg.Is(configuration.ApiSecret),
            playerCode: Arg.Is(player.PlayerCode),
            ipAddress: Arg.Is(query.IPAddress),
            lang: Arg.Is(0),
            isMobile: Arg.Is(false),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameurl);

        var sut = new CrPatiGameUrlGetService(
            fakeGameProvider,
            fakePlayer,
            fakeBrand,
            fakeClient);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.Equal(gameurl.GameUrl, actual);
    }
}
