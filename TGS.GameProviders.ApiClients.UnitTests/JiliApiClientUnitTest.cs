using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.ApiClients.Jili;
using TGS.GameProviders.Core.Jili;
using TGS.GameProviders.Core.Jili.Models;
using Tgs.Logging;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class JiliApiClientUnitTest
    {
        private readonly Uri _apiUrl = new Uri("https://uat-wb-api.jlfafafa2.com/singleWallet");
        private readonly string _agentId = "PG_Seamless";
        private readonly string _agentKey = "ed7c0450d69bcec7ddf42124ecb7493ef9597083";

        private readonly IJiliApiClient _client;

        public JiliApiClientUnitTest()
        {
            var httpClient = new HttpClient();
            var logger = NullLogger<JiliApiClient>.Instance;
            var tgsLog = Substitute.For<ITgsLog>();

            _client = new JiliApiClient(logger, httpClient, tgsLog);
            //_client = Substitute.For<IJiliApiClient>();
        }

        [Test]
        public async ValueTask LoginWithoutRedirectAsync()
        {
            var request = new JiliLoginWithoutRedirectRequest
            {
                GameExternalId = "10",
                Token = "token",
                Language = "en-US"
            };

            var response = await _client
                .LoginWithoutRedirectAsync(_apiUrl, _agentId, _agentKey, request)
                .ConfigureAwait(false);

            Assert.AreEqual("no api callback", response.ErrorMessage);
        }
    }
}