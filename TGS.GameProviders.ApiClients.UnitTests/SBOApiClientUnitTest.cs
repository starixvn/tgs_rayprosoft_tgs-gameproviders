using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.SBO;
using TGS.GameProviders.Core.SBO;
using TGS.GameProviders.Core.SBO.Models;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class SBOApiClientUnitTest
    {
        private readonly string _agentPrefix = "STGTGS";
        private readonly Uri _apiUrl = new("https://ex-api-demo-yy.568win.com");
        private readonly string _brandCode = "GPI";
        private readonly ISBOApiClient _client;

        private readonly string _companyKey = "34B2C4ED616F4E7B965EAE0861A3F92E";
        private readonly string _playerCurrencyCode = "CNY";
        private readonly string _gameProviderCurrencyCode = "CNY";

        public SBOApiClientUnitTest()
        {
            var httpClient = new HttpClient();
            var logger = NullLogger<SBOApiClient>.Instance;
            var tgsLog = Substitute.For<ITgsLog>();

            _client = Substitute.For<ISBOApiClient>();
            //_client = new SBOApiClient(httpClient, logger, tgsLog);
        }

        [Test]
        public async Task CreateAgentAsync()
        {
            var agentId =
                $"{_agentPrefix}{_brandCode.ToUpper()}{_playerCurrencyCode.Replace("_", string.Empty).ToUpper()}";

            var request = new SBOCreateAgentRequest
            {
                CompanyKey = _companyKey,
                AgentId = agentId,
                CurrencyCode = _gameProviderCurrencyCode
            };

            var response = await _client.CreateAgentAsync(_apiUrl, request).ConfigureAwait(false);
        }

        [Test]
        public async Task CreatePlayerAsync()
        {
            var playerCode = "4udwxysersic";

            var agentId =
                $"{_agentPrefix}{_brandCode.ToUpper()}{_playerCurrencyCode.Replace("_", string.Empty).ToUpper()}";

            var request = new SBOCreatePlayerRequest
            {
                CompanyKey = _companyKey,
                PlayerCode = playerCode,
                AgentId = agentId
            };

            var response = await _client.CreatePlayerAsync(_apiUrl, request).ConfigureAwait(false);
        }

        [Test]
        public async Task GetGameUrlAsync()
        {
            var playerCode = "4ucmqplxb029";

            var request = new SBOLoginRequest
            {
                CompanyKey = _companyKey,
                PlayerCode = playerCode,
                Portfolio = "SportsBook"
            };

            var response = await _client.LoginAsync(_apiUrl, request).ConfigureAwait(false);
        }

        [Test]
        public async Task KickAsync()
        {
            var playerCode = "4ucmqplxb029";

            var request = new SBOLogoutRequest
            {
                CompanyKey = _companyKey,
                PlayerCode = playerCode
            };

            var response = await _client.LogoutAsync(_apiUrl, request).ConfigureAwait(false);
        }
    }
}