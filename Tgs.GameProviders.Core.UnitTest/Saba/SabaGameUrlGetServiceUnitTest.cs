using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Saba;
using Tgs.GameProviders.Core.Saba.ApplicationServices;
using Tgs.GameProviders.Core.Saba.Models;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.Saba;

[TestFixture]
public class SabaGameUrlGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process_At_Real_Environment()
    {
        var fakeLogger = NullLogger<SabaGameUrlGetService>.Instance;
        var fakeService = Substitute.For<ISabaService>();
        var fakePlayerService = Substitute.For<ISabaGamePlayer>();
        var fakeUtility = Substitute.For<ISabaUtility>();
        var fakeApiClient = Substitute.For<ISabaApiClient>();

        var returnUrl = new Uri("http://www.google.com");
        var gameProviderLanguageCode = "en";

        var configuration = new SabaConfiguration
        {
            ServiceEndpointUrl = new("http://game-saba"),
            ApiUrl = new("http://t829tsa.bw6688.com"),
            OperatorId = "ZhiFeng055",
            VendorId = "zzdaa12nu1",
            SecretKey = "SecretKey",
            IsTestEnvironment = false
        };

        var gameInfo = new GameInfo(
            600001,
            GameStatus.Active,
            "1",
            null,
            null,
            null,
            Guid.Parse("09d8d244-206b-4030-8110-b2ea90dc9d2f"),
            GameType.Sportsbook,
            GamePlatformType.Desktop,
            false,
            true,
            false,
            true,
            null,
            "Sportsbook",
            "Sportsbook",
            null,
            "SABA");

        var player = new PlayerModel(
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

        var query = new SabaGameUrlQuery
        {
            GameType = gameInfo.GameType,
            GameId = gameInfo.GameId,
            BrandCode = "GPI",
            PlayerCode = player.PlayerCode,
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };

        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        fakeService.GetGameProviderIdAsync().Returns(ValueTask.FromResult(gameInfo.GameProviderId));
        fakeUtility.ToSabaPlayerId(player.PlayerCode, configuration.IsTestEnvironment).Returns(player.PlayerCode);
        fakeApiClient
            .GetGameUrlAsync(
                configuration.ApiUrl,
                Arg.Is<SabaGameUrlRequest>(x =>
                    x.VendorId == configuration.VendorId &&
                    x.PlayerId == player.PlayerCode)
            ).Returns(ValueTask.FromResult(new SabaGameUrlResponse
            {
                ErrorCode = SabaGetGameUrlApiError.Ok,
                Url = returnUrl.AbsoluteUri
            }));

        fakeService.GetGameProviderLanguageCodeAsync(gameInfo.GameProviderId, query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        var gameUrl = new Uri($"{returnUrl.AbsoluteUri}?tsId={query.LoginRecordId}&lang={gameProviderLanguageCode}");

        var sut = new SabaGameUrlGetService(fakeLogger, fakeService, fakePlayerService, fakeUtility, fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }

    [Test]
    public async Task Normal_Process_At_Test_Environment()
    {
        var fakeLogger = NullLogger<SabaGameUrlGetService>.Instance;
        var fakeService = Substitute.For<ISabaService>();
        var fakePlayerService = Substitute.For<ISabaGamePlayer>();
        var fakeUtility = Substitute.For<ISabaUtility>();
        var fakeApiClient = Substitute.For<ISabaApiClient>();

        var returnUrl = new Uri("http://www.google.com");
        var gameProviderLanguageCode = "en";

        var configuration = new SabaConfiguration
        {
            ServiceEndpointUrl = new("http://game-saba"),
            ApiUrl = new("http://t829tsa.bw6688.com"),
            OperatorId = "ZhiFeng055",
            VendorId = "zzdaa12nu1",
            SecretKey = "SecretKey",
            IsTestEnvironment = true
        };

        var gameInfo = new GameInfo(
            600001,
            GameStatus.Active,
            "1",
            null,
            null,
            null,
            Guid.Parse("09d8d244-206b-4030-8110-b2ea90dc9d2f"),
            GameType.Sportsbook,
            GamePlatformType.Desktop,
            false,
            true,
            false,
            true,
            null,
            "Sportsbook",
            "Sportsbook",
            null,
            "SABA");

        var player = new PlayerModel(
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

        var query = new SabaGameUrlQuery
        {
            GameType = gameInfo.GameType,
            GameId = gameInfo.GameId,
            BrandCode = "GPI",
            PlayerCode = player.PlayerCode,
            CurrencyCode = "CNY",
            LoginRecordId = Guid.NewGuid(),
            PlatformType = PlatformType.Desktop,
            Language = "en-US",
            IPAddress = "127.0.0.1",
            BackUrl = new("https://dev-t1.rpgp.cc/")
        };

        var playerId = $"{player.PlayerCode}_test";

        fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        fakeService.GetGameProviderIdAsync().Returns(ValueTask.FromResult(gameInfo.GameProviderId));
        fakeUtility.ToSabaPlayerId(player.PlayerCode, configuration.IsTestEnvironment).Returns(playerId);
        fakeApiClient
            .GetGameUrlAsync(
                configuration.ApiUrl,
                Arg.Is<SabaGameUrlRequest>(x =>
                    x.VendorId == configuration.VendorId &&
                    x.PlayerId == playerId)
            ).Returns(ValueTask.FromResult(new SabaGameUrlResponse
            {
                ErrorCode = SabaGetGameUrlApiError.Ok,
                Url = returnUrl.AbsoluteUri
            }));

        fakeService.GetGameProviderLanguageCodeAsync(gameInfo.GameProviderId, query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        var gameUrl = new Uri($"{returnUrl.AbsoluteUri}?tsId={query.LoginRecordId}&lang={gameProviderLanguageCode}");

        var sut = new SabaGameUrlGetService(fakeLogger, fakeService, fakePlayerService, fakeUtility, fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual.Url);
    }
}
