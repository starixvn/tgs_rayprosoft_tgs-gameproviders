using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.WGFmGaming;

[TestFixture]
public class WGFmGamingPlayerFundCheckingGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceWGFmGamingClient = Substitute.For<IGameInterfaceWGFmGamingClient>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var query = new WGFmGamingPlayerFundCheckingQuery
        {
            BrandCode = "BrandCode",
            CurrencyCode = "CNY",
            PlayerCode = "test",
            OrderId = Guid.NewGuid().ToString("N")
        };

        var configuration = new WGSportsConfiguration
        {
            ApiUrl = new("http://localhost/"),
            PartnerCode = "PartnerCode",
            PartnerKey = "PartnerKey",
            IV = "IV"
        };

        _ = fakeSettingRepository.GetWGSportsConfigurationAsync().Returns(ValueTask.FromResult(configuration));

        _ = fakeGameInterfaceWGFmGamingClient
            .FundStatusAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.PartnerCode),
                Arg.Is(configuration.PartnerKey),
                Arg.Is(configuration.IV),
                Arg.Is(query.OrderId))
            .Returns(true);

        var sut = new WGFmGamingPlayerFundCheckingGetService(
            fakeGameInterfaceWGFmGamingClient,
            fakeSettingRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.IsTrue(actual.OrderStatus);
    }
}
