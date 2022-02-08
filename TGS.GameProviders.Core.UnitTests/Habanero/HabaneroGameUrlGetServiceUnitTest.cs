using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Core.Infrastructures.Security;
using TGS.GameProviders.Core.Habanero;
using TGS.GameProviders.Core.Habanero.ApplicationServices;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.UnitTests.Habanero;

[TestFixture]
public class HabaneroGameUrlGetServiceUnitTest
{
    [Test]
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

        var game = new GameInfo(
            200001,
            GameStatus.Active,
            "SGTotemTowers",
            null,
            "?brandId={BRANDID}&token={TOKEN}&locale={CULTURE}&mode=real",
            null,
            Guid.Parse("de301075-182b-453f-a316-28e3164b2d06"),
            GameType.Slot,
            GamePlatformType.Desktop,
            false,
            true,
            false,
            true,
            null,
            "SGTotemTowers",
            "SGTotemTowers",
            null,
            "HABA");

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
            "CNY",
            PlayerStatus.Enabled);

        var query = new HabaneroGameUrlQuery
        {
            GameType = game.GameType,
            GameId = game.GameId,
            BrandCode = "GPI",
            PlayerCode = playerCode,
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };

        fakeTokenProvider.EncryptAsync(query.LoginRecordId).Returns(ValueTask.FromResult(token));
        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        fakeService.GetPlayerAsync(query.PlayerCode).Returns(ValueTask.FromResult(player));
        fakeService.GetGameAsync(query.GameId!.Value).Returns(ValueTask.FromResult(game));

        fakeService.GetGameDomainAsync(
                player.BrandId,
                game.GameProviderId,
                query.PlatformType)
            .Returns(ValueTask.FromResult(gameDomain));

        fakeService
            .GetGameProviderLanguageCodeAsync(game.GameProviderId, query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        var sut = new HabaneroGameUrlGetService(fakeLogger, fakeService, fakeTokenProvider);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}
