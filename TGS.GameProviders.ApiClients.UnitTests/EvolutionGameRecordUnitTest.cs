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

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class EvolutionGameRecordUnitTest
    {
        private readonly Uri _apiUrl = new Uri("https://babylonqm.uat1.evo-test.com/");
        private readonly string _casinoID = "babylonqm0000001";
        private readonly IEvolutionApiClient _client;
        private readonly string _externalLobbyApiToken = "test123";
        private readonly string _gameHistoryApiToken = "test123";
        private readonly string _owAuthToken = "I2gcyH6kRTiHYAzLxy1RfH";
        private readonly Uri _serviceEndpointUrl = new Uri("http://game-evo");
        private readonly Uri _ua2AuthenticationUrl = new Uri("https://babylonqm.uat1.evo-test.com/ua/v1/babylonqm0000001/test123");
        private readonly string _ua2Token = "test123";

        public EvolutionGameRecordUnitTest()
        {
            var client = new HttpClient();
            var faceLog = NullLogger<EvolutionApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            _client = Substitute.For<IEvolutionApiClient>();
            //_client = new EvolutionApiClient(client, faceLog, faceTgsLog);
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

            _ = await _client.GetGameUrlAsync(_ua2AuthenticationUrl, request);
        }

        [Test]
        public async Task GetGamHistoryRoundAsync()
        {
            var request = new EvolutionGamHistoryRoundRequest()
            {
                PlayerCode = "4uerjhy1ls17",
                RoundId = "3563277-56"
            };

            _ = await _client.GetGamHistoryRoundAsync(_apiUrl, _casinoID, _gameHistoryApiToken, request);
        }
    }
}
