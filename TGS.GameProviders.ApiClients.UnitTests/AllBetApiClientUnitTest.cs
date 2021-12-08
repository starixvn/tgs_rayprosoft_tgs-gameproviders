using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.ApiClients.AllBet;
using TGS.GameProviders.Core.AllBet;
using TGS.GameProviders.Core.AllBet.Models;
using Tgs.Logging;

namespace TGS.GameProviders.ApiClients.UnitTests;

[TestFixture]
public class AllBetApiClientUnitTest
{
    private readonly string _agentId = "ygpg1a";
    private readonly string _apiKey = "VO5ajdrJrayBQwaEjlMMVmYsME1n38q8Mo5COHKLdymAxP0TQ8Rmq2KdWwly6SlOPKbz4GBM0EDtnE+c6DtVrA==";
    private readonly Uri _apiUrl = new Uri("https://sw2.apidemo.net:8443");
    private readonly IAllBetApiClient _client;
    private readonly string _propertyId = "4222910";
    private readonly string _suffix = "mkb";

    public AllBetApiClientUnitTest()
    {
        var httpClient = new HttpClient();
        var logger = NullLogger<AllBetApiClient>.Instance;
        var tgsLog = Substitute.For<ITgsLog>();

        //_client = new AllBetApiClient(logger, httpClient, tgsLog);
        _client = Substitute.For<IAllBetApiClient>();
    }

    [Test]
    public async Task CheckOrCreateMemberAsync()
    {
        var playerCode = "4udvxng80u8y";

        var request = new AllBetCheckOrCreateMemberRequest
        {
            AgentId = _agentId,
            PlayerId = ToAllBetPlayerId(playerCode)
        };

        var response = await _client.CheckOrCreateMemberAsync(_apiUrl, _propertyId, _apiKey, request).ConfigureAwait(false);
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

        var response = await _client.LoginAsync(_apiUrl, _propertyId, _apiKey, request).ConfigureAwait(false);
    }

    [Test]
    public async Task LogoutAsync()
    {
        var playerCode = "4udvxng80u8y";

        var request = new AllBetLogoutRequest
        {
            PlayerId = ToAllBetPlayerId(playerCode)
        };

        var response = await _client.LogoutAsync(_apiUrl, _propertyId, _apiKey, request).ConfigureAwait(false);
    }

    private string ToAllBetPlayerId(string playerCode)
        => $"{playerCode}_{_suffix}";
}