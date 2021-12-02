using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.WGHorse;
using TGS.GameProviders.Core.WGHorse;
using TGS.GameProviders.Core.WGHorse.Models;
using TGS.GameProviders.Core.WGHorse.Models.Commons;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class WGHorseApiClientUnitTest
    {
        private const string _partnerCode = "PT";
        private const string _partnerIV = "b888888888888888";
        private const string _partnerKey = "a888888888888888";
        private readonly Uri _apiUrl = new Uri("http://61.56.84.139/via6");
        private readonly IWGHorseApiClient _client;

        public WGHorseApiClientUnitTest()
        {
            var client = new HttpClient();
            var faceLog = NullLogger<WGHorseApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            _client = Substitute.For<IWGHorseApiClient>();
            //_client = new WGHorseApiClient(client, faceLog, faceTgsLog);
        }

        [Test]
        public async Task AccessMoneyQueryAsync()
        {
            var data = new WGHorseAccessMoneyQueryRequest()
            {
                OrderId = "5671e95562f34674a4491bfc3e6aefb1",
            };
            _ = _client.AccessMoneyQueryAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).Returns(ValueTask.FromResult<WGHorseResponse>(
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
            var data = new WGHorseAccessMoneyRequest()
            {
                PlayerCode = playerCode,
                AmountType = WGHorseAmountTypes.FundIn,
                OrderId = Guid.NewGuid().ToString("N"),
                Amount = "1"
            };
            _ = _client.AccessMoneyAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).Returns(ValueTask.FromResult<WGHorseResponse>
                (new()
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
            var data = new WGHorseAccessMoneyRequest()
            {
                PlayerCode = playerCode,
                AmountType = WGHorseAmountTypes.FundOut,
                OrderId = Guid.NewGuid().ToString("N"),
                Amount = "1"
            };
            _ = _client.AccessMoneyAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).Returns(ValueTask.FromResult<WGHorseResponse>(
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
            var data = new WGHorseGetBalanceRequest()
            {
                PlayerCode = "4ucmqplxb029"
            };

            _ = await _client.GetBalanceAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).ConfigureAwait(false);
        }

        [Test]
        public async Task GetGameRecordAsync()
        {
            _ = await _client.GetGameRecordAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey
                , _partnerIV,
                new()
                {
                    StartTime = DateTimeOffset.Parse("2021-10-30"),
                    EndTime = DateTimeOffset.Parse("2021-11-01").AddMilliseconds(-1),
                    PageIndex = 1,
                    PageSize = 1000
                }).ConfigureAwait(false);
        }

        [Test]
        public async Task GetLoginAsync()
        {
            var playerCode = "4ucmqplxb029";
            var data = new WGHorsePrepareLoginRequest()
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                PlayerCode = playerCode,
                PlayerNickname = playerCode,
                CurrencyCode = "RMB"
            };
            var token = new WGHorsePrepareLoginResponse()
            {
                Token = Guid.NewGuid().ToString("N")
            };
            var prepareLoginResponse = await _client.GetPrepareLoginAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).ConfigureAwait(false);
            var loginRequest = new WGHorseLoginRequest
            {
                Token = prepareLoginResponse?.Token,
                Lang = ""
            };
            _ = await _client.GetLoginAsync(_apiUrl, _partnerCode, loginRequest).ConfigureAwait(false);
        }

        [Test]
        public async Task GetPrepareLoginAsync()
        {
            var playerCode = "4ucmqplxb029";
            var data = new WGHorsePrepareLoginRequest()
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
