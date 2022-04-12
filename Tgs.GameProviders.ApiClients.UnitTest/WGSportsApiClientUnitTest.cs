using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.WGSports;
using Tgs.GameProviders.Core.WGSports.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest
{
    [TestFixture]
    public class WGSportsApiClientUnitTest
    {
        private const string PartnerCode = "TA";
        private const string PartnerIV = "cba1238888888888";
        private const string PartnerKey = "abc1238888888888";
        private readonly Uri _apiUrl = new("https://api.jsc080.net/");

        [Test]
        public async Task AccessMoneyQueryAsync()
        {
            var orderId = Guid.NewGuid().ToString("N");

            var client = Substitute.For<IWGSportsApiClient>();

            _ = client.AccessMoneyQueryAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
                orderId).Returns(ValueTask.FromResult(true));

            var actual = await client.AccessMoneyQueryAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
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

            var client = Substitute.For<IWGSportsApiClient>();

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

            var client = Substitute.For<IWGSportsApiClient>();

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

            var client = Substitute.For<IWGSportsApiClient>();

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
            var startTime = DateTimeOffset.UtcNow.AddMinutes(-20).ToOffset(TimeSpan.FromHours(-4)).ToString("yyyy-MM-dd HH:mm:ss");
            var endTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4)).ToString("yyyy-MM-dd HH:mm:ss");

            (bool Success, WGSportsBetInfo[]? BetInfos) = (true, null);

            var client = Substitute.For<IWGSportsApiClient>();

            _ = client.GetGameRecordAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
                GameRecordTime.BetDate,
                startTime,
                endTime).Returns(ValueTask.FromResult((Success, BetInfos)));

            var actual = await client.GetGameRecordAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
                GameRecordTime.BetDate,
                startTime,
                endTime).ConfigureAwait(false);

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

            var client = Substitute.For<IWGSportsApiClient>();

            _ = client.GetPrepareLoginAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
                timestamp,
                playerCode,
                playerCode,
                currencyCode,
                string.Empty).Returns(ValueTask.FromResult<string?>(token));

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

            var client = Substitute.For<IWGSportsApiClient>();

            _ = client.GetPrepareLoginAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
                timestamp,
                playerCode,
                playerCode,
                currencyCode,
                string.Empty).Returns(ValueTask.FromResult<string?>(token));

            var actual = await client.GetPrepareLoginAsync(
                _apiUrl,
                PartnerCode,
                PartnerKey,
                PartnerIV,
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

            var client = Substitute.For<IWGSportsApiClient>();

            await client.LogoutAsync(
               _apiUrl,
               PartnerCode,
               PartnerKey,
               PartnerIV,
               playerCode).ConfigureAwait(false);
        }
    }
}
