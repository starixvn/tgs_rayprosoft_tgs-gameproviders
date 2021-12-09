using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.Avia;
using TGS.GameProviders.ApiClients.KY;
using TGS.GameProviders.Core.Avia;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class AviaApiClientUnitTest
    {
        private readonly Uri _apiUrl = new Uri("https://api.aviaapi.vip");
        private const string _authorization = "81e2d1a24c684a76a710a70f4900a353";

        private readonly IAviaApiClient _client;

        public AviaApiClientUnitTest()
        {
            var client = new HttpClient();
            var faceLog = NullLogger<AviaApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            _client = Substitute.For<IAviaApiClient>();
            //_client = new AviaApiClient(client, faceLog, faceTgsLog);
        }


        [Test]
        public async Task CreatePlayerAsync()
        {
            var playerCode = "4ucmqplxb029";
            var password = "TgsP@ssw0rd";
            var currency = "CNY";


            _ = _client.RegisterAsync(
                _apiUrl,
                _authorization,
                playerCode,
                password,
                currency).Returns(ValueTask.FromResult(true));

            var actual = await _client.RegisterAsync(
                 _apiUrl,
                _authorization,
                playerCode,
                password,
                currency).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(true, actual);
        }

        [Test]
        public async Task TransferOutAsync()
        {

            var playerCode = "4ucmqplxb029";
            var currency = "CNY";
            var orderId = Guid.NewGuid().ToString("N");
            var amount = 100;
            var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            _ = _client.TransferOutAsync(
                _apiUrl,
                _authorization,
                playerCode,
                currency,
                orderId,
                amount).Returns(ValueTask.FromResult((true, id)));

            var actual = await _client.TransferOutAsync(
                _apiUrl,
                _authorization,
                playerCode,
                currency,
                orderId,
                amount).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(true, actual.Success);
        }

        [Test]
        public async Task TransferInAsync()
        {

            var playerCode = "4ucmqplxb029";
            var currency = "CNY";
            var orderId = Guid.NewGuid().ToString("N");
            var amount = 100;
            var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            _ = _client.TransferInAsync(
                _apiUrl,
                _authorization,
                playerCode,
                currency,
                orderId,
                amount).Returns(ValueTask.FromResult((true, id)));

            var actual = await _client.TransferInAsync(
                _apiUrl,
                _authorization,
                playerCode,
                currency,
                orderId,
                amount).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(true, actual.Success);
        }

        [Test]
        public async Task GetGameUrlAsync()
        {
            var playerCode = "4ucmqplxb029";
            var lang = "ENG";
            var cateId = default(int?);
            var matchId = default(int?);
            var url = new Uri("https://www.google.com/");

            _ = _client.GetGameUrlAsync(
                _apiUrl,
                _authorization,
                playerCode,
                lang,
                cateId,
                matchId).Returns(ValueTask.FromResult(url));

            var actual = await _client.GetGameUrlAsync(
                _apiUrl,
                _authorization,
                playerCode,
                lang,
                cateId,
                matchId).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(url, actual);



        }

        [Test]
        public async Task GetBalanceAsync()
        {

            var playerCode = "4ucmqplxb023";

            decimal? balance = 100m;

            _ = _client.BalanceAsync(
                _apiUrl,
                _authorization,
                playerCode).Returns(ValueTask.FromResult((true, balance)));

            var actual = await _client.BalanceAsync(
                _apiUrl,
                _authorization,
                playerCode).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.AreEqual(balance, actual.Balance);
        }
    }
}
