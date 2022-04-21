using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Joker;
using Tgs.GameProviders.Core.Joker.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.Joker;

namespace Tgs.GameProviders.Core.UnitTest.Joker;

[TestFixture]
public class JokerPlayerFundCheckingGetServiceUnitTest
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

        var query = new JokerPlayerFundCheckingQuery
        {
            BrandCode = "GPI_HKD",
            PlayerCode = "4uf6k014jp5x",
            CurrencyCode = "HKD",
            OrderId = Guid.NewGuid().ToString("N")
        };

        var sut = new JokerPlayerFundCheckingGetService(fakeApiClient, fakeService, fakeAgentMapper);

        var expected = true;

        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        _ = fakeAgentMapper
            .GetAgent(
                configuration.BrandMerchants,
                query.BrandCode,
                query.CurrencyCode)
            .Returns(agent);

        _ = fakeApiClient
            .FundOrderCheckingAsync(
                configuration.ApiUrl,
                query.OrderId,
                agent.AgentId,
                agent.SecretKey)
            .Returns(expected);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(expected, actual.OrderStatus);
    }
}