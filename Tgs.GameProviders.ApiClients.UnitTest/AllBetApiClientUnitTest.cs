using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.AllBet;
using Tgs.GameProviders.Core.AllBet.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest;

[TestFixture]
public class AllBetApiClientUnitTest
{
    private const string AgentId = "ygpg1a";
    private const string ApiKey = "VO5ajdrJrayBQwaEjlMMVmYsME1n38q8Mo5COHKLdymAxP0TQ8Rmq2KdWwly6SlOPKbz4GBM0EDtnE+c6DtVrA==";
    private const string PropertyId = "4222910";
    private const string Suffix = "mkb";
    private readonly Uri _apiUrl = new("https://sw2.apidemo.net:8443");

    [Test]
    public async Task CheckOrCreateMemberAsync()
    {
        var playerCode = "4udvxng80u8y";

        var request = new AllBetCheckOrCreateMemberRequest
        {
            AgentId = AgentId,
            PlayerId = ToAllBetPlayerId(playerCode)
        };

        var client = Substitute.For<IAllBetApiClient>();

        _ = await client.CheckOrCreateMemberAsync(_apiUrl, PropertyId, ApiKey, request).ConfigureAwait(false);
    }

    [Test]
    public async Task LoginAsync()
    {
        var playerCode = "4udvxng80u8y";

        var request = new AllBetLoginRequest
        {
            PlayerId = ToAllBetPlayerId(playerCode),
            Language = "en"
        };

        var client = Substitute.For<IAllBetApiClient>();

        _ = await client.LoginAsync(_apiUrl, PropertyId, ApiKey, request).ConfigureAwait(false);
    }

    [Test]
    public async Task LogoutAsync()
    {
        var playerCode = "4udvxng80u8y";

        var request = new AllBetLogoutRequest
        {
            PlayerId = ToAllBetPlayerId(playerCode)
        };

        var client = Substitute.For<IAllBetApiClient>();

        _ = await client.LogoutAsync(_apiUrl, PropertyId, ApiKey, request).ConfigureAwait(false);
    }

    private static string ToAllBetPlayerId(string playerCode)
        => $"{playerCode}_{Suffix}";
}
