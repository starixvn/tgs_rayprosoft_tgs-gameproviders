using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameProviders.Core.TFGaming;

namespace Tgs.GameProviders.ApiClients.UnitTest;

public class TFApiClientUnitTest
{
    [Fact]
    public async Task GetGameUrlAsync()
    {
        var client = Substitute.For<ITFApiClient>();

        var launchUrl = new Uri("https://tfgaming.getlaunch.com");
        var publicToken = "publicToken";
        var playerCode = "playerCode";

        _ = await client.GetGameUrlAsync(launchUrl, publicToken, playerCode);
    }

    [Fact]
    public async Task KickAsync()
    {
        var client = Substitute.For<ITFApiClient>();

        var apiUrl = new Uri("https://tfgaming.kickplayer.com");
        var privateToken = "privateToken";
        var playerCode = "playerCode";
        var partnerId = "partnerId";

        await client.KickAsync(apiUrl, privateToken, partnerId, playerCode);
    }
}
