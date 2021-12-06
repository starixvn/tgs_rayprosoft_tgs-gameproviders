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
            var orderId = "5671e95562f34674a4491bfc3e6aefb1";

            _ = _client.AccessMoneyQueryAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, orderId).Returns(ValueTask.FromResult(true));
            var actual = await _client.AccessMoneyQueryAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, orderId).ConfigureAwait(false);
            Assert.NotNull(actual);
            Assert.AreEqual(true, actual);
        }

        [Test]
        public async Task FundInAsync()
        {
            var playerCode = "4ucmqplxb029";
            var orderId = Guid.NewGuid().ToString("N");
            var amount = 1m;

            _ = _client.AccessMoneyFundInAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                playerCode,
                orderId,
                amount).Returns(ValueTask.FromResult(true));

            var actual = await _client.AccessMoneyFundInAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                playerCode,
                orderId,
                amount).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(true, actual);
        }

        [Test]
        public async Task FundOutAsync()
        {
            var playerCode = "4ucmqplxb029";
            var orderId = Guid.NewGuid().ToString("N");
            var amount = 1m;

            _ = _client.AccessMoneyFundOutAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                playerCode,
                orderId,
                amount).Returns(ValueTask.FromResult(true));

            var actual = await _client.AccessMoneyFundOutAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                playerCode,
                orderId,
                amount).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(true, actual);
        }

        [Test]
        public async Task GetBalanceAsync()
        {
            var playerCode = "4ucmqplxb029";
            (bool IsPlayer, decimal? Balance) = (true, 1);

            _ = _client.GetBalanceAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                playerCode).Returns(ValueTask.FromResult((IsPlayer, Balance)));

            var actual = await _client.GetBalanceAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                playerCode).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(IsPlayer, actual.IsPlayer);
            Assert.AreEqual(Balance, actual.Balance);
        }

        [Test]
        public async Task GetGameRecordAsync()
        {
            var startTime = DateTimeOffset.Parse("2021-10-30");
            var endTime = DateTimeOffset.Parse("2021-11-01").AddMilliseconds(-1);
            int pageIndex = 1;
            int pageSize = 1000;
            (bool Success, WGHorseGameRecord[]? GameRecord) = (true, null);

            _ = _client.GetGameRecordAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                startTime,
                endTime,
                pageIndex,
                pageSize).Returns(ValueTask.FromResult((Success, GameRecord)));

            var actual = await _client.GetGameRecordAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                startTime,
                endTime,
                pageIndex,
                pageSize).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(Success, actual.Success);
        }

        [Test]
        public async Task GetLoginAsync()
        {
            var playerCode = "4ucmqplxb029";
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var currencyCode = "RMB";
            var lang = "0";
            var token = Guid.NewGuid().ToString();
            var gameUrl = new Uri("https://www.google.com/");

            _ = _client.GetPrepareLoginAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                timestamp,
                playerCode,
                playerCode,
                currencyCode).Returns(ValueTask.FromResult(token));

            _ = _client.GetLoginAsync(
                _apiUrl,
                _partnerCode,
                token,
                lang).Returns(ValueTask.FromResult(gameUrl));

            var actual = await _client.GetLoginAsync(
                _apiUrl,
                _partnerCode,
                token,
                lang).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(gameUrl, actual);
        }

        [Test]
        public async Task GetPrepareLoginAsync()
        {
            var playerCode = "4ucmqplxb029";
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var currencyCode = "RMB";
            var token = Guid.NewGuid().ToString();

            _ = _client.GetPrepareLoginAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                timestamp,
                playerCode,
                playerCode,
                currencyCode).Returns(ValueTask.FromResult(token));

            var actual = await _client.GetPrepareLoginAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                timestamp,
                playerCode,
                playerCode,
                currencyCode).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(token, actual);
        }

        [Test]
        public async Task LogoutAsync()
        {
            var playerCode = "4ucmqplxb029";

            var response = new WGHorseResponse()
            {
                ReturnCode = 1
            };

            _ = _client.LogoutAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                playerCode);

            await _client.LogoutAsync(
               _apiUrl,
               _partnerCode,
               _partnerKey,
               _partnerIV,
               playerCode).ConfigureAwait(false);
        }
    }
}
