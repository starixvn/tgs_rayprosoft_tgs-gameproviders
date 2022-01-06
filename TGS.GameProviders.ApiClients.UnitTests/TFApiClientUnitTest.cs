using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.TFGaming;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class TFApiClientUnitTest
    {
        [Test]
        public async Task GetGameUrlAsync()
        {
            var client = Substitute.For<ITFApiClient>();

            var launchUrl = new Uri("https://tfgaming.getlaunch.com");
            var publicToken = "publicToken";
            var playerCode = "playerCode";

            _ = await client
                .GetGameUrlAsync(launchUrl, publicToken, playerCode)
                .ConfigureAwait(false);
        }

        [Test]
        public async Task KickAsync()
        {
            var client = Substitute.For<ITFApiClient>();

            var apiUrl = new Uri("https://tfgaming.kickplayer.com");
            var privateToken = "privateToken";
            var playerCode = "playerCode";
            var partnerId = "partnerId";

            await client
                .KickAsync(apiUrl, privateToken, partnerId, playerCode)
                .ConfigureAwait(false);
        }
    }
}