using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.Card365;
using TGS.GameProviders.Core.Card365;
using TGS.GameProviders.Core.Card365.Model;

namespace TGS.GameProviders.ApiClients.UnitTests;

[TestFixture]
public class Card365ApiClientUnitTest
{
    private readonly ICard365ApiClient _client;

    private readonly Uri _apiUrl = new Uri("https://g5ky3i.365gaming.cc/api");
    private readonly string _webSite = "card365pega";
    private readonly string _agentId = "card0201";
    private readonly string _cert = "755C24B710D9F2B4B15DAB2B489CF471";

    public Card365ApiClientUnitTest()
    {
        var httpClient = new HttpClient();
        var logger = NullLogger<Card365ApiClient>.Instance;
        var tgsLog = Substitute.For<ITgsLog>();

        _client = Substitute.For<ICard365ApiClient>();
        //_client = new Card365ApiClient(logger, httpClient, tgsLog);
    }

    [Test]
    public async Task GetAccessKeyAsync()
    {
        var playerCode = "4ucmqplxb029";
        var currencyCode = "3";  // CNY

        var request = new Card365GetKeyRequest
        {
            Cert = _cert,
            PlayerCode = playerCode,
            Username = playerCode,
            AgentId = _agentId,
            CurrencyCode = currencyCode
        };

        var response = await _client
            .GetAccessKeyAsync(
                _apiUrl,
                _webSite,
                request)
            .ConfigureAwait(false);
    }

    [Test]
    public async Task LogoutAsync()
    {
        var playerCode = "4ucmqplxb029";

        var request = new Card365LogoutRequest
        {
            Cert = _cert,
            PlayerCode = playerCode,
            AgentId = _agentId
        };

        var response = await _client
            .LogoutAsync(
                _apiUrl,
                _webSite,
                request)
            .ConfigureAwait(false);
    }
}