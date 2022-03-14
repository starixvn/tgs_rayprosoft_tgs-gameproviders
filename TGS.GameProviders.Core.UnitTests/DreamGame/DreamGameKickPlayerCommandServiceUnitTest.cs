using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.DreamGame;
using TGS.GameProviders.Core.DreamGame.ApplicationServices;
using TGS.GameProviders.Core.DreamGame.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.UnitTests.DreamGame;

[TestFixture]
public class DreamGameKickPlayerCommandServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<DreamGameKickPlayerCommandService>.Instance;
        var fakeService = Substitute.For<IDreamGameService>();
        var fakeApiClient = Substitute.For<IDreamGameApiClient>();

        var configuration = new DreamGameConfiguration
        {
            ServiceEndpointUrl = new("http://game-dg"),
            ApiUrl = new("http://api.dg99web.com"),
            ApiKey = "974e8e9a072e4ad59a9e3f43a06ff9ae",
            AgentName = "DGTE0105CO",
            PlayerPassword = "TgsP@ssw0rd"
        };

        var onlineReportResults = new OnlineReportResult[] {
                new (){ Username ="4UCMQPLXB029",
                Nickname ="29",
                CurrencyName ="CNY",
                Ip ="220.132.206.70",
                Device="1",
                Login ="2022-03-14 09:11:03",
                MemberId =72832718}
            };

        var command = new DreamGameKickPlayerCommand
        {
            PlayerCode = "4ucmqplxb029"
        };

        var sut = new DreamGameKickPlayerCommandService(
            fakeLogger,
            fakeApiClient,
            fakeService);

        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        _ = fakeApiClient.OnlineReportAsync(configuration.ApiUrl, configuration.ApiKey, configuration.AgentName).Returns(onlineReportResults);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeApiClient
            .Received(1)
            .OfflineAsync(
                configuration.ApiUrl,
                configuration.ApiKey,
                configuration.AgentName,
                onlineReportResults.First().MemberId);
    }
}
