using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.WGSports;
using TGS.GameProviders.Core.WGSports;
using TGS.GameProviders.Core.WGSports.Models;
using TGS.GameProviders.Core.WGSports.Models.Commons;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class WGSportsApiClientUnitTest
    {
        private readonly Uri _apiUrl = new("https://api.jsc080.net/");
        private readonly IWGSportsApiClient _client;
        private readonly string _partnerCode = "TA";
        private readonly string _partnerIV = "cba1238888888888";
        private readonly string _partnerKey = "abc1238888888888";
        private readonly IWGSportsUtility _utility;

        public WGSportsApiClientUnitTest()
        {
            _utility = new WGSportsUtility();
            var client = new HttpClient();
            var faceLog = NullLogger<WGSportsApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            _client = Substitute.For<IWGSportsApiClient>();
            //_client = new WGSportsApiClient(client, faceLog, faceTgsLog, _utility);
        }

        [Test]
        public async Task AccessMoneyQueryAsync()
        {
            var data = new WGSportsAccessMoneyQueryRequest()
            {
                OrderId = "5671e95562f34674a4491bfc3e6aefb1",
            };
            _ = _client.AccessMoneyQueryAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).Returns(ValueTask.FromResult<WGSportsResponse>(
                new()
                {
                    ReturnCode = 1
                }));
            var result = await _client.AccessMoneyQueryAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).ConfigureAwait(false);
            Assert.AreEqual(1, result.ReturnCode);
        }

        [Test]
        public async Task FundInAsync()
        {
            var playerCode = "4ucmqplxb029";
            var data = new WGSportsAccessMoneyRequest()
            {
                PlayerCode = playerCode,
                AmountType = WGSportsAmountTypes.FundIn,
                OrderId = Guid.NewGuid().ToString("N"),
                Amount = "1"
            };
            _ = _client.AccessMoneyAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).Returns(ValueTask.FromResult<WGSportsResponse>(
                new()
                {
                    ReturnCode = 1
                }));
            var result = await _client.AccessMoneyAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).ConfigureAwait(false);
            Assert.AreEqual(1, result.ReturnCode);
        }

        [Test]
        public async Task FundOutAsync()
        {
            var playerCode = "4ucmqplxb029";
            var data = new WGSportsAccessMoneyRequest()
            {
                PlayerCode = playerCode,
                AmountType = WGSportsAmountTypes.FundOut,
                OrderId = Guid.NewGuid().ToString("N"),
                Amount = "1"
            };
            _ = _client.AccessMoneyAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).Returns(ValueTask.FromResult<WGSportsResponse>(
                new()
                {
                    ReturnCode = 1
                }));
            var result = await _client.AccessMoneyAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).ConfigureAwait(false);
            Assert.AreEqual(1, result.ReturnCode);
        }

        [Test]
        public async Task GetBalanceAsync()
        {
            var data = new WGSportsGetBalanceRequest()
            {
                PlayerCode = "4ucmqplxb029"
            };

            _ = await _client.GetBalanceAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).ConfigureAwait(false);
        }

        [Test]
        public async Task GetLoginAsync()
        {
            var playerCode = "4ucmqplxb029";
            var data = new WGSportsPrepareLoginRequest()
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                PlayerCode = playerCode,
                PlayerNickname = playerCode,
                CurrencyCode = "RMB"
            };
            var token = new WGSportsPrepareLoginResponse()
            {
                Token = Guid.NewGuid().ToString("N")
            };
            var prepareLoginResponse = await _client.GetPrepareLoginAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).ConfigureAwait(false);
            var loginRequest = new WGSportsLoginRequest
            {
                Token = prepareLoginResponse?.Token,
                Lang = ""
            };
            var response = await _client.GetLoginAsync(_apiUrl, _partnerCode, loginRequest).ConfigureAwait(false);
        }

        [Test]
        public async Task GetPrepareLoginAsync()
        {
            var playerCode = "4ucmqplxb029";
            var data = new WGSportsPrepareLoginRequest()
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                PlayerCode = playerCode,
                PlayerNickname = playerCode,
                CurrencyCode = "RMB"
            };
            _ = await _client.GetPrepareLoginAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).ConfigureAwait(false);
        }

        [Test]
        public async Task LogoutAsync()
        {
            _ = await _client.LogoutAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, "4ucmqplxb029").ConfigureAwait(false);
        }
    }
}
