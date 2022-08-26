using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Avia;
using Tgs.GameProviders.Core.Avia.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration.Avia;

namespace Tgs.GameProviders.Core.UnitTest.Avia;

[TestFixture]
public class AviaPlayerFundCheckingGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var faceLogger = NullLogger<AviaPlayerFundCheckingGetService>.Instance;
        var fakeAviaApiClient = Substitute.For<IAviaApiClient>();
        var fakeAviaService = Substitute.For<IAviaService>();

        var fakeService = new AviaPlayerFundCheckingGetService(faceLogger, fakeAviaApiClient, fakeAviaService);

        var query = new AviaPlayerFundCheckingQuery()
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = "202112131400370384ucmqplxb029"
        };

        var brandMerchant = new AviaBrandMerchant()
        {
            BrandCode = DefConst.BrandCodeDefault,
            ApiUrl = new("https://api.aviaapi.vip"),
            Authorization = "81e2d1a24c684a76a710a70f4900a353",
            PlayerPassword = "TgsP@ssw0rd"
        };

        var expected = true;
        _ = fakeAviaService.GetBrandMerchantAsync(query.BrandCode).Returns(ValueTask.FromResult(brandMerchant));
        _ = fakeAviaApiClient.TranslateinfoAsync(
            brandMerchant.ApiUrl,
            brandMerchant.Authorization,
            query.OrderId).Returns(ValueTask.FromResult<bool>((expected)));

        var actual = await fakeService.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(expected, actual.OrderStatus);
    }
}
