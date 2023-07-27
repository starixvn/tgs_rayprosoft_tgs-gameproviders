using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameProviders.Core.SBO;
using Tgs.GameProviders.Core.SBO.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest;

public class SBOApiClientUnitTest
{
    private const string AgentPrefix = "STGTGS";
    private const string BrandCode = "GPI";
    private const string CompanyKey = "34B2C4ED616F4E7B965EAE0861A3F92E";
    private const string GameProviderCurrencyCode = "CNY";
    private const string PlayerCurrencyCode = "CNY";
    private readonly Uri _apiUrl = new("https://ex-api-demo-yy.568win.com");

    [Fact]
    public async Task CreateAgentAsync()
    {
        var agentId =
            $"{AgentPrefix}{BrandCode.ToUpper()}{PlayerCurrencyCode.Replace("_", string.Empty).ToUpper()}";

        var request = new SBOCreateAgentRequest
        {
            CompanyKey = CompanyKey,
            AgentId = agentId,
            CurrencyCode = GameProviderCurrencyCode
        };

        var client = Substitute.For<ISBOApiClient>();

        _ = await client.CreateAgentAsync(_apiUrl, request).ConfigureAwait(false);
    }

    [Fact]
    public async Task CreatePlayerAsync()
    {
        var playerCode = "4udwxysersic";

        var agentId =
            $"{AgentPrefix}{BrandCode.ToUpper()}{PlayerCurrencyCode.Replace("_", string.Empty).ToUpper()}";

        var request = new SBOCreatePlayerRequest
        {
            CompanyKey = CompanyKey,
            PlayerCode = playerCode,
            AgentId = agentId
        };

        var client = Substitute.For<ISBOApiClient>();

        _ = await client.CreatePlayerAsync(_apiUrl, request).ConfigureAwait(false);
    }

    [Fact]
    public async Task GetGameUrlAsync()
    {
        var playerCode = "4ucmqplxb029";

        var request = new SBOLoginRequest
        {
            CompanyKey = CompanyKey,
            PlayerCode = playerCode,
            Portfolio = "SportsBook"
        };

        var client = Substitute.For<ISBOApiClient>();

        _ = await client.LoginAsync(_apiUrl, request).ConfigureAwait(false);
    }

    [Fact]
    public async Task GetSportsBookRoundHistoryAsync()
    {
        var request = new SBOGetRoundHistoryRequest
        {
            CompanyKey = CompanyKey,
            Portfolio = "SportsBook",
            ExternalRoundId = "4040907",
            Language = "en_US"
        };

        var client = Substitute.For<ISBOApiClient>();

        _ = await client.GetRoundHistoryAsync(_apiUrl, request).ConfigureAwait(false);
    }

    [Fact]
    public async Task GetVirtualSportsRoundHistoryAsync()
    {
        var request = new SBOGetRoundHistoryRequest
        {
            CompanyKey = CompanyKey,
            Portfolio = "VirtualSports",
            ExternalRoundId = "B2677480",
            Language = "en_US"
        };

        var client = Substitute.For<ISBOApiClient>();

        _ = await client.GetRoundHistoryAsync(_apiUrl, request).ConfigureAwait(false);
    }

    [Fact]
    public async Task KickAsync()
    {
        var playerCode = "4ucmqplxb029";

        var request = new SBOLogoutRequest
        {
            CompanyKey = CompanyKey,
            PlayerCode = playerCode
        };

        var client = Substitute.For<ISBOApiClient>();

        _ = await client.LogoutAsync(_apiUrl, request).ConfigureAwait(false);
    }
}
