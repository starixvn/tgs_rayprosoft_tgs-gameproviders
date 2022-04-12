using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.WGHorse;
using Tgs.GameProviders.Core.WGHorse.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest
{
    [TestFixture]
    public class WGHorseApiClientUnitTest
    {
        private const string PartnerCode = "PT";
        private const string PartnerIV = "b888888888888888";
        private const string PartnerKey = "a888888888888888";
        private readonly Uri _apiUrl = new("http://61.56.84.139/via6");

        [Test]
        public async Task AccessMoneyQueryAsync()
        {
            var orderId = "5671e95562f34674a4491bfc3e6aefb1";

            var client = Substitute.For<IWGHorseApiClient>();

            _ = client.AccessMoneyQueryAsync(_apiUrl, PartnerCode, PartnerKey, PartnerIV, orderId).Returns(ValueTask.FromResult(true));
            var actual = await client.AccessMoneyQueryAsync(_apiUrl, PartnerCode, PartnerKey, PartnerIV, orderId).ConfigureAwait(false);
            Assert.NotNull(actual);
            Assert.AreEqual(true, actual);
        }

        [Test]
        public async Task FundInAsync()
        {
            var playerCode = "4ucmqplxb029";
            var orderId = Guid.NewGuid().ToString("N");
            var amount = 1m;

            var client = Substitute.For<IWGHorseApiClient>();

            _ = client.AccessMoneyFundInAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
                playerCode,
                orderId,
                amount).Returns(ValueTask.FromResult(true));

            var actual = await client.AccessMoneyFundInAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
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

            var client = Substitute.For<IWGHorseApiClient>();

            _ = client.AccessMoneyFundOutAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
                playerCode,
                orderId,
                amount).Returns(ValueTask.FromResult(true));

            var actual = await client.AccessMoneyFundOutAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
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

            var client = Substitute.For<IWGHorseApiClient>();

            _ = client.GetBalanceAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
                playerCode).Returns(ValueTask.FromResult((IsPlayer, Balance)));

            var actual = await client.GetBalanceAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
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

            var client = Substitute.For<IWGHorseApiClient>();

            _ = client.GetGameRecordAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
                startTime,
                endTime,
                pageIndex,
                pageSize).Returns(ValueTask.FromResult((Success, GameRecord)));

            var actual = await client.GetGameRecordAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
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

            var client = Substitute.For<IWGHorseApiClient>();

            _ = client.GetPrepareLoginAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
                timestamp,
                playerCode,
                playerCode,
                currencyCode).Returns(ValueTask.FromResult<string?>(token));

            _ = client.GetLoginAsync(
                _apiUrl,
                PartnerCode,
                token,
                lang).Returns(ValueTask.FromResult<Uri?>(gameUrl));

            var actual = await client.GetLoginAsync(
                _apiUrl,
                PartnerCode,
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

            var client = Substitute.For<IWGHorseApiClient>();

            _ = client.GetPrepareLoginAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
                timestamp,
                playerCode,
                playerCode,
                currencyCode).Returns(ValueTask.FromResult<string?>(token));

            var actual = await client.GetPrepareLoginAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
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

            var client = Substitute.For<IWGHorseApiClient>();

            await client.LogoutAsync(
               _apiUrl,
               PartnerCode,
               PartnerKey,
               PartnerIV,
               playerCode).ConfigureAwait(false);
        }
    }
}
