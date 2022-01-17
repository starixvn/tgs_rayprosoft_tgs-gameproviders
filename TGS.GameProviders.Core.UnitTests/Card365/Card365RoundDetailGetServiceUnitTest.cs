using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.Card365.ApplicationServices;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Core.Repositories.Models.Player;
using TGS.GameProviders.Core.Repositories.Models.Rounds;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Core.UnitTests.Card365;

[TestFixture]
internal class Card365RoundDetailGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<Card365RoundDetailGetService>.Instance;
        var fakeRoundRepository = Substitute.For<IRoundRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameRepository = Substitute.For<IGameRepository>();

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
            Guid.Parse("630e822b-2635-4314-a7e4-5c7057c1a920"),
            GameProviderStatus.Active,
            null,
            true,
            true,
            true,
            10,
            5,
            19,
            false,
            "365",
            "365",
            null);

        var gameModel = new GameModel(
            1900008,
            GameStatus.Active,
            "563",
            null,
            null,
            null,
            gameProviderModel.GameProviderId,
            GameType.CardGame,
            GamePlatformType.DesktopAndMobile,
            false,
            true,
            false,
            true,
            null,
            "Three_Pictures",
            "Three Pictures",
            null,
            "365");

        var roundModel = new RoundModel(
            Guid.Parse("5566e31a-80ec-46eb-522d-08d9c07bf52d"),
            "1639679431950180036",
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
            @"{""OutMoney"":0.0,""Chair"":1,""Banker"":0,""PlayId"":1,""Tiles"":""[b10, b4, c1]"",""BetAmount"":7004.17,""RealBetMoney"":20.0,""PayAmount"":7023.57,""RealPayMoney"":20.0}");

        var query = new Card365RoundDetailQuery
        {
            RoundId = roundModel.RoundId,
            Language = "en-US"
        };

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
                new RoundDetailDataJson
                {
                    Value = JsonSerializer.Serialize(new
                    {
                        IsBanker = true,
                        Tiles = "[b10, b4, c1]"
                    })
                }
            }
        };

        _ = fakeRoundRepository.GetAsync(query.RoundId).Returns(Task.FromResult(roundModel));
        _ = fakeGameProviderRepository.GetAsync(roundModel.GameProviderId).Returns(ValueTask.FromResult(gameProviderModel));
        _ = fakeGameRepository.GetAsync(roundModel.GameId).Returns(Task.FromResult(gameModel));

        var sut = new Card365RoundDetailGetService(
            fakeLogger,
            fakeRoundRepository,
            fakeGameProviderRepository,
            fakeGameRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(expected.GameProviderName, actual.GameProviderName);
        Assert.AreEqual(expected.GameName, actual.GameName);
        Assert.AreEqual(expected.ExternalRoundId, actual.ExternalRoundId);
        Assert.AreEqual(expected.BetAmount, actual.BetAmount);
        Assert.AreEqual(expected.Turnover, actual.Turnover);
        Assert.AreEqual(expected.Winloss, actual.Winloss);
        Assert.AreEqual(expected.GameResult, actual.GameResult);
    }
}