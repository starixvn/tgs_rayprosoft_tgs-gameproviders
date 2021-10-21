using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.VR;
using TGS.GameProviders.ApiClients.VR.Models;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class VRApiClientUnitTest
    {
        private const string _deskey = "PLFNNVZ68DF4V88XZ8B0688660Z2LBP2";
        private const string _merchantid = "WGWG";
        private const string _version = "1.0";
        private readonly Uri _apiUrl = new("https://pmws.vrbetapi.com/");
        private readonly IVRApiClient _client;
        private readonly IVRUtility _utility;

        public VRApiClientUnitTest()
        {
            _utility = new VRUtility();
            var client = new HttpClient();
            var faceLog = NullLogger<VRApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            _client = Substitute.For<IVRApiClient>();
            //_client = new VRApiClient(client, faceLog, faceTgsLog, _utility);
        }

        /// <summary>
        /// Funds the in asynchronous.(GP->TGS)
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task FundInAsync()
        {
            var data = new VRFnudRequest
            {
                SerialNumber = Guid.NewGuid().ToString("N"),
                Type = 1,
                PlayerCode = "4ucmqplxb029",
                Amount = 1,
                CreateTime = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };
            _ = await _client.FundAsync(_apiUrl, _merchantid, _version, _deskey, VRAction.FundIn, data).ConfigureAwait(false);
        }

        /// <summary>
        /// Funds the out asynchronous.(TGS->GP)
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task FundOutAsync()
        {
            var data = new VRFnudRequest
            {
                SerialNumber = Guid.NewGuid().ToString("N"),
                Type = 0,
                PlayerCode = "4ucmqplxb029",
                Amount = 1,
                CreateTime = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };
            _ = await _client.FundAsync(_apiUrl, _merchantid, _version, _deskey, VRAction.FundOut, data).ConfigureAwait(false);
        }

        [Test]
        public async Task GetBalanceAsync()
        {
            var data = new VRGetBalanceRequest
            {
                PlayerCode = "4ucmqplxb029"
            };
            _ = await _client.GetBalanceAsync(_apiUrl, _merchantid, _version, _deskey, data).ConfigureAwait(false);
        }

        [Test]
        public async Task GetGameRecordAsync()
        {
            var date = DateTimeOffset.UtcNow;

            var data = new VRGameRecordRequest
            {
                StartTime = date.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                EndTime = date.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                IsUpdateTime = true,
                Page = 0,
                Rows = 500
            };
            _ = await _client.GetGameRecordAsync(_apiUrl, _merchantid, _version, _deskey, data).ConfigureAwait(false);
        }

        [Test]
        public void GetGameURL()
        {
            var data = new VRGameUrlRequest()
            {
                PlayerCode = "4ucmqplxb029",
                LoginTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                IsCreateUser = "true",
                ChannelId = 0,
                ChannelType = 0
            };
            _ = _client.GetGameURL(_apiUrl, _merchantid, _version, _deskey, data);
        }

        [Test]
        public async Task KickAsync()
        {
            var data = new VRKickRequest
            {
                PlayerCode = "4ucmqplxb029"
            };
            _ = await _client.KickAsync(_apiUrl, _merchantid, _version, _deskey, data).ConfigureAwait(false);
        }
    }
}
