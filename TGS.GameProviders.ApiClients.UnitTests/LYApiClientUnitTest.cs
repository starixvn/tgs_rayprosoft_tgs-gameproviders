using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.LY;
using TGS.GameProviders.Core.LY;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class LYApiClientUnitTest
    {
        private const string _agentId = "200995";
        private const string _currencyCode = "CNY";
        private const string _deskey = "706EB65DD6E6DF4C";
        private const string _md5key = "3E00054B89B31B7A";
        private readonly Uri _apiRecordUrl = new Uri("https://wc1-record.wwdlw78.com/getRecordHandle");
        private readonly Uri _apiUrl = new Uri("https://wc1-api.wwdlw78.com/channelHandle");
        private readonly ILYApiClient _client;

        public LYApiClientUnitTest()
        {
            var client = new HttpClient();
            var faceLog = NullLogger<LYApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            _client = Substitute.For<ILYApiClient>();
            //_client = new LYApiClient(client, faceLog, faceTgsLog);
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
            var startTime = DateTimeOffset.Parse("2021-10-28 13:00").ToOffset(TimeSpan.FromHours(+8)).ToUnixTimeMilliseconds();//dateTimeOffset.AddHours(-1).ToUnixTimeMilliseconds(),//1634537632446
            var endTime = DateTimeOffset.Parse("2021-10-28 13:40").ToOffset(TimeSpan.FromHours(+8)).ToUnixTimeMilliseconds();//dateTimeOffset.ToUnixTimeMilliseconds()//1634541232446
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
