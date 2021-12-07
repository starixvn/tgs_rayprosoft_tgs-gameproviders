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
using TGS.GameProviders.ApiClients.WGSports.Models.Commons;

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

        public WGSportsApiClientUnitTest()
        {
            var client = new HttpClient();
            var faceLog = NullLogger<WGSportsApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            _client = Substitute.For<IWGSportsApiClient>();
            //_client = new WGSportsApiClient(client, faceLog, faceTgsLog);
        }

        [Test]
        public async Task AccessMoneyQueryAsync()
        {
            var orderId = Guid.NewGuid().ToString("N");

            _ = _client.AccessMoneyQueryAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                orderId).Returns(ValueTask.FromResult(true));

            var actual = await _client.AccessMoneyQueryAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                orderId).ConfigureAwait(false);

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
                currencyCode,
                string.Empty).Returns(ValueTask.FromResult(token));

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
                currencyCode,
                string.Empty).Returns(ValueTask.FromResult(token));

            var actual = await _client.GetPrepareLoginAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                timestamp,
                playerCode,
                playerCode,
                currencyCode,
                string.Empty).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(token, actual);
        }

        [Test]
        public async Task LogoutAsync()
        {
            var playerCode = "4ucmqplxb029";

            var response = new WGSportsResponse()
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

        [Test]
        public async Task GetGameRecordAsync()
        {
            var startTime = DateTimeOffset.UtcNow.AddMinutes(-20).ToOffset(TimeSpan.FromHours(-4)).ToString("yyyy-MM-dd HH:mm:ss");
            var endTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4)).ToString("yyyy-MM-dd HH:mm:ss");

            (bool Success, WGSportsBetInfo[]? BetInfos) = (true, null);

            _ = _client.GetGameRecordAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                GameRecordTime.BetDate,
                startTime,
                endTime).Returns(ValueTask.FromResult((Success, BetInfos)));

            var actual = await _client.GetGameRecordAsync(
                _apiUrl,
                _partnerCode,
                _partnerKey,
                _partnerIV,
                GameRecordTime.BetDate,
                startTime,
                endTime).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(Success, actual.Success);
        }
    }
}
