using System;
using System.Net.Http;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.ApiClients.WGSports;
using TGS.GameProviders.ApiClients.WGSports.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class WGSportsApiClientUnitTest
    {
        private readonly IWGSportsApiClient _client;
        private readonly WGSportsConfiguration _configuration;
        private readonly IWGSportsUtility _utility;

        public WGSportsApiClientUnitTest()
        {
            _utility = new WGSportsUtility();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("ContentType", "application/x-www-form-urlencoded");
            _client = Substitute.For<IWGSportsApiClient>();
            //_client = new WGSportsApiClient(httpClient, _utility);
            _configuration = new WGSportsConfiguration
            {
                ApiUrl = new Uri("https://api.jsc080.net/"),
                PartnerCode = "TA",
                PartnerKey = "abc1238888888888",
                IV = "cba1238888888888"
            };
        }

        [Test]
        public async Task GetLoginAsync()
        {
            var playerCode = "4ucmqplxb029";
            var data = new WGSportsPrepareLoginRequest()
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                PlayerCode = playerCode,
                PlayerNickname = playerCode,
                CurrencyCode = "RMB"
            };
            var token = new WGSportsPrepareLoginResponse()
            {
                Token = Guid.NewGuid().ToString("N")
            };
            _ = _client.GetPrepareLoginAsync(_configuration, data).Returns(ValueTask.FromResult(token));
            var prepareLoginResponse = await _client.GetPrepareLoginAsync(_configuration, data).ConfigureAwait(false);
            var loginRequest = new WGSportsLoginRequest
            {
                Token = prepareLoginResponse.Token,
                Lang = ""
            };
            var response = await _client.GetLoginAsync(_configuration, loginRequest).ConfigureAwait(false);
        }

        [Test]
        public async Task GetPrepareLoginAsync()
        {
            var playerCode = "4ucmqplxb029";
            var data = new WGSportsPrepareLoginRequest()
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                PlayerCode = playerCode,
                PlayerNickname = playerCode,
                CurrencyCode = "RMB"
            };
            var response = await _client.GetPrepareLoginAsync(_configuration, data).ConfigureAwait(false);
        }
    }
}
