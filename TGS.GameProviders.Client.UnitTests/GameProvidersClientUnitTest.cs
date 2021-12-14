using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Tgs.Core.Infrastructures.Security;
using TGS.GameProviders.Core.BetConstruct;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Client.IntegrationTests
{
    [TestFixture]
    public class GameProvidersClientUnitTest
    {
        [Test]
        public async Task Test_GameProvidersClient_GameBCUrlAsync()
        {
            var fakeTokenProvider = Substitute.For<ITokenProvider>();
            var fakeBetConstructService = Substitute.For<IBetConstructService>();

            await using var web = new TestGameProvidersApplication(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(fakeTokenProvider);
                    services.AddSingleton(fakeBetConstructService);
                    services.AddFakeNatsMessageQueue();
                });
            });

            fakeBetConstructService
                .GetSettingConfigurationAsync()
                .Returns(ValueTask.FromResult(new BetConstructConfiguration { PartnerId = "partnerId" }));

            var playerCode = "4ucmqplxb029";
            fakeTokenProvider
                .EncryptAsync(playerCode)
                .Returns("ZYE5oSH5I1hzmrgnQnh3R6");

            var httpClient = web.CreateClient();

            var sut = new GameProvidersClient(httpClient);

            var gameProviderCode = "BC";
            var gameType = GameType.Sportsbook;
            var gameId = 2100001;
            var brandCode = "GPI";
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
