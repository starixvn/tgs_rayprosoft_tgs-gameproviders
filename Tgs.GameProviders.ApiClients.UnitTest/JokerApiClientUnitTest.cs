using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.ApiClients.Joker;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.UnitTest;

[TestFixture]
public class JokerApiClientUnitTest
{
    private readonly Uri _apiUrl = new Uri("https://w.api788.net");
    private static readonly string _AppId = "TGVY";
    private static readonly string _SecretKey = "s5ghrn77baoto";

    [Test]
    public async Task GetBalanceAsync()
    {
        var playerCode = "4uf6k014jp5x";

        var httpClient = new HttpClient();
        var tgsLog = Substitute.For<ITgsLog>();

        var sut = new JokerApiClient(httpClient, tgsLog, NullLogger<JokerApiClient>.Instance);
        //var sut = Substitute.For<IJokerApiClient>();

        _ = await sut.GetBalanceAsync(_apiUrl, playerCode, _AppId, _SecretKey)
            .ConfigureAwait(false);
    }

    [Test]
    public async Task CreateUserAsync()
    {
        var playerCode = "4uf6k014jp5x";

        var httpClient = new HttpClient();
        var tgsLog = Substitute.For<ITgsLog>();

        var sut = new JokerApiClient(httpClient, tgsLog, NullLogger<JokerApiClient>.Instance);
        //var sut = Substitute.For<IJokerApiClient>();

        _ = await sut.CreateUserAsync(_apiUrl, playerCode, _AppId, _SecretKey)
            .ConfigureAwait(false);
    }

    [Test]
    public async Task GetGameTokenAsync()
    {
        var playerCode = "4uf6k014jp5x";

        var httpClient = new HttpClient();
        var tgsLog = Substitute.For<ITgsLog>();

        var sut = new JokerApiClient(httpClient, tgsLog, NullLogger<JokerApiClient>.Instance);
        //var sut = Substitute.For<IJokerApiClient>();

        _ = await sut.GetGameTokenAsync(_apiUrl, playerCode, _AppId, _SecretKey)
            .ConfigureAwait(false);
    }

    [Test]
    public async Task FundInAsync()
    {
        var playerCode = "4uf6k014jp5x";
        var amount = 500m;
        var orderId = Guid.NewGuid().ToString();

        var httpClient = new HttpClient();
        var tgsLog = Substitute.For<ITgsLog>();

        var sut = new JokerApiClient(httpClient, tgsLog, NullLogger<JokerApiClient>.Instance);
        //var sut = Substitute.For<IJokerApiClient>();

        _ = await sut.FundInAsync(_apiUrl, playerCode, orderId, amount, _AppId, _SecretKey)
            .ConfigureAwait(false);
    }

    [Test]
    public async Task FundOutAsync()
    {
        var playerCode = "4uf6k014jp5x";
        var amount = 500m;
        var orderId = Guid.NewGuid().ToString();

        var httpClient = new HttpClient();
        var tgsLog = Substitute.For<ITgsLog>();

        var sut = new JokerApiClient(httpClient, tgsLog, NullLogger<JokerApiClient>.Instance);
        //var sut = Substitute.For<IJokerApiClient>();

        _ = await sut.FundOutAsync(_apiUrl, playerCode, orderId, amount, _AppId, _SecretKey)
            .ConfigureAwait(false);
    }

    [Test]
    public async Task KickPlayerAsync()
    {
        var playerCode = "4uf6k014jp5x";

        var httpClient = new HttpClient();
        var tgsLog = Substitute.For<ITgsLog>();

        var sut = new JokerApiClient(httpClient, tgsLog, NullLogger<JokerApiClient>.Instance);
        //var sut = Substitute.For<IJokerApiClient>();

        await sut.KickPlayerAsync(_apiUrl, playerCode, _AppId, _SecretKey)
            .ConfigureAwait(false);
    }
}