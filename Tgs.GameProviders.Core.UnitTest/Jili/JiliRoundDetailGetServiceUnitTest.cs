using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Jili;
using Tgs.GameProviders.Core.Jili.ApplicationServices;
using Tgs.GameProviders.Core.Jili.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.Jili;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.UnitTest.Jili;

[TestFixture]
public class JiliRoundDetailGetServiceUnitTest
{
    [Test]
    public async ValueTask Normal_Process()
    {
        var fakeLogger = NullLogger<JiliRoundDetailGetService>.Instance;
        var fakeRoundService = Substitute.For<IRoundService>();
        var fakePlayerService = Substitute.For<IPlayerService>();
        var fakeService = Substitute.For<IJiliService>();
        var fakeMerchantMapper = Substitute.For<IJiliMerchantMapper>();
        var fakeApiClient = Substitute.For<IJiliApiClient>();

        var historyUrl = "http://www.google.com";
        var gameProviderLanguageCode = "en-US";

        var merchant = new JiliMerchant
        {
            BrandCode = DefConst.BrandCodeDefault,
            AgentId = "PG_Seamless",
            AgentKey = "0d90dda822b6df548460da8f551c1a76c01b6a9e"
        };

        var configuration = new JiliConfiguration
        {
            ApiUrl = new Uri("https://uat-wb-api.jlfafafa2.com"),
            Merchants = new[]
            {
                merchant
            },
            ClientId = "ClientId",
            SecretKey = "SecretKey"
        };

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
            Guid.Parse("b957e28c-0e94-40af-9d37-ab48cdeb5d29"),
            playerInfo.PlayerCode,
            2200001,
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
            null);

        var query = new JiliRoundDetailQuery
        {
            RoundId = roundInfo.RoundId,
            Language = "en-US"
        };

        _ = fakeRoundService.GetAsync(query.RoundId).Returns(ValueTask.FromResult<RoundInfo?>(roundInfo));
        _ = fakePlayerService.GetAsync(roundInfo.PlayerCode).Returns(Task.FromResult(playerInfo));
        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakeMerchantMapper.GetMerchant(configuration.Merchants, playerInfo.BrandCode).Returns(merchant);
        _ = fakeService
            .GetGameProviderLanguageCodeAsync(Guid.Parse("b957e28c-0e94-40af-9d37-ab48cdeb5d29"), query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        _ = fakeApiClient
            .GetGameDetailUrlAsync(
                configuration.ApiUrl,
                merchant.AgentId,
                merchant.AgentKey,
                gameProviderLanguageCode,
                roundInfo.ExternalRoundId)
            .Returns(ValueTask.FromResult(
                new JiliGetGameDetailUrlResponse
                {
                    ErrorCode = 0,
                    Data = new JiliGetGameDetailUrlData
                    {
                        Url = historyUrl
                    }
                }));

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

        var sut = new JiliRoundDetailGetService(
            fakeLogger,
            fakeRoundService,
            fakePlayerService,
            fakeService,
            fakeMerchantMapper,
            fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        CollectionAssert.AreEqual(expected.Data, actual.Data);
    }
}
