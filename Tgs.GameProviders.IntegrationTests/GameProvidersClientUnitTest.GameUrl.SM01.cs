using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core.SM01.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests;

public partial class GameProvidersClientUnitTest
{
     [Test]
    public async Task Test_GameProvidersClient_GamePlatformLoginBySM01Async()
    {
        var fakeService = Substitute.For<IGetService<SM01PlatformLoginQuery, SM01PlatformLoginInfo>>();

        var web = new TestGameProvidersApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                _ = services.AddTransient(_ => fakeService);
            });
        });

        var playerCode = "test";

        var query = new SM01PlatformLoginQuery
        {
            PlayerCode = playerCode
        };
        var result = new SM01PlatformLoginInfo
        (
            MerchantId: "SQjCxm2cX5bahBta",
            CustomerName: "4ucmqplxb029",
            Timestamp: 1670390195123,
            Sign: "46C9EA4F03900B74379A15A56ED14229"
        );
        _ = fakeService
            .GetAsync(
                Arg.Is(query))
            .Returns(result);

        var httpClient = web.CreateClient();

        var sut = new GameProvidersClient(httpClient);

        var actual = await sut
            .GamePlatformLoginBySM01Async(
                playerCode: playerCode)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(result.MerchantId, actual.MerchantId);
        Assert.AreEqual(result.CustomerName, actual.CustomerName);
        Assert.AreEqual(result.Timestamp, actual.Timestamp);
        Assert.AreEqual(result.Sign, actual.Sign);
    }
}
