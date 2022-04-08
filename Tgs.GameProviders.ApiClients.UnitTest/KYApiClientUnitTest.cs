using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.ApiClients.KY;
using Tgs.GameProviders.Core.KY;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.UnitTest
{
    [TestFixture]
    public class KYApiClientUnitTest
    {
        private const string _agentId = "72412";
        private const string _deskey = "E2F3A416006F84F8";
        private const string _md5key = "B563DF2A3DE58DFB";
        private readonly Uri _apiRecordUrl = new Uri("https://wc1-record.hddv1.com/getRecordHandle");
        private readonly Uri _apiUrl = new Uri("https://wc1-api.hddv1.com/channelHandle");
        private readonly IKYApiClient _client;

        public KYApiClientUnitTest()
        {
            var client = new HttpClient();
            var faceLog = NullLogger<KYApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            _client = Substitute.For<IKYApiClient>();
            //_client = new KYApiClient(client, faceLog, faceTgsLog);
        }

        [Test()]
        [TestCase("72412202110271546265054ucmqplxb029", true)]
        [TestCase("72412202110271546265055ucmqplxb029", false)]
        public async Task FundCheckingAsync(string orderId, bool status)
        {
            _ = _client.FundCheckingAsync(
                _apiUrl,
                _deskey,
                _md5key,
                _agentId,
                orderId).Returns(ValueTask.FromResult<bool>(status));
            var response = await _client.FundCheckingAsync(
                _apiUrl,
                _deskey,
                _md5key,
                _agentId,
                orderId).ConfigureAwait(false);
            Assert.AreEqual(status, response);
        }

        [Test]
        public async Task FundInAsync()
        {
            var playerCode = "4ucmqplxb029";

            _ = await _client.FundInAsync(
                _apiUrl,
                _deskey,
                _md5key,
                _agentId,
                playerCode,
                10).ConfigureAwait(false);
        }

        [Test]
        public async Task FundOutAsync()
        {
            var playerCode = "4ucmqplxb029";

            _ = await _client.FundOutAsync(
                _apiUrl,
                _deskey,
                _md5key,
                _agentId,
                playerCode,
                10).ConfigureAwait(false);
        }

        [Test]
        public async Task GetBalanceAsync()
        {
            var playerCode = "4ucmqplxb029";
            _ = await _client.GetBalanceAsync(
                _apiUrl,
                _deskey,
                _md5key,
                _agentId,
                playerCode).ConfigureAwait(false);
        }

        [Test]
        public async Task GetGameRecordAsync()
        {
            var dateTimeOffset = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
            var startTime = dateTimeOffset.AddHours(-1).ToUnixTimeMilliseconds();//1634537632446
            var endTime = dateTimeOffset.ToUnixTimeMilliseconds();//1634541232446
            _ = await _client.GetGameRecordAsync(
                _apiRecordUrl,
                _deskey,
                _md5key,
                _agentId,
                startTime,
                endTime).ConfigureAwait(false);
        }

        [Test]
        public async Task GetGameUrlAsync()
        {
            var playerCode = "4ucmqplxb029";
            _ = await _client.GetGameUrlAsync(
                _apiUrl,
                _deskey,
                _md5key,
                _agentId,
                playerCode,
                "220.132.206.70",
                "GPI",
                510).ConfigureAwait(false);
        }

        [Test]
        public async Task KickAsync()
        {
            var playerCode = "4ucmqplxb029";
            await _client.KickAsync(
                _apiUrl,
                _deskey,
                _md5key,
                _agentId,
                playerCode).ConfigureAwait(false);
        }
    }
}
