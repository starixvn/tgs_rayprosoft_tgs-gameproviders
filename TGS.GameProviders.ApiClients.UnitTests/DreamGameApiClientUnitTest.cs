using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.DreamGame;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.DreamGame;
using TGS.GameProviders.Core.DreamGame.Models;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class DreamGameApiClientUnitTest
    {
        private readonly string _agentName = "DGTE0105CO";
        private readonly string _apiKey = "974e8e9a072e4ad59a9e3f43a06ff9ae";
        private readonly Uri _apiUrl = new("http://api.dg99web.com");
        private readonly Uri _videoGameApiUrl = new("https://ag.dg66.info");

        public async Task GetAgentFix_Actual()
        {
            var httpClient = new HttpClient();
            var faceLog = NullLogger<DreamGameApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            IDreamGameApiClient client = new DreamGameApiClient(httpClient, faceLog, faceTgsLog);
            var actual = await client.GetAgentFixAsync(_videoGameApiUrl, _agentName);

            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task GetAgentFix_UnitTest()
        {
            IDreamGameApiClient client = Substitute.For<IDreamGameApiClient>();
            _ = client.GetAgentFixAsync(_videoGameApiUrl, _agentName).Returns(ValueTask.FromResult("3213"));
            var actual = await client.GetAgentFixAsync(_videoGameApiUrl, _agentName);

            Assert.IsNotNull(actual);
            Assert.AreEqual("3213", actual);
        }

        public void GetGameResult_Actual()
        {
            var httpClient = new HttpClient();
            var faceLog = NullLogger<DreamGameApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            IDreamGameApiClient client = new DreamGameApiClient(httpClient, faceLog, faceTgsLog);
            var actual = client.GetGameResultAsync(_videoGameApiUrl, _agentName, "cn", "3213", "10500435001");

            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetGameResult_UnitTest()
        {
            IDreamGameApiClient client = Substitute.For<IDreamGameApiClient>();
            var expected = "https://ag.dg66.info/ag/result/result.html?language=cn&agentFix=3213&id=10500435001";
            _ = client.GetGameResultAsync(_videoGameApiUrl, _agentName, "cn", "3213", "10500435001").Returns(expected);
            var actual = client.GetGameResultAsync(_videoGameApiUrl, _agentName, "cn", "3213", "10500435001");

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        public async Task GetReport_Actual()
        {
            var httpClient = new HttpClient();
            var faceLog = NullLogger<DreamGameApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            IDreamGameApiClient client = new DreamGameApiClient(httpClient, faceLog, faceTgsLog);
            var actual = await client.GetReportAsync(_apiUrl, _apiKey, _agentName);
            Assert.IsNotNull(actual);
            Assert.AreEqual(true, actual.Any());
        }

        [Test]
        public async Task GetReport_UnitTest()
        {
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

            IDreamGameApiClient client = Substitute.For<IDreamGameApiClient>();
            _ = client.GetReportAsync(_apiUrl, _apiKey, _agentName).Returns(ValueTask.FromResult<IEnumerable<GameReportResult>>(gameReportResults));
            var actual = await client.GetReportAsync(_apiUrl, _apiKey, _agentName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(true, actual.Any());
        }

        public async Task Login_Actual()
        {
            var httpClient = new HttpClient();
            var faceLog = NullLogger<DreamGameApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            IDreamGameApiClient client = new DreamGameApiClient(httpClient, faceLog, faceTgsLog);
            var actual = await client.LoginAsync(_apiUrl, _apiKey, _agentName, "4ucmqplxb029", "TgsP@ssw0rd", PlatformType.Desktop);

            Assert.IsNotNull(actual);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(0, actual.CodeId);
        }

        [Test]
        public async Task Login_UnitTest()
        {
            IDreamGameApiClient client = Substitute.For<IDreamGameApiClient>();
            var expected = new LoginResult(true, 0, new("https://www.google.com/"));
            _ = client.LoginAsync(_apiUrl, _apiKey, _agentName, "4ucmqplxb029", "TgsP@ssw0rd", PlatformType.Desktop).Returns(ValueTask.FromResult(expected));
            var actual = await client.LoginAsync(_apiUrl, _apiKey, _agentName, "4ucmqplxb029", "TgsP@ssw0rd", PlatformType.Desktop);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Success, actual.Success);
            Assert.AreEqual(expected.CodeId, actual.CodeId);
            Assert.AreEqual(expected.Url, actual.Url);
        }

        public async Task Offline_Actual()
        {
            var httpClient = new HttpClient();
            var faceLog = NullLogger<DreamGameApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            IDreamGameApiClient client = new DreamGameApiClient(httpClient, faceLog, faceTgsLog);
            var actual = await client.OfflineAsync(_apiUrl, _apiKey, _agentName, 72832718);
            Assert.IsNotNull(actual);
            Assert.AreEqual(true, actual);
        }

        [Test]
        public async Task Offline_UnitTest()
        {
            IDreamGameApiClient client = Substitute.For<IDreamGameApiClient>();
            _ = client.OfflineAsync(_apiUrl, _apiKey, _agentName, 72832718).Returns(ValueTask.FromResult(true));
            var actual = await client.OfflineAsync(_apiUrl, _apiKey, _agentName, 72832718);

            Assert.IsNotNull(actual);
            Assert.AreEqual(true, actual);
        }

        public async Task OnlineReport_Actual()
        {
            var httpClient = new HttpClient();
            var faceLog = NullLogger<DreamGameApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            IDreamGameApiClient client = new DreamGameApiClient(httpClient, faceLog, faceTgsLog);
            var actual = await client.OnlineReportAsync(_apiUrl, _apiKey, _agentName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual);
        }

        [Test]
        public async Task OnlineReport_UnitTest()
        {
            var onlineReportResults = new OnlineReportResult[] {
                new (){ Username ="4UCMQPLXB029",
                Nickname ="29",
                CurrencyName ="CNY",
                Ip ="220.132.206.70",
                Device="1",
                Login ="2022-03-14 09:11:03",
                MemberId =72832718}
            };

            IDreamGameApiClient client = Substitute.For<IDreamGameApiClient>();
            _ = client.OnlineReportAsync(_apiUrl, _apiKey, _agentName).Returns(ValueTask.FromResult<IEnumerable<OnlineReportResult>>(onlineReportResults));
            var actual = await client.OnlineReportAsync(_apiUrl, _apiKey, _agentName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(true, actual.Any());
        }

        public async Task SignUp_Actual()
        {
            var httpClient = new HttpClient();
            var faceLog = NullLogger<DreamGameApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            IDreamGameApiClient client = new DreamGameApiClient(httpClient, faceLog, faceTgsLog);
            var actual = await client.SignUpAsync(_apiUrl, _apiKey, _agentName, "4ucmqplxb029", "TgsP@ssw0rd", "CNY");

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual);
        }

        [Test]
        public async Task SignUp_UnitTest()
        {
            IDreamGameApiClient client = Substitute.For<IDreamGameApiClient>();
            _ = client.SignUpAsync(_apiUrl, _apiKey, _agentName, "4ucmqplxb029", "TgsP@ssw0rd", "CNY").Returns(ValueTask.FromResult(0));
            var actual = await client.SignUpAsync(_apiUrl, _apiKey, _agentName, "4ucmqplxb029", "TgsP@ssw0rd", "CNY");

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual);
        }
    }
}
