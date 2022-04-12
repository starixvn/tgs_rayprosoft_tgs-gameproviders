using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Skywind;
using Tgs.GameProviders.Core.Skywind.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest
{
    [TestFixture]
    public class SkywindApiClientUnitTest
    {
        private const string Password = "3VSNMGFvbzBJacJT";
        private const string SecretKey = "f841de30-a9eb-49d8-87c7-de878b571afe";
        private const string Username = "PG_merchant_api_stg";
        private readonly Uri _apiUrl = new("https://api.gcpstg.m27613.com/v1");

        [Test]
        public async Task GetAccessTokenAsync()
        {
            var client = Substitute.For<ISkywindApiClient>();
            _ = await client.GetAccessTokenAsync(_apiUrl, new()
            {
                SecretKey = SecretKey,
                Username = Username,
                Password = Password
            });
        }

        [Test]
        public async Task GetGameUrlAsync()
        {
            var client = Substitute.For<ISkywindApiClient>();
            var loginResponse = await client.GetAccessTokenAsync(_apiUrl, new()
            {
                SecretKey = SecretKey,
                Username = Username,
                Password = Password
            });
            var request = new SkywindGameUrlRequest()
            {
                PlayerCode = "4ucmqplxb029",
                GameCode = "sw_fg",
                Ticket = "OZmqtPV4Wz4Q3mQ6haBZ3RBbB60kGMSHAsU3v5wfeYRS9IBQ43fQ17Du2A72vM8T0",
                Language = "zh-cn"
            };
            _ = await client.GetGameUrlAsync(_apiUrl, loginResponse?.AccessToken, request);
        }

        [Test]
        public async Task GetGamHistoryRoundDetailsAsync()
        {
            var client = Substitute.For<ISkywindApiClient>();
            var loginResponse = await client.GetAccessTokenAsync(_apiUrl, new()
            {
                SecretKey = SecretKey,
                Username = Username,
                Password = Password
            });
            var request = new SkywindGamHistoryRoundDetailsRequest()
            {
                RoundId = "OWLAp2LPK",
                Language = "zh-cn"
            };
            _ = await client.GetGamHistoryRoundDetailsAsync(_apiUrl, loginResponse?.AccessToken, request);
        }
    }
}
