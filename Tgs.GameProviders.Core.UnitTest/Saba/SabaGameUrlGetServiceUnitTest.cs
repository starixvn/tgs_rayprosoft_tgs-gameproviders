using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core.Saba;
using Tgs.GameProviders.Core.Saba.ApplicationServices;
using Tgs.GameProviders.Core.Saba.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.Saba;

public class SabaGameUrlGetServiceUnitTest
{
    [Fact]
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
            GameId: 1,
            GameCode: "test",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.Saba,
            GameExternalId: "1",
            Type: GameType.Slot,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "test",
            SubGame: true,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: true,
            Html5: true,
            Description: null,
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: GameProviderType.Saba);

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

        var query = new SabaGameUrlQuery
        {
            GameType = gameInfo.Type,
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

        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakeService.GetGameProviderIdAsync().Returns(ValueTask.FromResult(gameInfo.GameProviderId));
        _ = fakeUtility.ToSabaPlayerId(player.PlayerCode, configuration.IsTestEnvironment).Returns(player.PlayerCode);
        _ = fakeApiClient
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

        _ = fakeService.GetGameProviderLanguageCodeAsync(gameInfo.GameProviderId, query.Language)
             .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        var gameUrl = new Uri($"{returnUrl.AbsoluteUri}?tsId={query.LoginRecordId}&lang={gameProviderLanguageCode}");

        var sut = new SabaGameUrlGetService(fakeLogger, fakeService, fakePlayerService, fakeUtility, fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(gameUrl, actual.Url);
    }

    [Fact]
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
            GameId: 1,
            GameCode: "test",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.Saba,
            GameExternalId: "1",
            Type: GameType.Slot,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "test",
            SubGame: true,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: true,
            Html5: true,
            Description: null,
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: GameProviderType.Saba);

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

        var query = new SabaGameUrlQuery
        {
            GameType = gameInfo.Type,
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

        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakeService.GetGameProviderIdAsync().Returns(ValueTask.FromResult(gameInfo.GameProviderId));
        _ = fakeUtility.ToSabaPlayerId(player.PlayerCode, configuration.IsTestEnvironment).Returns(playerId);
        _ = fakeApiClient
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

        _ = fakeService.GetGameProviderLanguageCodeAsync(gameInfo.GameProviderId, query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        var gameUrl = new Uri($"{returnUrl.AbsoluteUri}?tsId={query.LoginRecordId}&lang={gameProviderLanguageCode}");

        var sut = new SabaGameUrlGetService(fakeLogger, fakeService, fakePlayerService, fakeUtility, fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(gameUrl, actual.Url);
    }
}
