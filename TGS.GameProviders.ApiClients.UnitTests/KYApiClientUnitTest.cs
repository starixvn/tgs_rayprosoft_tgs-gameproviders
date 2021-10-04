using System;
using System.Net.Http;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.ApiClients.KY;
using TGS.GameProviders.ApiClients.KY.Models;
using TGS.GameProviders.Domains.SettingConfiguration.KY;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class KYApiClientUnitTest
    {
        private readonly KYAgentMapper _agentMapper;
        private readonly IKYApiClient _client;
        private readonly KYBrandMerchant _merchant;
        private readonly IKYUtility _utility;

        public KYApiClientUnitTest()
        {
            _utility = new KYUtility();
            var client = new HttpClient();
            _client = Substitute.For<IKYApiClient>();
            //_client = new KYApiClient(client, _utility);
            _agentMapper = new KYAgentMapper
            {
                AgentId = "72412",
                CurrencyCode = "CNY",
                Status = true
            };
            _merchant = new KYBrandMerchant
            {
                BrandCode = "Default",
                ApiUrl = new Uri("https://wc1-api.hddv1.com/channelHandle"),
                ApiRecordUrl = new Uri("https://wc1-record.hddv1.com/getRecordHandle"),
                Deskey = "E2F3A416006F84F8",
                Md5key = "B563DF2A3DE58DFB",
                AgentMappers = new KYAgentMapper[] { _agentMapper }
            };
        }

        [Test]
        public async Task FundInAsync()
        {
            var dateTimeOffset = DateTimeOffset.UtcNow.AddHours(+8);
            var playerCode = "4ucmqplxb029";
            var data = new KYFnudRequest()
            {
                PlayerCode = playerCode,
                OrderId = $"{ _agentMapper.AgentId}{dateTimeOffset.ToString("yyyyMMddhhmmssfff")}{playerCode}",
                Amount = 10
            };
            var response = await _client.FundAsync(_merchant, _agentMapper.AgentId, dateTimeOffset, KYAction.FundIn, data).ConfigureAwait(false);
        }

        [Test]
        public async Task FundOutAsync()
        {
            var dateTimeOffset = DateTimeOffset.UtcNow.AddHours(+8);
            var playerCode = "4ucmqplxb029";
            var data = new KYFnudRequest()
            {
                PlayerCode = playerCode,
                OrderId = $"{ _agentMapper.AgentId}{dateTimeOffset.ToString("yyyyMMddhhmmssfff")}{playerCode}",
                Amount = 10
            };
            var response = await _client.FundAsync(_merchant, _agentMapper.AgentId, dateTimeOffset, KYAction.FundOut, data).ConfigureAwait(false);
        }

        [Test]
        public async Task GetBalanceAsync()
        {
            var playerCode = "4ucmqplxb029";
            var data = new KYGetBalanceRequest()
            {
                PlayerCode = playerCode
            };
            var response = await _client.GetBalanceAsync(_merchant, _agentMapper.AgentId, data).ConfigureAwait(false);
        }

        [Test]
        public async Task GetGameRecordAsync()
        {
            var dateTimeOffset = DateTimeOffset.UtcNow.AddHours(+8);
            var data = new KYGameRecordRequest()
            {
                StartTime = DateTimeOffset.UtcNow.AddDays(-30).ToUnixTimeMilliseconds(),
                EndTime = dateTimeOffset.ToUnixTimeMilliseconds()
            };
            var response = await _client.GetGameRecordAsync(_merchant, _agentMapper.AgentId, dateTimeOffset, data).ConfigureAwait(false);
        }

        [Test]
        public async Task GetGameUrlAsync()
        {
            var dateTimeOffset = DateTimeOffset.UtcNow.AddHours(+8);
            var playerCode = "4ucmqplxb029";
            var data = new KYGameUrlRequest()
            {
                PlayerCode = playerCode,
                Amount = 0,
                OrderId = $"{ _agentMapper.AgentId}{dateTimeOffset.ToString("yyyyMMddhhmmssfff")}{playerCode}",
                IP = "220.132.206.70",
                BrandsCode = "GPI",
                ExternalID = 510
            };
            var response = await _client.GetGameUrlAsync(_merchant, _agentMapper.AgentId, dateTimeOffset, data).ConfigureAwait(false);
        }

        [Test]
        public async Task KickAsync()
        {
            var playerCode = "4ucmqplxb029";
            var data = new KYKickRequest()
            {
                PlayerCode = playerCode
            };
            var response = await _client.KickAsync(_merchant, _agentMapper.AgentId, data).ConfigureAwait(false);
        }
    }
}
