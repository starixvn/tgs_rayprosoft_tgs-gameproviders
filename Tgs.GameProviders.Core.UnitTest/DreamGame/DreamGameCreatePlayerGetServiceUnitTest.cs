using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.DreamGame;
using Tgs.GameProviders.Core.DreamGame.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration.DreamGame;

namespace Tgs.GameProviders.Core.UnitTest.DreamGame
{
    [TestFixture]
    public class DreamGameCreatePlayerGetServiceUnitTest
    {
        [Test]
        public async Task HKD_TableLimit_Is_G()
        {
            var fakeApiClient = Substitute.For<IDreamGameApiClient>();

            var gameUrl = new Uri("http://www.google.com");

            var betLimits = new[] {
                new DreamGameBetLimit(){
                    Currency="CNY",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="EUR",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="GBP",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="HKD",
                    TableLimit="G"
                },
                new DreamGameBetLimit(){
                    Currency="IDR_1000",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="INR",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="JPY",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="KRW",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="KRW",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="MYR",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="PHP",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="SGD",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="THB",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="USD",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="VND_1000",
                    TableLimit="A"
                },
            };

            var query = new DreamGameCreatePlayerQuery(
                new("http://api.dg99web.com"),
                "974e8e9a072e4ad59a9e3f43a06ff9ae",
                 "DGTE0105CO",
                 "4ucmqplxb029",
                 "TgsP@ssw0rd",
                 "HKD",
                 "HKD",
                 betLimits);

            _ = fakeApiClient.SignUpAsync(query.ApiUrl,
                     query.ApiKey,
                     query.AgentName,
                     query.PlayerCode,
                     query.Password,
                     query.GameProviderCurrencyCode,
                     "G").Returns(0);

            var sut = new DreamGameCreatePlayerGetService(fakeApiClient);

            var actual = await sut.GetAsync(query).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(0, actual);
        }

        [Test]
        public async Task Normal_Process()
        {
            var fakeApiClient = Substitute.For<IDreamGameApiClient>();

            var gameUrl = new Uri("http://www.google.com");

            var betLimits = new[] {
                new DreamGameBetLimit(){
                    Currency="CNY",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="EUR",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="GBP",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="HKD",
                    TableLimit="G"
                },
                new DreamGameBetLimit(){
                    Currency="IDR_1000",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="INR",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="JPY",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="KRW",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="KRW",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="MYR",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="PHP",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="SGD",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="THB",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="USD",
                    TableLimit="A"
                },
                new DreamGameBetLimit(){
                    Currency="VND_1000",
                    TableLimit="A"
                },
            };

            var query = new DreamGameCreatePlayerQuery(
                new("http://api.dg99web.com"),
                "974e8e9a072e4ad59a9e3f43a06ff9ae",
                 "DGTE0105CO",
                 "4ucmqplxb029",
                 "TgsP@ssw0rd",
                 "CNY",
                 "CNY",
                 betLimits);

            _ = fakeApiClient.SignUpAsync(query.ApiUrl,
                     query.ApiKey,
                     query.AgentName,
                     query.PlayerCode,
                     query.Password,
                     query.GameProviderCurrencyCode,
                     "A").Returns(0);

            var sut = new DreamGameCreatePlayerGetService(fakeApiClient);

            var actual = await sut.GetAsync(query).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(0, actual);
        }
    }
}
