using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Joker;
using Tgs.GameProviders.Core.Joker.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.Joker;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.Joker;

[TestFixture]
public class JokerRoundDetailGetServiceUnitTest
{
    [Test]
    public async ValueTask Normal_Process()
    {
        var fakeRoundService = Substitute.For<IRoundService>();
        var fakePlayerService = Substitute.For<IPlayerService>();
        var fakeService = Substitute.For<IJokerService>();
        var fakeAgentMapper = Substitute.For<IJokerAgentMapper>();
        var fakeApiClient = Substitute.For<IJokerApiClient>();

        var agent = new JokerAgent
        {
            AgentId = "TGVY",
            CurrencyCode = "HKD",
            SecretKey = "s5ghrn77baoto",
            Status = true
        };

        var configuration = new JokerConfiguration
        {
            ServiceEndpointUrl = new("http://game-joker"),
            ApiUrl = new("https://w.api788.net"),
            GameLaunchUrl = new Uri("http://www.gwp6868.net/"),
            BrandMerchants = new JokerBrandMerchant[]
            {
                new()
                {
                    BrandCode = "Default",
                    Agents = new[] { agent }
                }
            }
        };

        var gameProviderLanguage = "en";
        var historyUrl = "//hs.gwp6868.net/History?Signature=lpjOI3lMu5kXnhT3tSF9VPD5g9Y%3d&Key=83ztt1eowy7qy&Timestamp=1650520123495&Lang=en&TransactionOCode=xj88k7byqz9ne";

        var playerInfo = new PlayerInfo(
            Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
            "TGS",
            Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
            "TGS",
            Guid.Parse("9f1ecf19-38cd-474f-b0d2-93638dc07d21"),
            "GPI_HKD",
            "4uf6k014jp5x",
            "4uf6k014jp5x",
            Guid.Parse("cf99c783-acdf-48cc-b1cc-08d99a806de9"),
            "HKD",
            PlayerStatus.Enabled);

        var additionalData = @"{""OCode"":""83ztt1eowy7qy"",""Username"":""4UF6K014JP5X"",""GameCode"":""quofrdenycyyn"",""Description"":""G2a9e5244e50b"",""Type"":""Game"",""Amount"":1.20,""Result"":2.10,""Time"":""2022-04-20T15:00:10.621+08:00""}";

        var roundInfo = new RoundInfo(
            Guid.Parse("4a3cf6e9-8001-0000-0001-000020dd98d5"),
            "83ztt1eowy7qy",
            Guid.Parse("2ebfe118-b36b-40fa-85b5-78363470addc"),
            playerInfo.PlayerCode,
            2600025,
            playerInfo.DefaultWalletCurrencyCode,
            RoundStatus.Closed,
            Guid.NewGuid(),
            playerInfo.DefaultWalletId,
            DateTime.Now,
            DateTime.Now,
            DateTime.Now,
            playerInfo.BrandId,
            DateTimeOffset.Parse("2022-04-20T15:00:10.621+08:00"),
            DateTimeOffset.Parse("2022-04-20T15:00:10.621+08:00"),
            1.2m,
            1.2m,
            2.1m,
            null,
            1.2m,
            -0.9m,
            null,
            RoundClientStatus.Win,
            null,
            null,
            null,
            additionalData);

        var sut = new JokerRoundDetailGetService(fakeApiClient, fakeService, fakeAgentMapper, fakePlayerService, fakeRoundService);

        var query = new JokerRoundDetailQuery
        {
            RoundId = roundInfo.RoundId,
            Language = "en-US"
        };

        _ = fakeRoundService.GetAsync(Arg.Is(query.RoundId)).Returns(roundInfo);
        _ = fakePlayerService.GetAsync(Arg.Is(roundInfo.PlayerCode)).Returns(playerInfo);
        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        _ = fakeService
            .GetGameProviderLanguageCodeAsync(Arg.Is(query.Language))
            .Returns(gameProviderLanguage);

        _ = fakeAgentMapper
            .GetAgent(
                Arg.Is(configuration.BrandMerchants),
                    Arg.Is(playerInfo.BrandCode),
                        Arg.Is(roundInfo.CurrencyCode))
            .Returns(agent);

        _ = fakeApiClient
            .GetGameDetailUrlAsync(
                Arg.Is(configuration.ApiUrl),
                    Arg.Is(gameProviderLanguage),
                        Arg.Is(roundInfo.ExternalRoundId),
                Arg.Any<JokerDataType>(),
                Arg.Is(agent.AgentId),
                    Arg.Is(agent.SecretKey))
            .Returns(historyUrl);

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