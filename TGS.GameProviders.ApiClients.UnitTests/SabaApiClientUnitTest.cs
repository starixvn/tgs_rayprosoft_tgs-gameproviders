using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.Saba;
using TGS.GameProviders.Core.Saba;
using TGS.GameProviders.Core.Saba.Models;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class SabaApiClientUnitTest
    {
        private readonly Uri _apiUrl = new Uri("http://t829tsa.bw6688.com");
        private readonly ISabaApiClient _client;

        private readonly string _operatorId = "ZhiFeng055";
        private readonly string _vendorId = "zzdaa12nu1";

        public SabaApiClientUnitTest()
        {
            var httpClient = new HttpClient();
            var logger = NullLogger<SabaApiClient>.Instance;
            var tgsLog = Substitute.For<ITgsLog>();

            _client = Substitute.For<ISabaApiClient>();
            //_client = new SabaApiClient(logger, httpClient, tgsLog);
        }

        [Test]
        public async Task CreatePlayerAsync()
        {
            var playerCode = "4ucmqplxb029";
            var playerId = $"{playerCode}_test";

            var request = new SabaCreateMemberRequest
            {
                VendorId = _vendorId,
                PlayerId = playerId,
                CurrencyCode = "20",
                OperatorId = _operatorId,
                OddsType = "1",
                Username = playerId
            };

            var response = await _client.CreateMemberAsync(_apiUrl, request)
                .ConfigureAwait(false);
        }

        [Test]
        public async Task GetBetDetailByTransIdAsync()
        {
            var externalTransactionId = "113995497149";

            var request = new SabaGetRoundHistoryRequest
            {
                VendorId = _vendorId,
                ExternalTransactionId = externalTransactionId
            };

            var response = await _client.GetBetDetailByTransIdAsync(_apiUrl, request).ConfigureAwait(false);
        }

        [Test]
        public async Task GetGameUrlAsync()
        {
            var playerCode = "4ucmqplxb029";
            var playerId = $"{playerCode}_test";

            var request = new SabaGameUrlRequest
            {
                VendorId = _vendorId,
                Platform = "1",
                PlayerId = playerId
            };

            var response = await _client.GetGameUrlAsync(_apiUrl, request).ConfigureAwait(false);
        }

        [Test]
        public async Task KickUserAsync()
        {
            var playerCode = "4ucmqplxb029";
            var playerId = $"{playerCode}_test";

            var request = new SabaKickUserRequest
            {
                VendorId = _vendorId,
                PlayerId = playerId
            };

            var response = await _client.KickUserAsync(_apiUrl, request).ConfigureAwait(false);
        }
    }
}
