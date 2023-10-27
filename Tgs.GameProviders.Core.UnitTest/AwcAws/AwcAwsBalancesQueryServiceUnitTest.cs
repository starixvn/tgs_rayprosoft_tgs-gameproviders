using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Core.AwcAws.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.AwcAws;

public class AwcAwsBalancesQueryServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeBrand = Substitute.For<IBrandRepository>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakeClient = Substitute.For<IGameInterfaceAwcServiceClient>();

        var query = new AwcAwsBalancesQuery(
             BrandCode: "GPI");

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(GameProviderType.AWC_AWS),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var brandId = Guid.NewGuid();
        _ = fakeBrand.GetBrandIdAsync(
            brandCode: Arg.Is(query.BrandCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(brandId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrand.GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: Arg.Is(gameProviderId),
            brandId: Arg.Is(brandId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var configuration = new AwcAwsConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            ApiRecordUrl = new("https://tttint.onlinegames22.com/"),
            AgentId = "testAgentId",
            Cert = "testCert",
        };
        _ = fakeGameProvider.GetAwcAwsConfigurationAsync(
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(configuration);

        var sut = new AwcAwsBalancesQueryService(
            fakeBrand,
            fakeClient,
            fakeGameProvider);

        _ = await sut.QueryAsync(
            query)
            .ToArrayAsync();

        _ = fakeClient
            .Received(1)
            .AllBalancesAsync(
            Arg.Is(configuration.ApiUrl),
            Arg.Is(configuration.AgentId),
            Arg.Is(configuration.Cert));
    }
}
