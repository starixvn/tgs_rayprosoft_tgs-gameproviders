using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.AsiaGaming;
using Tgs.GameProviders.Core.AsiaGaming.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest;

[TestFixture]
public class AsiaGamingApiClientUnitTest
{
    private const string AgentCode = "JH2_AGIN";
    private const string DesKey = "jh2hnM6K";
    private const string GameProviderCurrencyCode = "CNY";
    private const string Md5Key = "jh2bKFbpLZuW";
    private const string ProductionId = "JH2";
    private readonly Uri _loginApiUrl = new("http://gi.aggdemo.com:81");
    private readonly Uri _serviceApiUrl = new("https://swapi.etwlt.com");

    [Test]
    public async Task CheckOrCreateMemberAsync()
    {
        var playerCode = "4ucmqplxb029";

        var request = new AsiaGamingCheckOrCreateAccountRequest
        {
            AgentCode = AgentCode,
            PlayerCode = playerCode,
            Currency = GameProviderCurrencyCode
        };

        var client = Substitute.For<IAsiaGamingApiClient>();

        _ = await client
            .CheckOrCreateAccountAsync(_loginApiUrl, Md5Key, DesKey, request)
            .ConfigureAwait(false);
    }

    [Test]
    public async Task CreateGameSessionAsync()
    {
        var playerCode = "4ucmqplxb029";

        var request = new AsiaGamingCreateGameSessionRequest
        {
            ProductId = ProductionId,
            PlayerCode = playerCode,
            LoginRecordId = Guid.NewGuid(),
            Balance = 0
        };

        var client = Substitute.For<IAsiaGamingApiClient>();

        _ = await client
            .CreateGameSessionAsync(_serviceApiUrl, request)
            .ConfigureAwait(false);
    }
}
