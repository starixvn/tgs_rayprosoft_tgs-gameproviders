using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.PragmaticPlay;
using TGS.GameProviders.Core.PragmaticPlay;
using TGS.GameProviders.Core.PragmaticPlay.Models;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class PragmaticPlayApiClientUnitTest
    {
        private readonly Uri _apiUrl = new Uri("https://api.prerelease-env.biz");
        private readonly IPragmaticPlayApiClient _client;

        private readonly string _secretKey = "testKey";
        private readonly string _securityLogin = "tgs_tgs";

        public PragmaticPlayApiClientUnitTest()
        {
            var httpClient = new HttpClient();
            var logger = NullLogger<PragmaticPlayApiClient>.Instance;
            var tgsLog = Substitute.For<ITgsLog>();
            var utility = new PragmaticPlayUtility();

            //_client = new PragmaticPlayApiClient(httpClient, logger, tgsLog, utility);
            _client = Substitute.For<IPragmaticPlayApiClient>();
        }

        [Test]
        public async Task GetGameUrlAsync()
        {
            //var playerCode = "4ucmqplxb029";

            var request = new PragmaticPlayGameUrlRequest()
            {
                Token = "token",
                ExternalGameId = "vs50juicyfr",
                Language = "en",
                Platform = "WEB",
                LobbyUrl = "http://localhost:8081",
                SecureLogin = _securityLogin
            };

            var response = await _client.GetGameUrlAsync(_apiUrl, _secretKey, request).ConfigureAwait(false);
        }

        [Test]
        public async Task KickPlayerAsync()
        {
            var playerCode = "4ucmqplxb029";

            var request = new PragmaticPlayLogoutRequest
            {
                PlayerCode = playerCode,
                SecureLogin = _securityLogin
            };

            var response = await _client.LogoutAsync(_apiUrl, _secretKey, request).ConfigureAwait(false);
        }
    }
}
