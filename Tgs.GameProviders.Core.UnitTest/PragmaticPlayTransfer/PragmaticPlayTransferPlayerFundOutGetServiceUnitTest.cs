using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.PragmaticPlayTransfer;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.PragmaticPlayTransfer;

internal class PragmaticPlayTransferPlayerFundOutGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeService = Substitute.For<IPragmaticPlayTransferService>();
        var fakeApiClient = Substitute.For<IPragmaticPlayTransferApiClient>();

        var configuration = new PragmaticPlayTransferConfiguration
        {
            ServiceEndpointUrl = new("http://game-pp"),
            ApiUrl = new("https://api.prerelease-env.biz"),
            SecureLogin = "tgs_tgstfw",
            SecretKey = "testKey",
            ProviderId = "PragmaticPlay"
        };

        var success = true;
        var playerCode = "4ucmqplxb029";
        var orderId = Guid.NewGuid().ToString("N");
        var fundOutAmount = 10m;

        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);

        _ = fakeApiClient
            .FundOutAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.SecureLogin),
                Arg.Is(configuration.SecretKey),
                Arg.Is(playerCode),
                Arg.Is(orderId),
                Arg.Is(fundOutAmount))
            .Returns(success);

        var query = new PragmaticPlayTransferPlayerFundOutQuery
        {
            Amount = fundOutAmount,
            BrandCode = "GPI",
            CurrencyCode = "CNY",
            OrderId = orderId,
            PlayerCode = "4ucmqplxb029"
        };

        var sut = new PragmaticPlayTransferPlayerFundOutGetService(
            fakeService,
            fakeApiClient);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(orderId, actual.OrderId);
    }
}