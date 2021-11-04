using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.WGHorse;
using TGS.GameProviders.Core.WGHorse;
using TGS.GameProviders.Core.WGHorse.Models;
using TGS.GameProviders.Core.WGHorse.Models.Commons;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class WGHorseApiClientUnitTest
    {
        private const string _partnerCode = "PT";
        private const string _partnerIV = "b888888888888888";
        private const string _partnerKey = "a888888888888888";
        private readonly Uri _apiUrl = new Uri("http://61.56.84.139/via6");
        private readonly IWGHorseApiClient _client;
        private readonly IWGHorseUtility _utility;

        public WGHorseApiClientUnitTest()
        {
            _utility = new WGHorseUtility();
            var client = new HttpClient();
            var faceLog = NullLogger<WGHorseApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            //_client = Substitute.For<IWGHorseApiClient>();
            _client = new WGHorseApiClient(client, faceLog, faceTgsLog, _utility);
        }

        [Test]
        public async Task AccessMoneyQueryAsync()
        {
            var data = new WGHorseAccessMoneyQueryRequest()
            {
                OrderId = "5671e95562f34674a4491bfc3e6aefb1",
            };
            _ = _client.AccessMoneyQueryAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).Returns(ValueTask.FromResult<WGHorseResponse>(
                new()
                {
                    ReturnCode = 1
                }));
            var result = await _client.AccessMoneyQueryAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).ConfigureAwait(false);
            Assert.AreEqual(1, result.ReturnCode);
        }

        [Test]
        public async Task FundInAsync()
        {
            var playerCode = "4ucmqplxb029";
            var data = new WGHorseAccessMoneyRequest()
            {
                PlayerCode = playerCode,
                AmountType = WGHorseAmountTypes.FundIn,
                OrderId = Guid.NewGuid().ToString("N"),
                Amount = "1"
            };
            _ = _client.AccessMoneyAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).Returns(ValueTask.FromResult<WGHorseResponse>
                (new()
                {
                    ReturnCode = 1
                }));
            var result = await _client.AccessMoneyAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).ConfigureAwait(false);
            Assert.AreEqual(1, result.ReturnCode);
        }

        [Test]
        public async Task FundOutAsync()
        {
            var playerCode = "4ucmqplxb029";
            var data = new WGHorseAccessMoneyRequest()
            {
                PlayerCode = playerCode,
                AmountType = WGHorseAmountTypes.FundOut,
                OrderId = Guid.NewGuid().ToString("N"),
                Amount = "1"
            };
            _ = _client.AccessMoneyAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).Returns(ValueTask.FromResult<WGHorseResponse>(
                new()
                {
                    ReturnCode = 1
                }));
            var result = await _client.AccessMoneyAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).ConfigureAwait(false);
            Assert.AreEqual(1, result.ReturnCode);
        }

        [Test]
        public async Task GetBalanceAsync()
        {
            var data = new WGHorseGetBalanceRequest()
            {
                PlayerCode = "4ucmqplxb029"
            };

            _ = await _client.GetBalanceAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).ConfigureAwait(false);
        }

        [Test]
        public async Task GetLoginAsync()
        {
            var playerCode = "4ucmqplxb029";
            var data = new WGHorsePrepareLoginRequest()
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                PlayerCode = playerCode,
                PlayerNickname = playerCode,
                CurrencyCode = "RMB"
            };
            var token = new WGHorsePrepareLoginResponse()
            {
                Token = Guid.NewGuid().ToString("N")
            };
            var prepareLoginResponse = await _client.GetPrepareLoginAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).ConfigureAwait(false);
            var loginRequest = new WGHorseLoginRequest
            {
                Token = prepareLoginResponse?.Token,
                Lang = ""
            };
            _ = await _client.GetLoginAsync(_apiUrl, _partnerCode, loginRequest).ConfigureAwait(false);
        }

        [Test]
        public async Task GetPrepareLoginAsync()
        {
            var playerCode = "4ucmqplxb029";
            var data = new WGHorsePrepareLoginRequest()
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                PlayerCode = playerCode,
                PlayerNickname = playerCode,
                CurrencyCode = "RMB"
            };
            _ = await _client.GetPrepareLoginAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, data).ConfigureAwait(false);
        }

        [Test]
        public async Task LogoutAsync()
        {
            _ = await _client.LogoutAsync(_apiUrl, _partnerCode, _partnerKey, _partnerIV, "4ucmqplxb029").ConfigureAwait(false);
        }

        [Test]
        public void GP_to_TGS_FundIn_0()
        {
            var actual = _utility.AesDecrypt("ll%2fZKU%2bhK5pBYSPzebhYZqWVp4Ag6xx25mbVfi3toMuRGlXpXUgu6JhycvLLPovG9gOEj4bJCPgvkoBL5TrM%2fg%3d%3d", _partnerKey, _partnerIV);
            var expected = "4uerjhy1ls17|2|b0e85574deb049b4dcd008d97e57650f4uerjhy1ls17|0";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GP_to_TGS_FundIn_5()
        {
            var actual = _utility.AesDecrypt("9qDnhM1ib1RhRDjbKt1qpYf9Z8Cv%2b%2b1HZmv1PM3nGekgkfhm8Q4BUnc%2fhvhFmTPwR%2fNjj9Q8fsT0UQHSA9L0pA%3d%3d", _partnerKey, _partnerIV);
            var expected = "4ueqmkkmbo6z|1|9f119274674b4b7eddd308d97e57650f4ueqmkkmbo6z|5";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GP_to_TGS_FundIn_53()
        {
            var actual = _utility.AesDecrypt("S3u0bxkN6q3DLPjywjH4j2Nc8lgPFwZVxBgfgPcq3dls97UaAVZxgXiX3V98XrF5nT5%2bS%2bhfnnYCT9JXIgxT3Q%3d%3d", _partnerKey, _partnerIV);
            var expected = "4uerjhy1ls17|2|3f8d88d48189429fdc2908d97e57650f4uerjhy1ls17|53";
            Assert.AreEqual(expected, actual);
        }
    }
}
