using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.ApiClients.BigGaming;
using TGS.GameProviders.ApiClients.BigGaming.Models;
using TGS.GameProviders.Domains.SettingConfiguration.BigGaming;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class BigGamingApiClientUnitTest
    {

        private readonly IBigGamingApiClient _client;
        private readonly BigGamingBrandMerchant _merchant;
        private readonly IBigGamingUtility _utility;

        public BigGamingApiClientUnitTest()
        {
            _utility = new BigGamingUtility();
            var client = new HttpClient();
            _client = Substitute.For<IBigGamingApiClient>();
            //_client = new BigGamingApiClient(client, _utility);
            _merchant = new BigGamingBrandMerchant
            {

                ApiUrl = new Uri("http://openapi-uat.bytechn.com/open-cloud/api/"),
                CurrencyCode = "CNY",
                AgentLoginIdPassword = "TgsP@ssw0rd",
                SecretKey = "8153503006031672EF300005E5EF6AEF",
                SecretCode = "jk06"
            };
        }

        [Test]
        public async Task GetGameUrlLiveDealer()
        {
            var request = new BigGamingGetGameUrlLiveDealerRequest()
            {
                Random = Guid.NewGuid().ToString(),
                Sn = _merchant.SecretCode,
                LoginId = "4ucmqplxb029",
                Locale = "zh_CN",
                IsMobileUrl = 0,
                IsHttpsUrl = 0,
                ReturnUrl = string.Empty,
                FromIp = "127.0.0.1"
            };

            _ = await _client.GetGameUrlAsync(_merchant, request);
        }

        [Test]
        public async Task KickPlayerAsync()
        {

            var request = new BigGamingKickPlayerRequest()
            {
                Random = Guid.NewGuid().ToString(),
                Sn = _merchant.SecretCode,
                LoginId = "4ucmqplxb029"
            };

            _ = await _client.KickPlayerAsync(_merchant, request);
        }

        [Test]
        public async Task InfoPlayerAsync()
        {

            var request = new BigGamingInfoPlayerRequest()
            {
                Random = Guid.NewGuid().ToString(),
                Sn = _merchant.SecretCode,
                LoginId = "4ucmqplxb029"
            };

            _ = await _client.InfoPlayerAsync(_merchant, request);
        }

        [Test]
        public async Task CreatePlayerAsync()
        {

            var request = new BigGamingCreatePlayerRequest()
            {
                Random = Guid.NewGuid().ToString(),
                Sn = _merchant.SecretCode,
                LoginId = "4ucmqplxb029",
                Nickname = "4ucmqplxb029",
                AgentLoginId = "GPI"
            };

            _ = await _client.CreatePlayerAsync(_merchant, request);
        }
    }
}
