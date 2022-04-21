using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.IMESports;
using Tgs.GameProviders.Core.IMESports.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest
{
    [TestFixture]
    public class IMESportsApiClientUnitTest
    {
        private const string MerchantCode = "Ogt3HBRFebTqJczcN28CliP3BHqDWIYo";
        private const string Prefix = "IM1CP";
        private const int ProductWallet = 401;
        private readonly Uri _apiUrl = new("http://operatorapi.staging.imaegisapi.com");

        [Test]
        public async Task GetDesktopGameGameUrlAsync()
        {
            var playerCode = "4ucmqplxb029";
            var playerId = ToIMESportsPlayerId(playerCode);

            var request = new IMESportsLaunchGameRequest
            {
                MerchantCode = MerchantCode,
                IMESPlayerId = playerId,
                GameId = "ESPORTSBULL",
                Language = "EN",
                Http = "1",
                ProductWallet = ProductWallet
            };

            var client = Substitute.For<IIMESportsApiClient>();

            _ = await client.LaunchDesktopGameAsync(_apiUrl, request).ConfigureAwait(false);
        }

        [Test]
        public async Task GetMobileGameGameUrlAsync()
        {
            var playerCode = "4ucmqplxb029";
            var playerId = ToIMESportsPlayerId(playerCode);

            var request = new IMESportsLaunchGameRequest
            {
                MerchantCode = MerchantCode,
                IMESPlayerId = playerId,
                GameId = "ESPORTSBULL",
                Language = "EN",
                Http = "1",
                ProductWallet = ProductWallet
            };

            var client = Substitute.For<IIMESportsApiClient>();

            _ = await client.LaunchMobileGameAsync(_apiUrl, request).ConfigureAwait(false);
        }

        [Test]
        public async Task GetRoundHistoryAsync()
        {
            var request = new IMESportsGetRoundHistoryRequest
            {
                MerchantCode = MerchantCode,
                ProductWallet = ProductWallet,
                StartDate = "2021-11-26 18.46.44",
                EndDate = "2021-11-26 18.49.44",
                Page = 1,
                Language = "EN"
            };

            var client = Substitute.For<IIMESportsApiClient>();

            _ = await client.GetRoundHistoryAsync(_apiUrl, request);
        }

        [Test]
        public async Task RegisterPlayerAsync()
        {
            var playerCode = "4ucmqplxb029";
            var playerId = ToIMESportsPlayerId(playerCode);

            var request = new IMESportsRegisterPlayerRequest
            {
                MerchantCode = MerchantCode,
                IMESPlayerId = playerId,
                CurrencyCode = "CNY"
            };

            var client = Substitute.For<IIMESportsApiClient>();

            _ = await client.RegisterPlayerAsync(_apiUrl, request)
                .ConfigureAwait(false);
        }

        private static string ToIMESportsPlayerId(string playerCode)
            => $"{Prefix}_{playerCode}";
    }
}
