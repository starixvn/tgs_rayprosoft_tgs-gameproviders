using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.Avia;
using TGS.GameProviders.Core.Avia.ApplicationServices;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.UnitTests.Avia
{
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



            var configuration = new AviaConfiguration()
            {
                ServiceEndpointUrl = new("http://game-avia"),
                AmountMaximum = 10000000,
                AmountFormat = "10,2",
                ApiUrl = new("https://api.aviaapi.vip"),
                Authorization = "81e2d1a24c684a76a710a70f4900a353",
                PlayerPassword = "TgsP@ssw0rd"

            };
            var expected = true;
            _ = fakeAviaService.GetSettingConfigurationAsync().Returns(Task.FromResult(configuration));
            _ = fakeAviaApiClient.TranslateinfoAsync(
                configuration.ApiUrl,
                configuration.Authorization,
                query.OrderId).Returns(ValueTask.FromResult<bool>((expected)));

            var actual = await fakeService.GetAsync(query).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(expected, actual.OrderStatus);
        }
    }
}
