using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Avia;
using Tgs.GameProviders.Core.Avia.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration.Avia;

namespace Tgs.GameProviders.Core.UnitTest.Avia
{
    [TestFixture]
    public class AviaPlayerFundInGetServiceUnitTest
    {
        [Test]
        public async Task Normal_Process()
        {
            var faceLogger = NullLogger<AviaPlayerFundInGetService>.Instance;
            var fakeAviaApiClient = Substitute.For<IAviaApiClient>();
            var fakeAviaService = Substitute.For<IAviaService>();

            var fakeService = new AviaPlayerFundInGetService(faceLogger, fakeAviaApiClient, fakeAviaService);

            var query = new AviaPlayerFundInQuery()
            {
                BrandCode = "GPI",
                PlayerCode = "4ucmqplxb029",
                CurrencyCode = "CNY",
                OrderId = Guid.NewGuid().ToString("N"),
                Amount = 1.00m
            };

            var brandMerchant = new AviaBrandMerchant()
            {
                BrandCode = CustomConst.BrandCodeDefault,
                ApiUrl = new("https://api.aviaapi.vip"),
                Authorization = "81e2d1a24c684a76a710a70f4900a353",
                PlayerPassword = "TgsP@ssw0rd"
            };

            var success = true;
            var orderId = $"{ DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)):yyyyMMddHHmmssfff}{query.PlayerCode}";
            var gameProviderCurrencyCode = "CNY";

            _ = fakeAviaService.GetBrandMerchantAsync(query.BrandCode).Returns(ValueTask.FromResult(brandMerchant));
            _ = fakeAviaService.GetGameProviderCurrencyCodeAsync(query.CurrencyCode).Returns(ValueTask.FromResult(gameProviderCurrencyCode));
            _ = fakeAviaApiClient.TransferInAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Authorization,
                query.PlayerCode,
                query.CurrencyCode,
                query.Amount).Returns(ValueTask.FromResult<(bool Success, string orderId)>((success, orderId)));

            var actual = await fakeService.GetAsync(query).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(orderId, actual.OrderId);
        }
    }
}
