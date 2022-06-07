using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Habanero;
using Tgs.GameProviders.Core.Habanero.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.UnitTest.Habanero;

[TestFixture]
public class HabaneroRoundDetailGetServiceUnitTest
{
    [Test]
    public async ValueTask Normal_Process()
    {
        var fakeLogger = NullLogger<HabaneroRoundDetailGetService>.Instance;
        var fakeRoundService = Substitute.For<IRoundService>();
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

        var gameProviderInfo = new GameProviderModel(
            Guid.Parse("de301075-182b-453f-a316-28e3164b2d06"),
            GameProviderStatus.Active,
            null,
            true,
            true,
            true,
            10,
            5,
            20,
            false,
            "HABA",
            "Habanero",
            null);

        var gameInfo = new GameInfo(
            200001,
            GameStatus.Active,
            "SGTotemTowers",
            null,
            "?brandId={BRANDID}&token={TOKEN}&locale={CULTURE}&mode=real",
            null,
            gameProviderInfo.GameProviderId,
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

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        CollectionAssert.AreEqual(expected.Data, actual.Data);
    }
}
