using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.WMCasino;
using TGS.GameProviders.Core.WMCasino;
using TGS.GameProviders.Core.WMCasino.Models;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class WMCasinoApiClientUnitTest
    {
        private readonly string _agentId = "hpgtwapi";
        private readonly string _agentPrefix = "pg";
        private readonly string _agentSuffix = "cny";
        private readonly Uri _apiUrl = new("https://api.a45.me/api/wallet/Gateway.php");
        private readonly string _brandCode = "GPI";
        private readonly IWMCasinoApiClient _client;
        private readonly string _currencyCode = "CNY";
        private readonly string _signature = "97ffc7ab86c2f54be533e90da6f6ab9f";

        public WMCasinoApiClientUnitTest()
        {
            var httpClient = new HttpClient();
            var logger = NullLogger<WMCasinoApiClient>.Instance;
            var tgsLog = Substitute.For<ITgsLog>();

            _client = Substitute.For<IWMCasinoApiClient>();
            //_client = new WMCasinoApiClient(httpClient, logger, tgsLog);
        }

        [Test]
        public async Task CreatePlayerAsync()
        {
            var playerCode = "4ucmqplxb029";
            var playerId = ToWMCasinoPlayerId(playerCode);

            var request = new WMCasinoMemberRegisterRequest
            {
                Signature = _signature,
                VendorId = _agentId,
                User = playerId,
                Username = playerCode
            };

            var response = await _client.MemberRegisterAsync(_apiUrl, request).ConfigureAwait(false);
        }

        [Test]
        public async Task GetGameUrlAsync()
        {
            var playerCode = "4ucmqplxb029";
            var playerId = ToWMCasinoPlayerId(playerCode);

            var request = new WMCasinoLoginGameRequest
            {
                Signature = _signature,
                VendorId = _agentId,
                User = playerId
            };

            var response = await _client.LoginGameAsync(_apiUrl, request).ConfigureAwait(false);
        }

        [Test]
        public async Task KickAsync()
        {
            var playerCode = "4ucmqplxb029";
            var playerId = ToWMCasinoPlayerId(playerCode);

            var request = new WMCasinoLogoutGameRequest
            {
                Signature = _signature,
                VendorId = _agentId,
                User = playerId
            };

            var response = await _client.LogoutGameAsync(_apiUrl, request).ConfigureAwait(false);
        }

        private string ToWMCasinoPlayerId(string playerCode)
            => $"{_agentPrefix}_{playerCode}_{_agentSuffix}";
    }
}
