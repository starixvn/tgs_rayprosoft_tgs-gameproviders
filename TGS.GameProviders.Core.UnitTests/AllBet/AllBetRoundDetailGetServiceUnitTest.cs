using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.AllBet.ApplicationServices;
using TGS.GameProviders.Core.AllBet.Models;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.UnitTests.AllBet;

[TestFixture]
public class AllBetRoundDetailGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<AllBetRoundDetailGetService>.Instance;
        var fakeRoundService = Substitute.For<IRoundService>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameService = Substitute.For<IGameService>();
        var fakePlayerService = Substitute.For<IPlayerService>();

        var playerModel = new PlayerInfo(
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
            Guid.Parse("579d6da3-b0ed-4e3f-bd4a-434a98efbd68"),
            GameProviderStatus.Active,
            null,
            true,
            true,
            true,
            10,
            5,
            13,
            false,
            "AB",
            "AllBet",
            null);

        var gameModel = new GameInfo(
            1300002,
            GameStatus.Active,
            "101",
            null,
            null,
            null,
            gameProviderModel.GameProviderId,
            GameType.LiveDealer,
            GamePlatformType.Desktop,
            false,
            true,
            false,
            true,
            null,
            "Normal_Baccarat",
            "Normal Baccarat",
            null,
            "AB");

        var roundModel = new RoundInfo(
            Guid.Parse("5566e31a-80ec-46eb-522d-08d9c07bf52d"),
            "1639679431950180036",
            gameProviderModel.GameProviderId,
            playerModel.PlayerCode,
            gameModel.GameId,
            playerModel.DefaultWalletCurrencyCode,
            (Tgs.GameProviders.QueryServer.Client.RoundStatus)RoundStatus.Closed,
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
            (Tgs.GameProviders.QueryServer.Client.RoundClientStatus)ClientRoundStatus.Win,
            null,
            null,
            null,
            JsonSerializer.Serialize(new AllBetGameHistory(101, "{408,211,-1},{302,313,-1}", "")));

        var query = new AllBetRoundDetailQuery
        {
            RoundId = roundModel.RoundId,
            Language = "en-US"
        };

        var expected = new RoundDetailResult
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataJson
                {
                    Value = JsonSerializer.Serialize(new Dictionary<string, string>
                    {
                        ["Player"] = "♦8;♥J",
                        ["Banker"] = "♣2;♣K"
                    })
                }
            }
        };

        _ = fakeRoundService.GetAsync(query.RoundId).Returns(ValueTask.FromResult(roundModel));
        _ = fakeGameProviderRepository.GetAsync(roundModel.GameProviderId).Returns(ValueTask.FromResult(gameProviderModel));
        _ = fakeGameService.GetCacheAsync(roundModel.GameId).Returns(ValueTask.FromResult(gameModel));
        _ = fakePlayerService.GetAsync(roundModel.PlayerCode).Returns(Task.FromResult(playerModel));

        var sut = new AllBetRoundDetailGetService(
            fakeLogger,
            fakeRoundService,
            fakeGameProviderRepository,
            fakeGameService);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        CollectionAssert.AreEqual(expected.Data, actual.Data);
    }
}
