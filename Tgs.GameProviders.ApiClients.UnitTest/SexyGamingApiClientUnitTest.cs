using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.ApiClients.SexyGaming;
using Tgs.GameProviders.Core.SexyGaming;
using Tgs.GameProviders.Core.SexyGaming.Models;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.UnitTest
{
    [TestFixture]
    public class SexyGamingApiClientUnitTest
    {
        private const string _agentId = "tgs";
        private const string _cert = "OFIHXL0mfK8AcohvyTs";
        private readonly Uri _apiUrl = new Uri("https://tttint.onlinegames22.com/");

        private readonly Dictionary<int, string> _betLimitMapper =
            new Dictionary<int, string>
               {
                    {280301,  "CNY"},
                    {280302,  "CNY"},
                    {280303,  "CNY"},
                    {282202,  "EUR"},
                    {282204,  "EUR"},
                    {282205,  "EUR"},
                    {280605,  "IDR_1000"},
                    {280606,  "IDR_1000"},
                    {280607,  "IDR_1000"},
                    {281301,  "PTI"}, //IDR
                    {281302,  "PTI"}, //IDR
                    {300403,  "JPY"},
                    {280801,  "KRW"},
                    {280802,  "KRW"},
                    {280803,  "KRW"},
                    {280101,  "MYR"},
                    {280102,  "MYR"},
                    {280103,  "MYR"},
                    {280908,  "THB"},
                    {280909,  "THB"},
                    {280910,  "THB"},
                    {280201,  "HKD"},
                    {280203,  "HKD"},
                    {280204,  "HKD"},
                    {280205,  "HKD"},
                    {280206,  "HKD"},
                    {280701,  "USD"},
                    {280702,  "USD"},
                    {280703,  "USD"},
                    {280704,  "USD"},
                    {280705,  "USD"},
                    {281101,  "VND_1000"},
                    {281102,  "VND_1000"},
                    {281103,  "VND_1000"},
                    {281104,  "VND_1000"},
                    {281105,  "VND_1000"},
                    {281401,  "PTV"} //VND
               };

        private readonly ISexyGamingApiClient _client;

        public SexyGamingApiClientUnitTest()
        {
            var client = new HttpClient();
            var faceLog = NullLogger<SexyGamingApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            _client = Substitute.For<ISexyGamingApiClient>();
            //_client = new SexyGamingApiClient(client, faceLog, faceTgsLog);
        }

        [Test]
        public async Task CreatePlayerAsync()
        {
            var playerCode = "4ucmqplxb029";
            var currencyCode = "CNY";
            var cultureCode = "cn";
            var BetLime = _betLimitMapper.Where(x => x.Value == currencyCode).Select(x => x.Key).ToArray();

            var request = new SexyGamingCreateMemberRequest
            {
                AgentId = _agentId,
                Cert = _cert,
                UserId = playerCode,
                Currency = currencyCode,
                BetLimit = JsonSerializer.Serialize(new
                {
                    SEXYBCRT = new
                    {
                        LIVE = new
                        {
                            limitId = BetLime
                        }
                    }
                }),
                Language = cultureCode
            };
            _ = await _client.CreatePlayerAsync(_apiUrl, request);
        }

        [Test]
        public async Task GetGamingGameUrlAsync()
        {
            var request = new SexyGamingGameUrlRequest()
            {
                AgentId = _agentId,
                Cert = _cert,
                UserId = "4ucmqplxb029",
                IsMobileLogin = false,
                Extension1 = JsonSerializer.Serialize(new { token = "rDLIWMPV7dgRstCYBzSY6EidJQh51TWgg8b7XgefrRt5QkjiCB5pwwNvRKSdGDPsc" })
            };

            _ = await _client.GetGamingGameUrlAsync(_apiUrl, request);
        }

        [Test]
        public async Task GetGamingLoginGameAsync()
        {
            var request = new SexyGamingLoginGameRequest()
            {
                AgentId = _agentId,
                Cert = _cert,
                UserId = "4ucmqplxb029",
                IsMobileLogin = false,
                Extension1 = JsonSerializer.Serialize(new { token = "rDLIWMPV7dgRstCYBzSY6EidJQh51TWgg8b7XgefrRt5QkjiCB5pwwNvRKSdGDPsc" })
            };

            _ = await _client.GetGamingLoginGameAsync(_apiUrl, request);
        }

        [Test]
        public async Task LogoutAsync()
        {
            var request = new SexyGamingLogoutRequest()
            {
                AgentId = _agentId,
                Cert = _cert,
                PlayerCode = "4ucmqplxb029",
            };

            _ = await _client.LogoutAsync(_apiUrl, request);
        }
    }
}
