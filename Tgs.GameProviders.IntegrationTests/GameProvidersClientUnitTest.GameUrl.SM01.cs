using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core.SM01.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests;

public partial class GameProvidersClientUnitTest
{
    [Fact]
    public async Task Game_Url_By_SM01()
    {
        var playerCode = "test";

        var result = new SM01PlatformLoginInfo(
            ApiUrl: "https://cn.ucfclive.com",
            MerchantId: "SQjCxm2cX5bahBta",
            CustomerName: "4ucmqplxb029",
            Timestamp: 1670390195123,
            Sign: "46C9EA4F03900B74379A15A56ED14229");

        var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IGetService<SM01PlatformLoginQuery, SM01PlatformLoginInfo>>();

				var query = new SM01PlatformLoginQuery
				{
					PlayerCode = playerCode
				};

				_ = fakeService
					.GetAsync(
						Arg.Is(query),
						Arg.Any<CancellationToken>())
					.Returns(result);

				_ = services.AddTransient(_ => fakeService);
			}));

        var httpClient = web.CreateClient();

        var sut = new GameProvidersClient(httpClient);

        var actual = await sut.GamePlatformLoginBySM01Async(playerCode: playerCode);

        Assert.NotNull(actual);
        Assert.Equal(result.MerchantId, actual.MerchantId);
        Assert.Equal(result.CustomerName, actual.CustomerName);
        Assert.Equal(result.Timestamp, actual.Timestamp);
        Assert.Equal(result.Sign, actual.Sign);
    }
}
