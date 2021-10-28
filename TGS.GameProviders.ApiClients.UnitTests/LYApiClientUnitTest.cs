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
using TGS.GameProviders.ApiClients.LY;
using TGS.GameProviders.ApiClients.LY.Models;

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
        private readonly ILYUtility _utility;

        public LYApiClientUnitTest()
        {
            _utility = new LYUtility();
            var client = new HttpClient();
            var faceLog = NullLogger<LYApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            //_client = Substitute.For<ILYApiClient>();
            _client = new LYApiClient(client, faceLog, faceTgsLog, _utility);
        }

        [Test]
        public async Task FundInAsync()
        {
            var dateTimeOffset = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
            var playerCode = "4ucmqplxb029";
            var data = new LYFnudRequest()
            {
                PlayerCode = playerCode,
                OrderId = $"{ _agentId}{dateTimeOffset.ToString("yyyyMMddhhmmssfff")}{playerCode}",
                Amount = 8
            };
            var response = await _client.FundAsync(_apiUrl, _deskey, _md5key, _agentId, dateTimeOffset, LYAction.FundIn, data).ConfigureAwait(false);
        }

        [Test]
        public async Task FundOutAsync()
        {
            var dateTimeOffset = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
            var playerCode = "4ucmqplxb029";
            var data = new LYFnudRequest()
            {
                PlayerCode = playerCode,
                OrderId = $"{ _agentId}{dateTimeOffset.ToString("yyyyMMddhhmmssfff")}{playerCode}",
                Amount = 10
            };
            var response = await _client.FundAsync(_apiUrl, _deskey, _md5key, _agentId, dateTimeOffset, LYAction.FundOut, data).ConfigureAwait(false);
        }

        [Test]
        public async Task GetBalanceAsync()
        {
            var playerCode = "4ucmqplxb029";
            var data = new LYGetBalanceRequest()
            {
                PlayerCode = playerCode
            };
            var response = await _client.GetBalanceAsync(_apiUrl, _deskey, _md5key, _agentId, data).ConfigureAwait(false);
        }

        [Test]
        public async Task GetGameRecordAsync()
        {
            var dateTimeOffset = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
            var data = new LYGameRecordRequest()
            {
                StartTime = DateTimeOffset.Parse("2021-10-28 13:00").ToOffset(TimeSpan.FromHours(+8)).ToUnixTimeMilliseconds(),//dateTimeOffset.AddHours(-1).ToUnixTimeMilliseconds(),//1634537632446
                EndTime = DateTimeOffset.Parse("2021-10-28 13:40").ToOffset(TimeSpan.FromHours(+8)).ToUnixTimeMilliseconds()//dateTimeOffset.ToUnixTimeMilliseconds()//1634541232446
            };
            var response = await _client.GetGameRecordAsync(_apiRecordUrl, _deskey, _md5key, _agentId, dateTimeOffset, data).ConfigureAwait(false);
        }

        [Test]
        public async Task GetGameUrlAsync()
        {
            var dateTimeOffset = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
            var playerCode = "4ucmqplxb029";
            var data = new LYGameUrlRequest()
            {
                PlayerCode = playerCode,
                Amount = 0,
                OrderId = $"{_agentId}{dateTimeOffset.ToString("yyyyMMddhhmmssfff")}{playerCode}",
                IP = "220.132.206.70",
                BrandsCode = "GPI",
                ExternalID = 510
            };
            var response = await _client.GetGameUrlAsync(_apiUrl, _deskey, _md5key, _agentId, dateTimeOffset, data).ConfigureAwait(false);
        }

        [Test]
        public async Task KickAsync()
        {
            var playerCode = "4ucmqplxb029";
            var data = new LYKickRequest()
            {
                PlayerCode = playerCode
            };
            var response = await _client.KickAsync(_apiUrl, _deskey, _md5key, _agentId, data).ConfigureAwait(false);
        }
    }
}
