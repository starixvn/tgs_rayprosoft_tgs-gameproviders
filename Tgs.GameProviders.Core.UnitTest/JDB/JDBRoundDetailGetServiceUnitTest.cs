using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core.JDB;
using Tgs.GameProviders.Core.JDB.ApplicationServices;
using Tgs.GameProviders.Core.JDB.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.JDB;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.UnitTest.JDB;

[TestFixture]
public class JDBRoundDetailGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeRoundService = Substitute.For<IRoundRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeService = Substitute.For<IJDBService>();
        var fakeAgentMapper = Substitute.For<IJDBAgentMapper>();
        var fakeApiClient = Substitute.For<IJDBApiClient>();

        var gameProviderLanguageCode = "en";
        var historyUrl = "https://www.google.com";

        var agent = new JDBAgent
        {
            Id = "wgvb3rmb",
            CurrencyCode = "CNY"
        };

        var configuration = new JDBConfiguration
        {
            ServiceEndpointUrl = new("http://game-jdb"),
            ApiUrl = new("http://api.jdb711.com"),
            AESIv = "756edb2fc85fe908",
            AESKey = "4f09f467e6a8fb9f",
            ClientDomain = "VB3",
            TimezoneHour = 8,
            MoreGame = false,
            Agents = new[] { agent }
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
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.JDB,
            UniqueId: 1,
            Name: GameProviderType.JDB,
            TransferWallet: false,
            AutoCloseRound: true,
            SupportTestPlayer: false,
            AllowKickPlayer: true,
            PlaceBetTimeout: 0,
            SettleBetTimeout: 0,
             Description: GameProviderType.JDB,
            Localization: null);

        var gameInfo = new GameInfo(
            GameId: 1,
            GameCode: "test",
            GameProviderId: gameProviderInfo.GameProviderId,
            GameProviderCode: GameProviderType.JDB,
            GameExternalId: "0_14034",
            Type: GameType.LiveDealerLobby,
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
            SubGameProviderCode: GameProviderType.JDB);

        var roundInfo = new RoundInfo(
            Guid.Parse("015dc0ab-eecc-49da-299e-08d9d18bf7ef"),
            "5250172280339",
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

        var query = new JDBRoundDetailQuery
        {
            RoundId = roundInfo.RoundId,
            Language = "en-US"
        };

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

        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakeService.GetGameProviderLanguageAsync(roundInfo.GameProviderId, query.Language)
             .Returns(ValueTask.FromResult(gameProviderLanguageCode));

        _ = fakeAgentMapper.GetAgent(configuration.Agents, player.DefaultWalletCurrencyCode)
            .Returns(agent);

        _ = fakeRoundService.GetAsync(query.RoundId).Returns(ValueTask.FromResult<RoundInfo?>(roundInfo));
        _ = fakePlayerRepository.GetAsync(roundInfo.PlayerCode).Returns(ValueTask.FromResult(player));
        _ = fakeGameProviderRepository.GetAsync(roundInfo.GameProviderId).Returns(ValueTask.FromResult(gameProviderInfo));
        _ = fakeGameRepository.GetAsync(roundInfo.GameId).Returns(ValueTask.FromResult(gameInfo));

        var items = gameInfo.GameExternalId.Split('_');

        fakeApiClient
            .GetBetHistoryUrlAsync(
                configuration.ApiUrl,
                configuration.ClientDomain,
                configuration.AESIv,
                configuration.AESKey,
                Arg.Is<JDBBetHistoryRequest>(x
                    => x.Parent == agent.Id &&
                       x.UserId == player.PlayerCode &&
                       x.Language == gameProviderLanguageCode &&
                       x.GameType == int.Parse(items[0]) &&
                       x.SequenceNo == long.Parse(roundInfo.ExternalRoundId)))
            .Returns(ValueTask.FromResult(
                new JDBBetHistoryResponse
                {
                    Status = "0000",
                    Data = new JDBBetHistoryPageUrl[]
                    {
                        new()
                        {
                            Path = historyUrl
                        }
                    }
                }));

        var sut = new JDBRoundDetailGetService(
            fakeRoundService,
            fakePlayerRepository,
            fakeGameProviderRepository,
            fakeGameRepository,
            fakeService,
            fakeAgentMapper,
            fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        CollectionAssert.AreEqual(expected.Data, actual.Data);
    }
}
