using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Card365.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.UnitTest.Card365;

[TestFixture]
internal class Card365RoundDetailGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<Card365RoundDetailGetService>.Instance;
        var fakeRoundService = Substitute.For<IRoundService>();

        var playerInfo = new PlayerInfo(
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

        var roundInfo = new RoundInfo(
            Guid.Parse("5566e31a-80ec-46eb-522d-08d9c07bf52d"),
            "1639679431950180036",
            Guid.Parse("630e822b-2635-4314-a7e4-5c7057c1a920"),
            playerInfo.PlayerCode,
            1900008,
            playerInfo.DefaultWalletCurrencyCode,
            GameRoundStatus.Closed,
            Guid.NewGuid(),
            playerInfo.DefaultWalletId,
            DateTime.Now.AddMinutes(-1),
            DateTime.Now,
            DateTime.Now,
            playerInfo.BrandId,
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
            @"{""OutMoney"":0.0,""Chair"":1,""Banker"":0,""PlayId"":1,""Tiles"":""[b10, b4, c1]"",""BetAmount"":7004.17,""RealBetMoney"":20.0,""PayAmount"":7023.57,""RealPayMoney"":20.0}");

        var query = new Card365RoundDetailQuery
        {
            RoundId = roundInfo.RoundId,
            Language = "en-US"
        };

        var expected = new RoundDetailResult
        {
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

        _ = fakeRoundService.GetAsync(query.RoundId).Returns(ValueTask.FromResult<RoundInfo?>(roundInfo));

        var sut = new Card365RoundDetailGetService(
            fakeLogger,
            fakeRoundService);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        CollectionAssert.AreEqual(expected.Data, actual.Data);
    }
}
