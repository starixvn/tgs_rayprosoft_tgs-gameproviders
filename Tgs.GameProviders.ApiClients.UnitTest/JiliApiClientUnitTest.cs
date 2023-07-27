using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameProviders.Core.Jili;
using Tgs.GameProviders.Core.Jili.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest;

public class JiliApiClientUnitTest
{
    private const string AgentId = "PG_Seamless";
    private const string AgentKey = "0d90dda822b6df548460da8f551c1a76c01b6a9e";
    private readonly Uri _apiUrl = new("https://uat-wb-api.jlfafafa2.com");

    [Fact]
    public async Task GetGameDetailUrlAsync()
    {
        var language = "en-US";
        var externalRoundId = "1639679431950180036";

        var client = Substitute.For<IJiliApiClient>();

        _ = await client
            .GetGameDetailUrlAsync(_apiUrl, AgentId, AgentKey, language, externalRoundId)
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task KickMemberAsync()
    {
        var jiliPlayerId = "GPI_4ucmqplxb029";

        var client = Substitute.For<IJiliApiClient>();

        _ = await client
            .KickMemberAsync(_apiUrl, jiliPlayerId, AgentId, AgentKey)
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task LoginWithoutRedirectAsync()
    {
        var request = new JiliLoginWithoutRedirectRequest
        {
            GameExternalId = "10",
            Token = "token",
            Language = "en-US"
        };

        var client = Substitute.For<IJiliApiClient>();

        _ = await client
            .LoginWithoutRedirectAsync(_apiUrl, AgentId, AgentKey, request)
            .ConfigureAwait(false);
    }
}
