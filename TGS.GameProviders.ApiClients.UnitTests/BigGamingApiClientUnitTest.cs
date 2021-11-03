using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.BigGaming;
using TGS.GameProviders.Core.BigGaming;
using TGS.GameProviders.Core.BigGaming.Models;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class BigGamingApiClientUnitTest
    {
        private readonly string _agentLoginIdPassword = "TgsP@ssw0rd";
        private readonly Uri _apiUrl = new("http://openapi-uat.bytechn.com/open-cloud/api/");
        private readonly IBigGamingApiClient _client;
        private readonly string _currencyCode = "CNY";
        private readonly string _secretCode = "jk06";
        private readonly string _secretKey = "8153503006031672EF300005E5EF6AEF";
        private readonly IBigGamingUtility _utility;

        public BigGamingApiClientUnitTest()
        {
            _utility = new BigGamingUtility();
            var client = new HttpClient();
            var faceLog = NullLogger<BigGamingApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            _client = Substitute.For<IBigGamingApiClient>();
            //_client = new BigGamingApiClient(client, faceLog, faceTgsLog, _utility);
        }

        [Test]
        public async Task CreatePlayerAsync()
        {
            var request = new BigGamingCreatePlayerRequest()
            {
                Random = Guid.NewGuid().ToString(),
                Sn = _secretCode,
                LoginId = "4ucmqplxb029",
                Nickname = "4ucmqplxb029",
                AgentLoginId = "GPI"
            };

            _ = await _client.CreatePlayerAsync(_apiUrl, _secretCode, _agentLoginIdPassword, request);
        }

        [Test]
        public async Task GetGameUrlLiveDealer()
        {
            var request = new BigGamingGetGameUrlLiveDealerRequest()
            {
                Random = Guid.NewGuid().ToString(),
                Sn = _secretCode,
                LoginId = "4ucmqplxb029",
                Locale = "zh_CN",
                IsMobileUrl = 0,
                IsHttpsUrl = 0,
                ReturnUrl = string.Empty,
                FromIp = "127.0.0.1"
            };

            _ = await _client.GetGameUrlAsync(_apiUrl, _secretCode, _agentLoginIdPassword, request);
        }

        [Test]
        public async Task InfoPlayerAsync()
        {
            var request = new BigGamingInfoPlayerRequest()
            {
                Random = Guid.NewGuid().ToString(),
                Sn = _secretCode,
                LoginId = "4ucmqplxb029"
            };

            _ = await _client.InfoPlayerAsync(_apiUrl, _agentLoginIdPassword, request);
        }

        [Test]
        public async Task KickPlayerAsync()
        {
            var request = new BigGamingKickPlayerRequest()
            {
                Random = Guid.NewGuid().ToString(),
                Sn = _secretCode,
                LoginId = "4ucmqplxb029"
            };

            _ = await _client.KickPlayerAsync(_apiUrl, _agentLoginIdPassword, request);
        }
    }
}
