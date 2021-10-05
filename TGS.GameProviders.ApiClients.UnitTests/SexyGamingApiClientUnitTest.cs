using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.SexyGaming;
using TGS.GameProviders.ApiClients.SexyGaming.Models;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.SexyGaming;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class SexyGamingApiClientUnitTest
    {
        private readonly ISexyGamingApiClient _client;
        private readonly SexyGamingConfiguration _configuration;

        private readonly Dictionary<int, SexyGamingGameProviderBetLimit> BetLimitMapper =
            new Dictionary<int, SexyGamingGameProviderBetLimit>
               {
                    {280301, new(GuidExtensions.NewSequentialGuid(), "CNY", 5, 500, 500)},
                    {280302, new(GuidExtensions.NewSequentialGuid(), "CNY", 10, 1000, 1000)},
                    {280303, new(GuidExtensions.NewSequentialGuid(), "CNY", 20, 2500, 2500)},
                    {282202, new(GuidExtensions.NewSequentialGuid(), "EUR", 1, 500, 500)},
                    {282204, new(GuidExtensions.NewSequentialGuid(), "EUR", 10, 2500, 2500)},
                    {282205, new(GuidExtensions.NewSequentialGuid(), "EUR", 10, 5000, 5000)},
                    {280605, new(GuidExtensions.NewSequentialGuid(), "IDR_1000", 50, 5000, 5000)},
                    {280606, new(GuidExtensions.NewSequentialGuid(), "IDR_1000", 100, 10000, 10000)},
                    {280607, new(GuidExtensions.NewSequentialGuid(), "IDR_1000", 200, 20000, 20000)},
                    {281301, new(GuidExtensions.NewSequentialGuid(), "IDR", 5000, 500000, 500000)},     //PTI
                    {281302, new(GuidExtensions.NewSequentialGuid(), "IDR", 5000, 50000000, 50000000)}, //PTI
                    {300403, new(GuidExtensions.NewSequentialGuid(), "JPY", 200, 50000, 50000)},
                    {280801, new(GuidExtensions.NewSequentialGuid(), "KRW", 5000, 100000, 100000)},
                    {280802, new(GuidExtensions.NewSequentialGuid(), "KRW", 100000, 10000000, 10000000)},
                    {280803, new(GuidExtensions.NewSequentialGuid(), "KRW", 200000, 20000000, 20000000)},
                    {280101, new(GuidExtensions.NewSequentialGuid(), "MYR", 1, 250, 250)},
                    {280102, new(GuidExtensions.NewSequentialGuid(), "MYR", 2, 500, 500)},
                    {280103, new(GuidExtensions.NewSequentialGuid(), "MYR", 5, 1000, 1000)},
                    {280908, new(GuidExtensions.NewSequentialGuid(), "THB", 100, 20000, 20000)},
                    {280909, new(GuidExtensions.NewSequentialGuid(), "THB", 500, 50000, 50000)},
                    {280910, new(GuidExtensions.NewSequentialGuid(), "THB", 1000, 100000, 100000)},
                    {280201, new(GuidExtensions.NewSequentialGuid(), "HKD", 2, 500, 500)},
                    {280203, new(GuidExtensions.NewSequentialGuid(), "HKD", 10, 20000, 20000)},
                    {280204, new(GuidExtensions.NewSequentialGuid(), "HKD", 20, 4000, 4000)},
                    {280205, new(GuidExtensions.NewSequentialGuid(), "HKD", 40, 10000, 10000)},
                    {280206, new(GuidExtensions.NewSequentialGuid(), "HKD", 5, 5000, 5000)},
                    {280701, new(GuidExtensions.NewSequentialGuid(), "USD", 1, 100, 100)},
                    {280702, new(GuidExtensions.NewSequentialGuid(), "USD", 1, 500, 500)},
                    {280703, new(GuidExtensions.NewSequentialGuid(), "USD", 5, 200, 200)},
                    {280704, new(GuidExtensions.NewSequentialGuid(), "USD", 5, 1000, 1000)},
                    {280705, new(GuidExtensions.NewSequentialGuid(), "USD", 10, 500, 500)},
                    {281101, new(GuidExtensions.NewSequentialGuid(), "VND_1000", 10, 500, 500)},
                    {281102, new(GuidExtensions.NewSequentialGuid(), "VND_1000", 20, 1000, 1000)},
                    {281103, new(GuidExtensions.NewSequentialGuid(), "VND_1000", 50, 2500, 2500)},
                    {281104, new(GuidExtensions.NewSequentialGuid(), "VND_1000", 100, 5000, 5000)},
                    {281105, new(GuidExtensions.NewSequentialGuid(), "VND_1000", 200, 10000, 10000)},
                    {281401, new(GuidExtensions.NewSequentialGuid(), "VND", 10000, 500000, 500000)} //PTV
               };

        public SexyGamingApiClientUnitTest()
        {
            var client = new HttpClient();
            var faceLog = NullLogger<SexyGamingApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            _client = Substitute.For<ISexyGamingApiClient>();
            //_client = new SexyGamingApiClient(client, faceLog, faceTgsLog);
            _configuration = new SexyGamingConfiguration
            {
                ServiceEndpointUrl = new Uri("http://game-sg"),
                ApiUrl = new("https://tttint.onlinegames22.com/"),
                AgentId = "tgs",
                Cert = "OFIHXL0mfK8AcohvyTs",
                BetLimit = new SexyGamingBetLimit
                {
                    SexybcrtMapper = BetLimitMapper.ToDictionary(o => o.Key, o => o.Value.CurrencyCode)
                }
            };
        }

        [Test]
        public async Task GetGameUrlAsync()
        {
            var request = new SexyGamingLoginGameRequest()
            {
                UserId = "4ucmqplxb029",
                IsMobileLogin = false,
                Extension1 = JsonSerializer.Serialize(new { token = "rDLIWMPV7dgRstCYBzSY6EidJQh51TWgg8b7XgefrRt5QkjiCB5pwwNvRKSdGDPsc" })
            };

            _ = await _client.GetGameUrlAsync(_configuration, request);
        }

        [Test]
        public async Task LogoutAsync()
        {
            var request = new SexyGamingLogoutRequest()
            {
                PlayerCode = "4ucmqplxb029",
            };

            _ = await _client.LogoutAsync(_configuration, request);
        }
    }
}
