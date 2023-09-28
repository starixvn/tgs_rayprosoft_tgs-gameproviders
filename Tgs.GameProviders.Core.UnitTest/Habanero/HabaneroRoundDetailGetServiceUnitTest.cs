using System;
using System.Threading.Tasks;
using ExpectedObjects;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core.Habanero;
using Tgs.GameProviders.Core.Habanero.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.UnitTest.Habanero;

public class HabaneroRoundDetailGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<HabaneroRoundDetailGetService>.Instance;
        var fakeRoundService = Substitute.For<IRoundRepository>();
        var fakeService = Substitute.For<IHabaneroService>();

        var externalRoundId = "1639679431950180036";
        var gameProviderLanguageCode = "en-US";
        var historyUrl = "https://app-test.insvr.com/games/history?brandId=a5df0ada-7b6a-eb11-9889-00155db5435a&friendlyid=1639679431950180036&viewtype=game&local=en-US&hash=4c10361f46949c358108ae115e7900a214cbcf68ad5c7f13afdb4ed9be39c683";

        var configuration = new HabaneroConfiguration
        {
            ServiceEndpointUrl = new Uri("http://game-haba"),
            BrandId = "a5df0ada-7b6a-eb11-9889-00155db5435a",
            ApiKey = "349F896E-DBC8-4A9D-963D-EFC921AD19FC",
            ApiUrl = new("https://ws-test.insvr.com"),
            BetHistoryUrl = new("https://app-test.insvr.com")
        };

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

        var gameProviderInfo = new GameProviderInfo(
            GameProviderId: Guid.Parse("de301075-182b-453f-a316-28e3164b2d06"),
            GameProviderCode: GameProviderType.Habanero,
            UniqueId: 1,
            Name: GameProviderType.Habanero,
            TransferWallet: false,
            AutoCloseRound: true,
            SupportTestPlayer: false,
            AllowKickPlayer: true,
            PlaceBetTimeout: 0,
            SettleBetTimeout: 0,
             Description: GameProviderType.Habanero,
            Localization: null);

        var gameInfo = new GameInfo(
            GameId: 1,
            GameCode: "test",
            GameProviderId: gameProviderInfo.GameProviderId,
            GameProviderCode: GameProviderType.Habanero,
            GameExternalId: "1",
            Type: GameType.Slot,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "test",
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

        var roundInfo = new RoundInfo(
            Guid.Parse("5566e31a-80ec-46eb-522d-08d9c07bf52d"),
            externalRoundId,
            gameProviderInfo.GameProviderId,
            player.PlayerCode,
            gameInfo.GameId,
            player.DefaultWalletCurrencyCode,
            GameRoundStatus.Closed,
            Guid.NewGuid(),
            player.DefaultWalletId,
            DateTime.Now.AddMinutes(-1),
            DateTime.Now,
            DateTime.Now,
            player.BrandId,
            DateTime.Now.AddMinutes(-1),
            DateTime.Now,
            20m,
            20m,
            45m,
            null,
            20m,
            -25m,
            null,
            GameRoundClientStatus.Win,
            null,
            null,
            null,
            null);

        var sut = new HabaneroRoundDetailGetService(fakeLogger, fakeService, fakeRoundService);

        var query = new HabaneroRoundDetailQuery
        {
            RoundId = roundInfo.RoundId,
            Language = "en-US"
        };

        _ = fakeRoundService.GetAsync(query.RoundId).Returns(ValueTask.FromResult<RoundInfo?>(roundInfo));
        _ = fakeService.GetGameProviderAsync(roundInfo.GameProviderId).Returns(ValueTask.FromResult(gameProviderInfo));
        _ = fakeService.GetGameAsync(roundInfo.GameId).Returns(ValueTask.FromResult(gameInfo));
        _ = fakeService.GetPlayerAsync(roundInfo.PlayerCode).Returns(ValueTask.FromResult(player));
        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakeService
            .GetGameProviderLanguageCodeAsync(gameProviderInfo.GameProviderId, query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        var expected = new RoundDetailResult
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataUrl
                {
                    Url = historyUrl
                }
            }
        };

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        expected.Data.ToExpectedObject().ShouldEqual(actual.Data);
    }
}
