using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.Jili;
using TGS.GameProviders.Core.Jili;
using TGS.GameProviders.Core.Jili.Models;

namespace TGS.GameProviders.ApiClients.UnitTests;

[TestFixture]
public class JiliApiClientUnitTest
{
    private readonly string _agentId = "PG_Seamless";
    private readonly string _agentKey = "0d90dda822b6df548460da8f551c1a76c01b6a9e";
    private readonly Uri _apiUrl = new Uri("https://uat-wb-api.jlfafafa2.com");
    private readonly IJiliApiClient _client;

    public JiliApiClientUnitTest()
    {
        var httpClient = new HttpClient();
        var logger = NullLogger<JiliApiClient>.Instance;
        var tgsLog = Substitute.For<ITgsLog>();

        //_client = new JiliApiClient(logger, httpClient, tgsLog);
        _client = Substitute.For<IJiliApiClient>();
    }

    [Test]
    public async Task GetGameDetailUrlAsync()
    {
        var language = "en-US";
        var externalRoundId = "1639679431950180036";

        var response = await _client
            .GetGameDetailUrlAsync(_apiUrl, _agentId, _agentKey, language, externalRoundId)
            .ConfigureAwait(false);
    }

    [Test]
    public async ValueTask KickMemberAsync()
    {
        var jiliPlayerId = "GPI_4ucmqplxb029";

        var response = await _client
            .KickMemberAsync(_apiUrl, jiliPlayerId, _agentId, _agentKey)
            .ConfigureAwait(false);
    }

    [Test]
    public async ValueTask LoginWithoutRedirectAsync()
    {
        var request = new JiliLoginWithoutRedirectRequest
        {
            GameExternalId = "10",
            Token = "token",
            Language = "en-US"
        };

        var response = await _client
            .LoginWithoutRedirectAsync(_apiUrl, _agentId, _agentKey, request)
            .ConfigureAwait(false);
    }
}
