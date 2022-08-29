using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Joker;
using Tgs.GameProviders.Core.Joker.ApplicationServices;
using Tgs.GameProviders.Core.Joker.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.Joker;

namespace Tgs.GameProviders.Core.UnitTest.Joker;

[TestFixture]
public class JokerGameRecordListGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
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

        var success = true;
        var response = new JokerGameRecordInfoResponse
        {
            NextId = "eyIyMDIyMDQyMDA5NTgiOnsiQ3JlYXRlZFRpbWUiOiI1NTdmOGNjMC1jMDRkLTExZWMtYmVlYy1kZjk4OWNhYjE5MGMifX0=",
            Data = new JokerGameRecordInfoData
            {
                Game = new JokerGameRecordData[]
                {
                    new()
                    {
                        TransactionId = "xmkeajuna89ne",
                        Username = "4UF6K014JP5X",
                        GameExternalId= "7rw3tfwz11kaw",
                        Description= "Main Spin",
                        BetAmount = 1.60m,
                        SettleAmount= 3.20m,
                        Time= DateTimeOffset.Parse("2022-04-20T09:58:08.9660000+08:00"),
                        Type = "Game"
                    }
                }
            }
        };

        var gameProviderCode = GameProviderType.Joker;
        var gameProviderId = Guid.Parse("2ebfe118-b36b-40fa-85b5-78363470addc");

        var query = new JokerGameRecordListQuery(
            "Default",
            "TGVY",
            DateTimeOffset.Parse("2022-04-20T00:00:00+00:00"),
            DateTimeOffset.Parse("2022-04-21T00:00:00+00:00"),
            null);

        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);

        _ = fakeAgentMapper
            .GetAgentById(
                Arg.Is(configuration.BrandMerchants),
                Arg.Is(query.BrandCode),
                Arg.Is(query.AgentId))
            .Returns(agent);

        _ = fakeService.GetGameProviderIdAsync().Returns(gameProviderId);

        _ = fakeApiClient
            .GetGameRecordAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(query.StartTime),
                Arg.Is(query.EndTime),
                Arg.Is(query.NextId),
                Arg.Is(agent.AgentId),
                Arg.Is(agent.SecretKey))
            .Returns((success, response));

        var sut = new JokerGameRecordListGetService(fakeApiClient, fakeService, fakeAgentMapper);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(success, actual.Success);
        Assert.AreEqual(gameProviderCode, actual.Datas[0].GameProviderCode);
        Assert.AreEqual(gameProviderId, actual.Datas[0].GameProviderId);
        Assert.AreEqual(response.Data.Game.ElementAt(0).TransactionId, actual.Datas[0].ExternalRoundId);
    }
}
