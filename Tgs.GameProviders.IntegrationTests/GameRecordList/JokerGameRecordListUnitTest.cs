using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core.Joker;
using Tgs.GameProviders.Core.Joker.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.Joker;
using Tgs.GameProviders.ViewModels;

namespace Tgs.GameProviders.IntegrationTests.GameRecordList;

[TestFixture]
public class JokerGameRecordListUnitTest
{
     [Test]
    public async Task Test_Joker_GameRecordListAsync()
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

        var brandMerchant = new JokerBrandMerchant()
        {
            BrandCode = "Default",
            Agents = new[] { agent }
        };

        var configuration = new JokerConfiguration
        {
            ServiceEndpointUrl = new("http://game-joker"),
            ApiUrl = new("https://w.api788.net"),
            GameLaunchUrl = new Uri("http://www.gwp6868.net/"),
            BrandMerchants = new[] { brandMerchant }
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
        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                _ = services.AddSingleton(fakeApiClient);
                _ = services.AddSingleton(fakeService);
                _ = services.AddSingleton(fakeAgentMapper);
            });
        });

        var query = new JokerGameRecordListRequest
        {
            BrandCode = brandMerchant.BrandCode,
            AgentId = agent.AgentId,
            StartTime = DateTimeOffset.Parse("2021-04-20T00:00:00+00:00"),
            EndTime = DateTimeOffset.Parse("2021-04-21T00:00:00+00:00"),
            NextId = null
        };

        var gameProviderCode = GameProviderType.Joker;
        var gameProviderId = Guid.Parse("2ebfe118-b36b-40fa-85b5-78363470addc");

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

        var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

        var httpClient = web.CreateDefaultClient(new DelegatingHandler[] { fakeLoggingHandler });

        var sut = new GameRecordListClient(httpClient);

        var actual = await sut
            .JokerGameRecordListAsync(
                query.BrandCode,
                query.AgentId,
                query.StartTime,
                query.EndTime,
                query.NextId)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(success, actual!.Success);
        Assert.AreEqual(gameProviderCode, actual.Datas!.ElementAt(0).GameProviderCode);
        Assert.AreEqual(gameProviderId, actual.Datas!.ElementAt(0).GameProviderId);
        Assert.AreEqual(response.Data.Game.ElementAt(0).TransactionId, actual.Datas!.ElementAt(0).ExternalRoundId);
    }
}
