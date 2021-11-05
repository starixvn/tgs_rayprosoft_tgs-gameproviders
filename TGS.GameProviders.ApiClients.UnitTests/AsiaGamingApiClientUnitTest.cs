using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.AsiaGaming;
using TGS.GameProviders.Core.AsiaGaming;
using TGS.GameProviders.Core.AsiaGaming.Models;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class AsiaGamingApiClientUnitTest
    {
        private readonly Uri _loginApiUrl = new("http://gi.aggdemo.com:81");
        private readonly Uri _serviceApiUrl = new("https://swapi.etwlt.com");

        private readonly string _agentCode = "JH2_AGIN";
        private readonly string _productionId = "JH2";

        private readonly string _desKey = "jh2hnM6K";
        private readonly string _md5Key = "jh2bKFbpLZuW";

        private readonly IAsiaGamingApiClient _client;
        private readonly string _gameProviderCurrencyCode = "CNY";

        public AsiaGamingApiClientUnitTest()
        {
            var httpClient = new HttpClient();
            var logger = NullLogger<AsiaGamingApiClient>.Instance;
            var tgsLog = Substitute.For<ITgsLog>();
            var utility = new AsiaGamingUtility();

            //_client = new AsiaGamingApiClient(logger, httpClient, utility, tgsLog);
            _client = Substitute.For<IAsiaGamingApiClient>();
        }

        [Test]
        public async Task CheckOrCreateMemberAsync()
        {
            var playerCode = "4ucmqplxb029";

            var request = new AsiaGamingCheckOrCreateAccountRequest()
            {
                AgentCode = _agentCode,
                PlayerCode = playerCode,
                Currency = _gameProviderCurrencyCode
            };

            var response = await _client.CheckOrCreateAccountAsync(_loginApiUrl, _md5Key, _desKey, request)
                .ConfigureAwait(false);
        }

        [Test]
        public async Task CreateGameSessionAsync()
        {
            var playerCode = "4ucmqplxb029";

            var request = new AsiaGamingCreateGameSessionRequest
            {
                ProductId = _productionId,
                PlayerCode = playerCode,
                LoginRecordId = Guid.NewGuid(),
                Balance = 0
            };

            var response = await _client.CreateGameSessionAsync(_serviceApiUrl, request)
                .ConfigureAwait(false);
        }
    }
}