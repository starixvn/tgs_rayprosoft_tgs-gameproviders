using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.Habanero;
using TGS.GameProviders.Core.Habanero.ApplicationServices;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Core.Repositories.Models.Player;
using TGS.GameProviders.Core.Repositories.Models.Rounds;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.RoundDetail;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.UnitTests.Habanero;

[TestFixture]
public class HabaneroRoundDetailGetServiceUnitTest
{
    [Test]
    public async ValueTask Normal_Process()
    {
        var fakeLogger = NullLogger<HabaneroRoundDetailGetService>.Instance;
        var fakeRoundRepository = Substitute.For<IRoundRepository>();
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

        var playerModel = new PlayerModel(
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

        var gameProviderModel = new GameProviderModel(
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

        var gameModel = new GameModel(
            200001,
            GameStatus.Active,
            "SGTotemTowers",
            null,
            "?brandId={BRANDID}&token={TOKEN}&locale={CULTURE}&mode=real",
            null,
            gameProviderModel.GameProviderId,
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

        var roundModel = new RoundModel(
            Guid.Parse("5566e31a-80ec-46eb-522d-08d9c07bf52d"),
            externalRoundId,
            gameProviderModel.GameProviderId,
            playerModel.PlayerCode,
            gameModel.GameId,
            playerModel.DefaultWalletCurrencyCode,
            RoundStatus.Closed,
            Guid.NewGuid(),
            playerModel.DefaultWalletId,
            DateTime.Now.AddMinutes(-1),
            DateTime.Now,
            DateTime.Now,
            playerModel.BrandId,
            DateTime.Now.AddMinutes(-1),
            DateTime.Now,
            20m,
            20m,
            45m,
            null,
            20m,
            -25m,
            null,
            ClientRoundStatus.Win,
            null,
            null,
            null,
            null);

        var sut = new HabaneroRoundDetailGetService(fakeLogger, fakeService, fakeRoundRepository);

        var query = new HabaneroRoundDetailQuery
        {
            RoundId = roundModel.RoundId,
            Language = "en-US"
        };

        _ = fakeRoundRepository.GetAsync(query.RoundId).Returns(Task.FromResult(roundModel));
        _ = fakeService.GetGameProviderAsync(roundModel.GameProviderId).Returns(ValueTask.FromResult(gameProviderModel));
        _ = fakeService.GetGameAsync(roundModel.GameId).Returns(ValueTask.FromResult(gameModel));
        _ = fakeService.GetPlayerAsync(roundModel.PlayerCode).Returns(ValueTask.FromResult(playerModel));
        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakeService
            .GetGameProviderLanguageCodeAsync(gameProviderModel.GameProviderId, query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        var expected = new RoundDetailResult
        {
            GameProviderName = gameProviderModel.Name,
            GameName = gameModel.Name,
            ExternalRoundId = roundModel.ExternalRoundId,
            BetAmount = roundModel.BetAmount,
            Turnover = roundModel.Turnover,
            Winloss = roundModel.WinLoss,
            GameResult = roundModel.ClientStatus.ToString(),
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
        Assert.AreEqual(expected.GameProviderName, actual.GameProviderName);
        Assert.AreEqual(expected.GameName, actual.GameName);
        Assert.AreEqual(expected.ExternalRoundId, actual.ExternalRoundId);
        Assert.AreEqual(expected.BetAmount, actual.BetAmount);
        Assert.AreEqual(expected.Turnover, actual.Turnover);
        Assert.AreEqual(expected.Winloss, actual.Winloss);
        Assert.AreEqual(expected.GameResult, actual.GameResult);
        CollectionAssert.AreEqual(expected.Data, actual.Data);
    }
}