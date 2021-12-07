using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.IMESports;
using TGS.GameProviders.Core.IMESports;
using TGS.GameProviders.Core.IMESports.Models;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class IMESportsApiClientUnitTest
    {
        private readonly Uri _apiUrl = new Uri("http://operatorapi.staging.imaegisapi.com");
        private readonly IIMESportsApiClient _client;

        private readonly string _merchantCode = "Ogt3HBRFebTqJczcN28CliP3BHqDWIYo";
        private readonly string _prefix = "IM1CP";
        private readonly int _productWallet = 401;

        public IMESportsApiClientUnitTest()
        {
            var httpClient = new HttpClient();
            var logger = NullLogger<IMESportsApiClient>.Instance;
            var tgsLog = Substitute.For<ITgsLog>();

            _client = Substitute.For<IIMESportsApiClient>();
            //_client = new IMESportsApiClient(logger, httpClient, tgsLog);
        }

        [Test]
        public async Task GetDesktopGameGameUrlAsync()
        {
            var playerCode = "4ucmqplxb029";
            var playerId = ToIMESportsPlayerId(playerCode);

            var request = new IMESportsLaunchGameRequest
            {
                MerchantCode = _merchantCode,
                IMESPlayerId = playerId,
                GameId = "ESPORTSBULL",
                Language = "EN",
                Http = "1",
                ProductWallet = _productWallet
            };

            var response = await _client.LaunchDesktopGameAsync(_apiUrl, request).ConfigureAwait(false);
        }

        [Test]
        public async Task GetMobileGameGameUrlAsync()
        {
            var playerCode = "4ucmqplxb029";
            var playerId = ToIMESportsPlayerId(playerCode);

            var request = new IMESportsLaunchGameRequest
            {
                MerchantCode = _merchantCode,
                IMESPlayerId = playerId,
                GameId = "ESPORTSBULL",
                Language = "EN",
                Http = "1",
                ProductWallet = _productWallet
            };

            var response = await _client.LaunchMobileGameAsync(_apiUrl, request).ConfigureAwait(false);
        }

        [Test]
        public async Task GetRoundHistoryAsync()
        {
            var request = new IMESportsGetRoundHistoryRequest
            {
                MerchantCode = _merchantCode,
                ProductWallet = _productWallet,
                StartDate = "2021-11-26 18.46.44",
                EndDate = "2021-11-26 18.49.44",
                Page = 1,
                Language = "EN"
            };

            var response = await _client.GetRoundHistoryAsync(_apiUrl, request);
        }

        [Test]
        public async Task RegisterPlayerAsync()
        {
            var playerCode = "4ucmqplxb029";
            var playerId = ToIMESportsPlayerId(playerCode);

            var request = new IMESportsRegisterPlayerRequest
            {
                MerchantCode = _merchantCode,
                IMESPlayerId = playerId,
                CurrencyCode = "CNY"
            };

            var response = await _client.RegisterPlayerAsync(_apiUrl, request)
                .ConfigureAwait(false);
        }

        private string ToIMESportsPlayerId(string playerCode)
            => $"{_prefix}_{playerCode}";
    }
}
