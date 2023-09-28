using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using ExpectedObjects;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core.AllBet.ApplicationServices;
using Tgs.GameProviders.Core.AllBet.Models;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.UnitTest.AllBet;

public class AllBetRoundDetailGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeRoundService = Substitute.For<IRoundRepository>();

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

        var roundInfo = new RoundInfo(
            Guid.Parse("5566e31a-80ec-46eb-522d-08d9c07bf52d"),
            "1639679431950180036",
            Guid.Parse("579d6da3-b0ed-4e3f-bd4a-434a98efbd68"),
            player.PlayerCode,
            1300002,
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
            JsonSerializer.Serialize(new AllBetGameHistory(101, "{408,211,-1},{302,313,-1}", "")));

        var query = new AllBetRoundDetailQuery
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
                    Value = JsonSerializer.Serialize(new Dictionary<string, string>
                    {
                        ["Player"] = "♦8;♥J",
                        ["Banker"] = "♣2;♣K"
                    })
                }
            }
        };

        _ = fakeRoundService.GetAsync(query.RoundId).Returns(ValueTask.FromResult<RoundInfo?>(roundInfo));

        var sut = new AllBetRoundDetailGetService(
            fakeRoundService);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        expected.Data.ToExpectedObject().ShouldEqual(actual.Data);
    }
}
