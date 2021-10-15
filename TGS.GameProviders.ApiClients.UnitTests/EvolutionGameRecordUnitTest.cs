using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.Evolution;
using TGS.GameProviders.ApiClients.Evolution.Models;
using TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGameUrl;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.ApiClients;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class EvolutionGameRecordUnitTest
    {
        private readonly IEvolutionApiClient _client;
        private readonly EvolutionConfiguration _configuration;

        public EvolutionGameRecordUnitTest()
        {
            var client = new HttpClient();
            var faceLog = NullLogger<EvolutionApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            _client = Substitute.For<IEvolutionApiClient>();
            //_client = new EvolutionApiClient(client, faceLog, faceTgsLog);
            _configuration = new EvolutionConfiguration
            {
                ApiUrl = new Uri("https://babylonqm.uat1.evo-test.com/"),
                CasinoID = "babylonqm0000001",
                Ua2Token = "test123",
                GameHistoryApiToken = "test123",
                ExternalLobbyApiToken = "test123",
                Ua2AuthenticationUrl = new Uri("https://babylonqm.uat1.evo-test.com/ua/v1/babylonqm0000001/test123"),
                OwAuthToken = "I2gcyH6kRTiHYAzLxy1RfH",
                ServiceEndpointUrl = new Uri("http://game-evo")
            };
        }

        [Test]
        public async Task GetGameUrlAsync()
        {
            var request = new EvolutionGameUrlRequest()
            {
                Uuid = GuidExtensions.NewSequentialGuid().ToString(),
                Player = new EvolutionGameUrlPlayer()
                {
                    Id = "4ucmqplxb029",
                    Update = false,
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    Nickname = "4ucmqplxb029",
                    Country = "CN",
                    Language = "cn",
                    Currency = "CNY",
                    Session = new EvolutionGameUrlPlayerSession()
                    {
                        Id = "rDLIWMPV7dgRstCYBzSY6EidJQh51TWgg8b7XgefrRt5QkjiCB5pwwNvRKSdGDPsc",
                        Ip = "127.0.0.1"
                    },
                    Group = default(EvolutionGameUrlPlayerGroup)
                },
                Config = new EvolutionGameUrlConfig()
                {
                    Game = new EvolutionGameUrlConfigGame()
                    {
                        Category = "baccarat_sicbo",
                        Table = null
                    },
                    Channel = new EvolutionGameUrlConfigChannel()
                    {
                        Wrapped = true,
                        Mobile = false
                    }
                }
            };

            _ = await _client.GetGameUrlAsync(_configuration, request);
        }

        [Test]
        public async Task GetGamHistoryRoundAsync()
        {
            var request = new EvolutionGamHistoryRoundRequest()
            {
                PlayerCode = "4uevujuqj4n6",
                RoundId = "6930499-297"
            };

            _ = await _client.GetGamHistoryRoundAsync(_configuration, request);
        }
    }
}
