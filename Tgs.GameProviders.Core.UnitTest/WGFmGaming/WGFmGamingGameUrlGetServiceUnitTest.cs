using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.WGFmGaming;

[TestFixture]
public class WGFmGamingGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceWGFmGamingClient = Substitute.For<IGameInterfaceWGFmGamingClient>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();

        var query = new WGFmGamingGameUrlQuery
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
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.WGSports)).Returns(ValueTask.FromResult(gameProviderId));

        var configuration = new WGSportsConfiguration
        {
            ApiUrl = new("http://localhost/"),
            PartnerCode = "PartnerCode",
            PartnerKey = "PartnerKey",
            IV = "IV"
        };
        _ = fakeSettingRepository.GetWGSportsConfigurationAsync().Returns(ValueTask.FromResult(configuration));

        var playerModel = new PlayerModel(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "test",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY",
            PlayerStatus.Enabled);
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);

        var cultureCode = "2";
        _ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
            Arg.Is(gameProviderId),
            Arg.Is(query.Language)).Returns(cultureCode);

        var currencyCode = "RMB";
        _ = fakeGameProviderRepository.GetGameProviderCurrencyCodeAsync(
            Arg.Is(gameProviderId),
            Arg.Is(query.CurrencyCode)).Returns(currencyCode);

        var token = Guid.NewGuid().ToString("N");
        _ = fakeGameInterfaceWGFmGamingClient
             .GamePrepareLoginAsync(
                 Arg.Is(configuration.ApiUrl),
                 Arg.Is(configuration.PartnerCode),
                 Arg.Is(configuration.PartnerKey),
                 Arg.Is(configuration.IV),
                 Arg.Is(playerModel.PlayerCode),
                 Arg.Is(currencyCode))
             .Returns(token);

        var gameUrl = new Uri("http://localhost/");
        _ = fakeGameInterfaceWGFmGamingClient
            .GameLoginAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.PartnerCode),
                Arg.Is(token),
                Arg.Is(cultureCode),
                Arg.Is(2))
            .Returns(gameUrl);

        var sut = new WGFmGamingGameUrlGetService(
            fakeGameInterfaceWGFmGamingClient,
            fakeSettingRepository,
            fakeGameProviderRepository,
            fakePlayerRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual);
    }
}
