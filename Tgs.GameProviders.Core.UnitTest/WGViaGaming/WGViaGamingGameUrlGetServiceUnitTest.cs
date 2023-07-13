using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.WGViaGaming.Client;
using Tgs.GameProviders.Core.WGViaGaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.WGViaGaming;

[TestFixture]
public class WGViaGamingGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameInterfaceWGViaGamingClient>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandRepository = Substitute.For<IBrandRepository>();

        var query = new WGViaGamingGameUrlQuery
        {
            GameType = GameType.Slot,
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

        var cultureCode = "2";
        _ = fakeGameProviderRepository
            .GetGameProviderCultureCodeAsync(
            gameProviderId: Arg.Is(gameProviderId),
            cultureCode: Arg.Is(query.Language),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(cultureCode);

        var currencyCode = "RMB";
        _ = fakeGameProviderRepository
            .GetGameProviderCurrencyCodeAsync(
            gameProviderId: Arg.Is(gameProviderId),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(currencyCode);

        var gameurl = "https://localhost";
        _ = fakeClient
            .GameUrlAsync(
            apiUrl: Arg.Is(configurationData.ApiUrl),
            partnerCode: Arg.Is(configurationData.PartnerCode),
            secretKey: Arg.Is(configurationData.PartnerKey),
            secretIV: Arg.Is(configurationData.IV),
            playerCode: Arg.Is(player.PlayerCode),
            currencyCode: Arg.Is(currencyCode),
            lang: Arg.Is(cultureCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameurl);

        var sut = new WGViaGamingGameUrlGetService(
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandRepository,
            fakeClient);

        var actual = await sut
            .GetAsync(
            query: query)
            .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(new Uri(gameurl), actual);
    }
}
