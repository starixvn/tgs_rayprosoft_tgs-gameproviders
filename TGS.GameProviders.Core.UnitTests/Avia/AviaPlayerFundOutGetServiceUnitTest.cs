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
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.UnitTests.Avia
{
    [TestFixture]
    public class AviaPlayerFundOutGetServiceUnitTest
    {
        [Test]
        public async Task Normal_Process()
        {
            var faceLogger = NullLogger<AviaPlayerFundOutGetService>.Instance;
            var fakeAviaApiClient = Substitute.For<IAviaApiClient>();
            var fakeAviaService = Substitute.For<IAviaService>();

            var fakeService = new AviaPlayerFundOutGetService(faceLogger, fakeAviaApiClient, fakeAviaService);

            var query = new AviaPlayerFundOutQuery()
            {
                BrandCode = "GPI",
                PlayerCode = "4ucmqplxb029",
                CurrencyCode = "CNY",
                OrderId = Guid.NewGuid().ToString("N"),
                Amount = 1.00m
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

            var success = true;
            var orderId = $"{ DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)):yyyyMMddHHmmssfff}{query.PlayerCode}";
            var gameProviderCurrencyCode = "CNY";

            _ = fakeAviaService.GetSettingConfigurationAsync().Returns(Task.FromResult(configuration));
            _ = fakeAviaService.GetGameProviderCurrencyCodeAsync(query.CurrencyCode).Returns(ValueTask.FromResult(gameProviderCurrencyCode));
            _ = fakeAviaApiClient.TransferOutAsync(
                configuration.ApiUrl,
                configuration.Authorization,
                query.PlayerCode,
                query.CurrencyCode,
                query.Amount).Returns(ValueTask.FromResult<(bool Success, string orderId)>((success, orderId)));

            var actual = await fakeService.GetAsync(query).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(orderId, actual.OrderId);
        }
    }
}
