using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.AsiaGaming.ApplicationServices;
using TGS.GameProviders.Core.AsiaGaming.Models;
using TGS.GameProviders.Core.DreamGame;
using TGS.GameProviders.Core.DreamGame.Models;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.AsiaGaming;

namespace TGS.GameProviders.Client.IntegrationTests
{
    [TestFixture]
    public class DreamGameClientUnitTest
    {
        [Test]
        public async Task Test_GameRecordList()
        {
            var configuration = new DreamGameConfiguration
            {
                ServiceEndpointUrl = new("http://game-dg"),
                ApiUrl = new("http://api.dg99web.com"),
                ApiKey = "974e8e9a072e4ad59a9e3f43a06ff9ae",
                AgentName = "DGTE0105CO",
                PlayerPassword = "TgsP@ssw0rd"
            };

            var gameReportResults = new GameReportResult[] {
                new (){
                    Id= 10500435001,
                    TableId= 10101,
                    ShoeId= 16,
                    PlayId= 12,
                    LobbyId= 1,
                    GameType= 1,
                    GameId= 1,
                    MemberId= 72832718,
                    ParentId= 19377,
                    BetTime= "2022-03-14 08:57:57",
                    CalTime= "2022-03-14 08:58:14",
                    WinOrLoss= 50.0m,
                    BalanceBefore= 13096.531m,
                    BetPoints= 50.0m,
                    BetPointsz= 0.0m,
                    AvailableBet= 0.0m,
                    UserName= "4UCMQPLXB029",
                    Result= "{\"result\":\"6,1,7\",\"poker\":{\"banker\":\"16-42-0\",\"player\":\"39-20-0\"}}",
                    BetDetail= "{\"banker\":25.0,\"player\":25.0,\"playerW\":50.0}",
                    Ip= "220.132.206.70",
                    Ext= "220314B010760",
                    IsRevocation= 1,
                    CurrencyId= 1,
                    DeviceType= 1,
                    Serial= "-10500435479",
                    Roadid= 0,
                    Pluginid= 0 }};

            await using var web = new TestGameProvidersApplication(builder =>
            {
                _ = builder.ConfigureServices(services =>
                {

                    var fakeService = Substitute.For<IDreamGameService>();
                    _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
                    _ = fakeService.GetGameProviderIdAsync().Returns(Guid.NewGuid());
                    _ = services.AddTransient(_ => fakeService);

                    var fakeClient = Substitute.For<IDreamGameApiClient>();
                    _ = fakeClient.GetReportAsync(configuration.ApiUrl, configuration.ApiKey, configuration.AgentName).Returns(gameReportResults);
                    _ = services.AddTransient(_ => fakeClient);


                    _ = services.AddFakeNatsMessageQueue();
                });
            });


            var httpClient = web.CreateClient();

            var sut = new DreamGameClient(httpClient);

            var actual = await sut.GameRecordListAsync().ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(true, actual!.Success);
            Assert.AreEqual(true, actual.Datas!.Any());
        }

        [Test]
        public async Task Test_MarkGameRecordList()
        {
            var data = new string[] { "123456789" };

            var configuration = new DreamGameConfiguration
            {
                ServiceEndpointUrl = new("http://game-dg"),
                ApiUrl = new("http://api.dg99web.com"),
                ApiKey = "974e8e9a072e4ad59a9e3f43a06ff9ae",
                AgentName = "DGTE0105CO",
                PlayerPassword = "TgsP@ssw0rd"
            };

            await using var web = new TestGameProvidersApplication(builder =>
            {
                _ = builder.ConfigureServices(services =>
                {

                    var fakeService = Substitute.For<IDreamGameService>();
                    _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
                    _ = services.AddTransient(_ => fakeService);

                    var fakeClient = Substitute.For<IDreamGameApiClient>();
                    _ = fakeClient.MarkReportAsync(configuration.ApiUrl, configuration.ApiKey, configuration.AgentName, data).Returns(true);
                    _ = services.AddTransient(_ => fakeClient);


                    _ = services.AddFakeNatsMessageQueue();
                });
            });

            var httpClient = web.CreateClient();

            var sut = new DreamGameClient(httpClient);

            await sut.MarkGameRecordListAsync(data).ConfigureAwait(false);
        }
    }
}
