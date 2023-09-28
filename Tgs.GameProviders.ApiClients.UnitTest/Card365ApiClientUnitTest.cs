using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameProviders.Core.Card365;
using Tgs.GameProviders.Core.Card365.Model;

namespace Tgs.GameProviders.ApiClients.UnitTest;

public class Card365ApiClientUnitTest
{
    private const string AgentId = "card0201";
    private const string Cert = "755C24B710D9F2B4B15DAB2B489CF471";
    private const string WebSite = "card365pega";
    private readonly Uri _apiUrl = new("https://g5ky3i.365gaming.cc/api");

    [Fact]
    public async Task GetAccessKeyAsync()
    {
        var playerCode = "4ucmqplxb029";
        var currencyCode = "3";  // CNY

        var request = new Card365GetKeyRequest
        {
            Cert = Cert,
            PlayerCode = playerCode,
            Username = playerCode,
            AgentId = AgentId,
            CurrencyCode = currencyCode
        };

        var client = Substitute.For<ICard365ApiClient>();

        _ = await client.GetAccessKeyAsync(_apiUrl, WebSite, request);
    }

    [Fact]
    public async Task LogoutAsync()
    {
        var playerCode = "4ucmqplxb029";

        var request = new Card365LogoutRequest
        {
            Cert = Cert,
            PlayerCode = playerCode,
            AgentId = AgentId
        };

        var client = Substitute.For<ICard365ApiClient>();

        _ = await client
            .LogoutAsync(
            apiUrl: _apiUrl,
            website: WebSite,
            request: request);
    }
}
