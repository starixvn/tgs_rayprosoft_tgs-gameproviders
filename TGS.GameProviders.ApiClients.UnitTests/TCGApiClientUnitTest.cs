using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.Skywind.Models;
using TGS.GameProviders.ApiClients.TCG;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.TCG;
using TGS.Shared.Game.Models.TCG.Outgoing;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class TCGApiClientUnitTest
    {
        private readonly ITCGApiClient _client;
        private readonly TCGConfiguration _configuration;
        private readonly TCGMerchant _merchant;
        private readonly TCGMerchant _merchantVND;
        private readonly ITCGUtility _utility;

        public TCGApiClientUnitTest()
        {
            _utility = new TCGUtility();
            var client = new HttpClient();
            var faceLog = NullLogger<TCGApiClient>.Instance;
            var faceTgsLog = Substitute.For<ITgsLog>();
            _client = Substitute.For<ITCGApiClient>();
            //_client = new TCGApiClient(client, faceLog, faceTgsLog, _utility);

            _merchant = new TCGMerchant
            {
                GameUrl = new("https://tcg-cny.srvttt.com"),
                CurrencyCode = "CNY",
                MerchantCode = "pegacnys",
                DESKey = "EuWEfLac",
                SHA256Key = "h2f69FzdSmMZzrdE",
                PlayerPassword = "TgsPassw0rd"
            };

            _merchantVND = new TCGMerchant()
            {
                GameUrl = new("https://tcg-vnd.srvttt.com"),
                CurrencyCode = "VND",
                MerchantCode = "pegavnds",
                DESKey = "AiynBxjI",
                SHA256Key = "NH5v1N9XlCnbQjj1",
                PlayerPassword = "TgsPassw0rd"
            };
            _configuration = new TCGConfiguration()
            {
                ServiceEndpointUrl = new("http://game-tcg"),
                ApiUrl = new("http://www.connect6play.com/doBusiness.do"),
                Merchants = new TCGMerchant[] { _merchant, _merchantVND }
            };
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
            _ = await _client.LaunchGameAsync(_configuration.ApiUrl, _merchant, data).ConfigureAwait(false);
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
            _ = await _client.LaunchGameAsync(_configuration.ApiUrl, _merchantVND, data).ConfigureAwait(false);
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
            _ = await _client.LaunchGameAsync(_configuration.ApiUrl, _merchant, data).ConfigureAwait(false);
        }

        [Test]
        public async Task KickOutLottoPlayerAsync()
        {
            var data = new TCGKickOutLottoPlayerRequest()
            {
                UserName = "4ucmqplxb029"
            };
            _ = await _client.KickOutLottoPlayerAsync(_configuration.ApiUrl, _merchant, data).ConfigureAwait(false);
        }
    }
}
