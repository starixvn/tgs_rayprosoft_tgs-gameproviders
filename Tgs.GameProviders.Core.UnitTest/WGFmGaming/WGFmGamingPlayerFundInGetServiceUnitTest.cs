using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.WGFmGaming;

[TestFixture]
public class WGFmGamingPlayerFundInGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceWGFmGamingClient = Substitute.For<IGameInterfaceWGFmGamingClient>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var query = new WGFmGamingPlayerFundInQuery
        {
            BrandCode = "BrandCode",
            CurrencyCode = "CNY",
            PlayerCode = "test",
            OrderId = Guid.NewGuid().ToString("N"),
            Amount = 1
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
            .FundInAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.PartnerCode),
                Arg.Is(configuration.PartnerKey),
                Arg.Is(configuration.IV),
                Arg.Is(query.PlayerCode),
                Arg.Is(query.OrderId),
                Arg.Is(query.Amount));

        var sut = new WGFmGamingPlayerFundInGetService(
            fakeGameInterfaceWGFmGamingClient,
            fakeSettingRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(query.OrderId, actual.OrderId);
    }
}
