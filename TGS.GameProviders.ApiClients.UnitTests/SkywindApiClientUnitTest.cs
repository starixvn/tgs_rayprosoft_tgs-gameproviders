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
using TGS.GameProviders.ApiClients;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class SkywindApiClientUnitTest
    {
        private readonly ISkywindApiClient _client;
        private readonly SkywindConfiguration _configuration;

        public SkywindApiClientUnitTest()
        {
            var client = new HttpClient();
            var faceLog = NullLogger<SkywindApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            _client = Substitute.For<ISkywindApiClient>();
            //_client = new SkywindApiClient(client, faceLog, faceTgsLog);
            _configuration = new SkywindConfiguration
            {
                ApiUrl = new Uri("https://api.gcpstg.m27613.com/v1"),
                LiveLobbyUrl = new Uri("https://2rkp4pb7-lobby.ss211208.com"),
                MerchId = "sw_PG_merchant_stg",
                MerchPwd = "tm8jhMq2Sc5DdxJ5",
                SecretKey = "f841de30-a9eb-49d8-87c7-de878b571afe",
                Username = "PG_merchant_api_stg",
                Password = "3VSNMGFvbzBJacJT",
                ServiceEndpointUrl = new Uri("http://game-sw")
            };
        }

        [Test]
        public async Task GetAccessTokenAsync()
        {
            _ = await _client.GetAccessTokenAsync(_configuration);
        }

        [Test]
        public async Task GetGameUrlAsync()
        {
            var loginResponse = await _client.GetAccessTokenAsync(_configuration);
            var request = new SkywindGameUrlRequest()
            {
                PlayerCode = "4ucmqplxb029",
                GameCode = "sw_fg",
                Ticket = "2Em5iT5tzqjAWu2rDa6COFzWfA7wruEngVWckhIPZ5L7vl6V1QybL922KWNzLPsVo",
                Language = "zh-cn"
            };
            _ = await _client.GetGameUrlAsync(_configuration, loginResponse?.AccessToken, request);
        }
    }
}
