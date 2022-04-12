using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.BigGaming;
using Tgs.GameProviders.Core.BigGaming.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest
{
    [TestFixture]
    public class BigGamingApiClientUnitTest
    {
        private readonly string _agentLoginIdPassword = "TgsP@ssw0rd";
        private readonly Uri _apiUrl = new("http://openapi-uat.bytechn.com/open-cloud/api/");
        private readonly string _secretCode = "jk06";

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
            IBigGamingApiClient client = Substitute.For<IBigGamingApiClient>();

            _ = await client.CreatePlayerAsync(_apiUrl, _secretCode, _agentLoginIdPassword, request);
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

            IBigGamingApiClient client = Substitute.For<IBigGamingApiClient>();

            _ = await client.GetGameUrlAsync(_apiUrl, _secretCode, _agentLoginIdPassword, request);
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

            IBigGamingApiClient client = Substitute.For<IBigGamingApiClient>();

            _ = await client.InfoPlayerAsync(_apiUrl, _agentLoginIdPassword, request);
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

            IBigGamingApiClient client = Substitute.For<IBigGamingApiClient>();

            _ = await client.KickPlayerAsync(_apiUrl, _agentLoginIdPassword, request);
        }
    }
}
