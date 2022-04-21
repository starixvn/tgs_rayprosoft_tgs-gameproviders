using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Avia;
using Tgs.GameProviders.Core.Avia.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration.Avia;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.Avia
{
    [TestFixture]
    public class AviaPlayerBalanceGetServiceUnitTest
    {
        [Test]
        public async Task Normal_Process()
        {
            var faceLogger = NullLogger<AviaPlayerBalanceGetService>.Instance;
            var fakeAviaApiClient = Substitute.For<IAviaApiClient>();
            var fakeAviaService = Substitute.For<IAviaService>();
            var fakePlayerService = Substitute.For<IPlayerService>();
            var fakeAviaGamePlayerService = Substitute.For<IAviaGamePlayerService>();

            var fakeService = new AviaPlayerBalanceGetService(faceLogger, fakeAviaApiClient, fakeAviaService, fakePlayerService, fakeAviaGamePlayerService);

            var query = new AviaPlayerBalanceQuery()
            {
                PlayerCode = "4ucmqplxb029"
            };

            var playerInfo = new PlayerInfo(
                Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
                "TGS",
                Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
                "TGS",
                Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                "GPI",
                "4ucmqplxb029",
                "4ucmqplxb029",
                Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
                "CNY",
                PlayerStatus.Enabled);

            var brandMerchant = new AviaBrandMerchant()
            {
                BrandCode = DefConst.BrandCodeDefault,
                ApiUrl = new("https://api.aviaapi.vip"),
                Authorization = "81e2d1a24c684a76a710a70f4900a353",
                PlayerPassword = "TgsP@ssw0rd"
            };

            var gameProviderCurrencyCode = "CNY";
            decimal? balance = 100;
            _ = fakeAviaService.GetBrandMerchantAsync(playerInfo.BrandCode).Returns(ValueTask.FromResult(brandMerchant));
            _ = fakePlayerService.GetCacheAsync(query.PlayerCode).Returns(Task.FromResult(playerInfo));
            _ = fakeAviaService.GetGameProviderCurrencyCodeAsync(playerInfo.DefaultWalletCurrencyCode).Returns(ValueTask.FromResult(gameProviderCurrencyCode));
            _ = fakeAviaApiClient.BalanceAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Authorization,
                query.PlayerCode).Returns(ValueTask.FromResult<(bool Success, decimal?)>((true, balance)));

            var actual = await fakeService.GetAsync(query).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(balance, actual);
        }
    }
}
