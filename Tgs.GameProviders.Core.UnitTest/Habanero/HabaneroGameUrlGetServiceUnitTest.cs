using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Core.Infrastructures.Security;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core.Habanero;
using Tgs.GameProviders.Core.Habanero.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.Habanero;

public class HabaneroGameUrlGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<HabaneroGameUrlGetService>.Instance;
        var fakeTokenProvider = Substitute.For<ITokenProvider>();
        var fakeService = Substitute.For<IHabaneroService>();

        var gameDomain = "https://app-test.insvr.com/go.ashx";
        var playerCode = "4ucmqplxb029";
        var token = "k1tfcjO0NXNeUpVSgEUe4TnactNrhuuQRZRc4QHHwyL7Q8x5LqjIQtgYW3X5GUq0h";
        var gameProviderLanguageCode = "en-US";

        var gameUrl =
            new Uri("https://app-test.insvr.com/go.ashx?brandId=a5df0ada-7b6a-eb11-9889-00155db5435a&token=k1tfcjO0NXNeUpVSgEUe4TnactNrhuuQRZRc4QHHwyL7Q8x5LqjIQtgYW3X5GUq0h&locale=en-US&mode=real&keyname=SGTotemTowers");

        var configuration = new HabaneroConfiguration
        {
            ServiceEndpointUrl = new Uri("http://game-haba"),
            BrandId = "a5df0ada-7b6a-eb11-9889-00155db5435a",
            ApiKey = "349F896E-DBC8-4A9D-963D-EFC921AD19FC",
            ApiUrl = new("https://ws-test.insvr.com"),
            BetHistoryUrl = new("https://app-test.insvr.com")
        };

        var gameInfo = new GameInfo(
            GameId: 1,
            GameCode: "SGTotemTowers",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.Habanero,
            GameExternalId: "SGTotemTowers",
            Type: GameType.Slot,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "SGTotemTowers",
            SubGame: true,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: true,
            Html5: true,
            Description: null,
            EndpointPath: "?brandId={BRANDID}&token={TOKEN}&locale={CULTURE}&mode=real",
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: GameProviderType.Habanero);

        var player = new PlayerInfo(
            Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
            "TGS",
            Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
            "TGS",
            Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
            "GPI",
            "4ucmqplxb029",
            "4ucmqplxb029",
            Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
            "CNY");

        var query = new HabaneroGameUrlQuery
        {
            GameType = gameInfo.Type,
            GameId = gameInfo.GameId,
            BrandCode = "GPI",
            PlayerCode = playerCode,
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };

        _ = fakeTokenProvider.EncryptAsync(query.LoginRecordId).Returns(ValueTask.FromResult(token));
        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakeService.GetPlayerAsync(query.PlayerCode).Returns(ValueTask.FromResult(player));
        _ = fakeService.GetGameAsync(query.GameId!.Value).Returns(ValueTask.FromResult(gameInfo));

        _ = fakeService.GetGameDomainAsync(
                 player.BrandId,
                 gameInfo.GameProviderId,
                 query.PlatformType)
             .Returns(ValueTask.FromResult(gameDomain));

        _ = fakeService
             .GetGameProviderLanguageCodeAsync(gameInfo.GameProviderId, query.Language)
             .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        var sut = new HabaneroGameUrlGetService(fakeLogger, fakeService, fakeTokenProvider);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.Equal(gameUrl, actual.Url);
    }
}
