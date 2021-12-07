using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.TCG;
using TGS.GameProviders.Core.TCG;
using TGS.GameProviders.Core.TCG.Models;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class TCGApiClientUnitTest
    {
        private const string _currencyCode = "CNY";
        private const string _desKey = "EuWEfLac";
        private const string _merchantCode = "pegacnys";
        private const string _playerPassword = "TgsPassw0rd";
        private const string _sha256Key = "h2f69FzdSmMZzrdE";
        private const string _vnd_currencyCode = "CNY";
        private const string _vnd_desKey = "EuWEfLac";
        private const string _vnd_merchantCode = "pegacnys";
        private const string _vnd_playerPassword = "TgsPassw0rd";
        private const string _vnd_sha256Key = "h2f69FzdSmMZzrdE";
        private readonly Uri _apiUrl = new("http://www.connect6play.com/doBusiness.do");
        private readonly ITCGApiClient _client;
        private readonly Uri _gameUrl = new("https://tcg-cny.srvttt.com");

        public TCGApiClientUnitTest()
        {
            var client = new HttpClient();
            var faceLog = NullLogger<TCGApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            _client = Substitute.For<ITCGApiClient>();
            //_client = new TCGApiClient(client, faceLog, faceTgsLog);
        }

        [Test]
        public async Task GetLaunchGameByELOTTOAsync()
        {
            var data = new TCGLaunchGameByELOTTORequest()
            {
                UserName = "4ucmqplxb029",
                Platform = "html5",
                GameMode = "2",
                GameCode = "Lobby",
                Language = "CN",
                LotteryBetMode = "Elott_Traditional",
                View = "Lobby"
            };
            _ = await _client.LaunchGameAsync(_apiUrl, _merchantCode, _desKey, _sha256Key, data).ConfigureAwait(false);
        }

        [Test]
        public async Task GetLaunchGameByLOTTOVNAsync()
        {
            var data = new TCGLaunchGameByLOTTOVNRequest()
            {
                UserName = "4ucmqplxb029",
                Platform = "WEB",
                GameMode = "420",
                GameCode = "lobby",
                Language = "CN",
                LotteryBetMode = "Elott",
                View = "lobby"
            };
            _ = await _client.LaunchGameAsync(_apiUrl, _vnd_merchantCode, _vnd_desKey, _vnd_sha256Key, data).ConfigureAwait(false);
        }

        [Test]
        public async Task GetLaunchGameBySEAAsync()
        {
            var data = new TCGLaunchGameBySEARequest()
            {
                UserName = "4ucmqplxb029",
                Platform = "html5",
                GameMode = "384",
                GameCode = "lobby",
                Language = "CN",
                LotteryBetMode = "SEA_Tradition",
                View = "Lobby"
            };
            _ = await _client.LaunchGameAsync(_apiUrl, _merchantCode, _desKey, _sha256Key, data).ConfigureAwait(false);
        }

        [Test]
        public async Task KickOutLottoPlayerAsync()
        {
            var data = new TCGKickOutLottoPlayerRequest()
            {
                UserName = "4ucmqplxb029"
            };
            _ = await _client.KickOutLottoPlayerAsync(_apiUrl, _merchantCode, _desKey, _sha256Key, data).ConfigureAwait(false);
        }
    }
}
