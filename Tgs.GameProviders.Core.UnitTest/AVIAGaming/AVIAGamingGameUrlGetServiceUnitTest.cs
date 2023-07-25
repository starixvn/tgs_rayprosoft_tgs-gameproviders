using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.AVIAGaming.Client;
using Tgs.GameInterface.AVIAGaming.Protos;
using Tgs.GameProviders.Core.AVIAGaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.AVIAGaming;

[TestFixture]
public class AVIAGamingGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Create_PlayerAsync()
    {
        var fakeClient = Substitute.For<IGameInterfaceAVIAGamingClient>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakePlayer = Substitute.For<IPlayerRepository>();
        var fakeBrand = Substitute.For<IBrandRepository>();

        var query = new AVIAGamingGameUrlQuery
        {
            GameType = GameType.ESports,
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
            gameProviderCode: Arg.Is(GameProviderType.AVIAGaming),
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

        var configuration = new AVIAGamingConfiguration
        {
            AmountMaximum = 10000000,
            AmountFormat = "10,2",
            ApiUrl = new Uri("http://localhost/"),
            Authorization = "authorization",
            PlayerPassword = "TgsP@ssw0rd"
        };
        _ = fakeGameProvider
            .GetAVIAGamingConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>()).
            Returns(ValueTask.FromResult(configuration));

        var cultureCode = "ENG";
        _ = fakeGameProvider
            .GetGameProviderCultureCodeAsync(
            gameProviderId: gameProviderId,
            cultureCode: query.Language,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(cultureCode);

        var gameurl = new GameUrlReply
        {
            IsPlayerExist = false,
            GameUrl = string.Empty
        };
        _ = fakeClient
            .GameUrlAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            authorization: Arg.Is(configuration.Authorization),
            playerCode: Arg.Is(player.PlayerCode),
            lang: Arg.Is(cultureCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameurl);

        var currency = "CNY";
        _ = fakeGameProvider
            .GetGameProviderCurrencyCodeAsync(
            gameProviderId: Arg.Is(gameProviderId),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(currency);

        var sut = new AVIAGamingGameUrlGetService(
            fakeGameProvider,
            fakePlayer,
            fakeBrand,
            fakeClient);

        _ = await sut
           .GetAsync(
           query: query)
           .ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .PlayerCreateAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
            authorization: Arg.Is(configuration.Authorization),
            playerCode: Arg.Is(player.PlayerCode),
            currency: Arg.Is(currency),
            password: Arg.Is(configuration.PlayerPassword),
            cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Test]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameInterfaceAVIAGamingClient>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakePlayer = Substitute.For<IPlayerRepository>();
        var fakeBrand = Substitute.For<IBrandRepository>();

        var query = new AVIAGamingGameUrlQuery
        {
            GameType = GameType.ESports,
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
            gameProviderCode: Arg.Is(GameProviderType.AVIAGaming),
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

        var configuration = new AVIAGamingConfiguration
        {
            AmountMaximum = 10000000,
            AmountFormat = "10,2",
            ApiUrl = new Uri("http://localhost/"),
            Authorization = "authorization",
            PlayerPassword = "TgsP@ssw0rd"
        };
        _ = fakeGameProvider
            .GetAVIAGamingConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>()).
            Returns(ValueTask.FromResult(configuration));

        var cultureCode = "ENG";
        _ = fakeGameProvider
            .GetGameProviderCultureCodeAsync(
            gameProviderId: gameProviderId,
            cultureCode: query.Language,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(cultureCode);

        var gameurl = new GameUrlReply
        {
            IsPlayerExist = true,
            GameUrl = "https://localhost"
        };
        _ = fakeClient
            .GameUrlAsync(
            apiUrl: Arg.Is(configuration.ApiUrl),
             authorization: Arg.Is(configuration.Authorization),
             playerCode: Arg.Is(player.PlayerCode),
             lang: Arg.Is(cultureCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameurl);

        var sut = new AVIAGamingGameUrlGetService(
            fakeGameProvider,
            fakePlayer,
            fakeBrand,
            fakeClient);

        var actual = await sut
            .GetAsync(
            query: query)
            .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameurl.GameUrl, actual);
    }
}
