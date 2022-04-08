using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.DreamGame;
using Tgs.GameProviders.Core.DreamGame.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.DreamGame;

[TestFixture]
public class DreamGameMarkGameCommandServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeService = Substitute.For<IDreamGameService>();
        var fakeClient = Substitute.For<IDreamGameApiClient>();

        var configuration = new DreamGameConfiguration
        {
            ServiceEndpointUrl = new("http://game-dg"),
            ApiUrl = new("http://api.dg99web.com"),
            ApiKey = "974e8e9a072e4ad59a9e3f43a06ff9ae",
            AgentName = "DGTE0105CO",
            PlayerPassword = "TgsP@ssw0rd"
        };
        var externalRoundIds = new string[] { "123456" };

        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        _ = fakeClient.MarkReportAsync(configuration.ApiUrl, configuration.ApiKey, configuration.AgentName, externalRoundIds).Returns(true);
        var sut = new DreamGameMarkGameCommandService(
            fakeClient,
            fakeService);

        await sut.ExecuteAsync(new(externalRoundIds)).ConfigureAwait(false);
    }
}
