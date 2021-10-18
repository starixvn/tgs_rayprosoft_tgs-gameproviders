using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.Skywind;
using TGS.GameProviders.ApiClients.Skywind.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class SkywindApiClientUnitTest
    {
        private readonly ISkywindApiClient _client;
        private readonly Uri _apiUrl = new Uri("https://api.gcpstg.m27613.com/v1");
        private readonly Uri _liveLobbyUrl = new Uri("https://2rkp4pb7-lobby.ss211208.com");
        private const string _merchId = "sw_PG_merchant_stg";
        private const string _merchPwd = "tm8jhMq2Sc5DdxJ5";
        private const string _secretKey = "f841de30-a9eb-49d8-87c7-de878b571afe";
        private const string _username = "PG_merchant_api_stg";
        private const string _password = "3VSNMGFvbzBJacJT";
        private readonly Uri _serviceEndpointUrl = new Uri("http://game-sw");
        public SkywindApiClientUnitTest()
        {
            var client = new HttpClient();
            var faceLog = NullLogger<SkywindApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            _client = Substitute.For<ISkywindApiClient>();
            //_client = new SkywindApiClient(client, faceLog, faceTgsLog);

        }

        [Test]
        public async Task GetAccessTokenAsync()
        {
            _ = await _client.GetAccessTokenAsync(_apiUrl, new()
            {
                SecretKey = _secretKey,
                Username = _username,
                Password = _password
            });
        }

        [Test]
        public async Task GetGameUrlAsync()
        {
            var loginResponse = await _client.GetAccessTokenAsync(_apiUrl, new()
            {
                SecretKey = _secretKey,
                Username = _username,
                Password = _password
            });
            var request = new SkywindGameUrlRequest()
            {
                PlayerCode = "4ucmqplxb029",
                GameCode = "sw_fg",
                Ticket = "OZmqtPV4Wz4Q3mQ6haBZ3RBbB60kGMSHAsU3v5wfeYRS9IBQ43fQ17Du2A72vM8T0",
                Language = "zh-cn"
            };
            _ = await _client.GetGameUrlAsync(_apiUrl, loginResponse?.AccessToken, request);
        }

        [Test]
        public async Task GetGamHistoryRoundDetailsAsync()
        {
            var loginResponse = await _client.GetAccessTokenAsync(_apiUrl, new()
            {
                SecretKey = _secretKey,
                Username = _username,
                Password = _password
            });
            var request = new SkywindGamHistoryRoundDetailsRequest()
            {
                RoundId = "OWLAp2LPK",
                Language = "zh-cn"
            };
            _ = await _client.GetGamHistoryRoundDetailsAsync(_apiUrl, loginResponse?.AccessToken, request);
        }
    }
}
