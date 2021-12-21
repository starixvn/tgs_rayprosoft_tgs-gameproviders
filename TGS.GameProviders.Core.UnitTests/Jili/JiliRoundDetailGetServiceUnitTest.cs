using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.Jili;
using TGS.GameProviders.Core.Jili.ApplicationServices;
using TGS.GameProviders.Core.Jili.Models;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Core.Repositories.Models.Player;
using TGS.GameProviders.Core.Repositories.Models.Rounds;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.RoundDetail;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.Jili;

namespace TGS.GameProviders.Core.UnitTests.Jili;

[TestFixture]
public class JiliRoundDetailGetServiceUnitTest
{
    [Test]
    public async ValueTask Normal_Process()
    {
        var fakeLogger = NullLogger<JiliRoundDetailGetService>.Instance;
        var fakeRoundRepository = Substitute.For<IRoundRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeService = Substitute.For<IJiliService>();
        var fakeMerchantMapper = Substitute.For<IJiliMerchantMapper>();
        var fakeApiClient = Substitute.For<IJiliApiClient>();

        var historyUrl = "http://www.google.com";
        var gameProviderLanguageCode = "en-US";

        var merchant = new JiliMerchant
        {
            BrandCode = "Default",
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
            Guid.Parse("b957e28c-0e94-40af-9d37-ab48cdeb5d29"),
            GameProviderStatus.Active,
            null,
            true,
            true,
            true,
            10,
            5,
            22,
            false,
            "JILI",
            "JILI",
            null);

        var gameModel = new GameModel(
            2200001,
            GameStatus.Active,
            "10",
            null,
            null,
            null,
            gameProviderModel.GameProviderId,
            GameType.Slot,
            GamePlatformType.Desktop,
            false,
            true,
            false,
            true,
            null,
            "FaFaFa",
            "FaFaFa",
            null,
            "JILI");

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
            null);

        var query = new JiliRoundDetailQuery
        {
            RoundId = roundModel.RoundId,
            Language = "en-US"
        };

        _ = fakeRoundRepository.GetAsync(query.RoundId).Returns(Task.FromResult(roundModel));
        _ = fakeGameProviderRepository.GetAsync(roundModel.GameProviderId).Returns(ValueTask.FromResult(gameProviderModel));
        _ = fakeGameRepository.GetAsync(roundModel.GameId).Returns(Task.FromResult(gameModel));
        _ = fakePlayerRepository.GetAsync(roundModel.PlayerCode).Returns(Task.FromResult(playerModel));
        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakeMerchantMapper.GetMerchant(configuration.Merchants, playerModel.BrandCode).Returns(merchant);
        _ = fakeService
            .GetGameProviderLanguageCodeAsync(gameProviderModel.GameProviderId, query.Language)
            .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        _ = fakeApiClient
            .GetGameDetailUrlAsync(
                configuration.ApiUrl,
                merchant.AgentId,
                merchant.AgentKey,
                gameProviderLanguageCode,
                roundModel.ExternalRoundId)
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

        var sut = new JiliRoundDetailGetService(
            fakeLogger,
            fakeRoundRepository,
            fakeGameProviderRepository,
            fakeGameRepository,
            fakePlayerRepository,
            fakeService,
            fakeMerchantMapper,
            fakeApiClient);

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