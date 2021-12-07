using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Client.IntegrationTests
{
    [TestFixture]
    public class GameProvidersClientUnitTest
    {
        [Test]
        public async Task Test_GameProvidersClient_GameBCUrlAsync()
        {
            await using var web = new TestGameProvidersApplication(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddFakeNatsMessageQueue();
                });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersClient(httpClient);

            var gameProviderCode = "BC";
            var gameType = GameType.Sportsbook;
            var gameId = 2100001;
            var brandCode = "GPI";
            var playerCode = "4ucmqplxb029";
            var currencyCode = "CNY";
            var loginRecordId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var platformType = PlatformType.Desktop;
            var language = "zh-CN";
            var ipAddress = "127.0.0.1";
            var lobbyDomain = "https://devsportsbook.srvttt.com";

            var actual = await sut.GameBetConstructUrlAsync(
                gameProviderCode,
                gameType,
                gameId,
                brandCode,
                playerCode,
                currencyCode,
                loginRecordId,
                platformType,
                language,
                ipAddress,
                null,
                lobbyDomain);

            Assert.IsNotNull(actual);
        }
    }
}