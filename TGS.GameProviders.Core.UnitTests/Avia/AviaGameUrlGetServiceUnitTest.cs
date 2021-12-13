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
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.UnitTests.Avia
{
    [TestFixture]
    public class AviaGameUrlGetServiceUnitTest
    {
        [Test]
        public async Task Normal_Process()
        {
            var faceLogger = NullLogger<AviaGameUrlGetService>.Instance;
            var fakeAviaApiClient = Substitute.For<IAviaApiClient>();
            var fakeAviaService = Substitute.For<IAviaService>();
            var fakeGamePlayerService = Substitute.For<IAviaGamePlayerService>();

            var fakeService = new AviaGameUrlGetService(fakeAviaApiClient, faceLogger, fakeAviaService, fakeGamePlayerService);

            var query = new AviaGameUrlQuery()
            {
                GameType = GameType.ESports,
                GameId = 2300001,
                BrandCode = "GPI",
                PlayerCode = "4ucmqplxb029",
                CurrencyCode = "CNY",
                LoginRecordId = Guid.NewGuid(),
                PlatformType = PlatformType.Desktop,
                Language = "zh-CN",
                IPAddress = "127.0.0.1",
                BackUrl = new("https://dev-t1.rpgp.cc/")
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
            var gameProviderCurrencyCode = "CNY";
            var gameProviderCultureCode = "CHN";
            Uri? gameUrl = new("https://www.google.com/");
            _ = fakeAviaService.GetSettingConfigurationAsync().Returns(Task.FromResult(configuration));
            _ = fakeAviaService.GetGameProviderCurrencyCodeAsync(query.CurrencyCode).Returns(ValueTask.FromResult(gameProviderCurrencyCode));
            _ = fakeAviaService.GetGameProviderCultureCodeAsync(query.Language).Returns(ValueTask.FromResult(gameProviderCultureCode));
            _ = fakeAviaApiClient.GetGameUrlAsync(
                configuration.ApiUrl,
                configuration.Authorization,
                query.PlayerCode,
                gameProviderCultureCode,
                null,
                null).Returns(ValueTask.FromResult<(bool Success, Uri?)>((true, gameUrl)));

            var actual = await fakeService.GetAsync(query).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(gameUrl, actual.Url);
        }
    }
}
