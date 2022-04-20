using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.ApiClients.Joker;
using Tgs.GameProviders.Core.Joker;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.UnitTest;

[TestFixture]
public class JokerApiClientUnitTest
{
    private static readonly string _AppId = "TGVY";
    private static readonly string _SecretKey = "s5ghrn77baoto";
    private readonly Uri _apiUrl = new Uri("https://w.api788.net");

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
    public async Task FundOrderCheckingAsync()
    {
        var orderId = "00acb462-4301-4f83-82e2-9983a5705403";

        var httpClient = new HttpClient();
        var tgsLog = Substitute.For<ITgsLog>();

        var sut = new JokerApiClient(httpClient, tgsLog, NullLogger<JokerApiClient>.Instance);
        //var sut = Substitute.For<IJokerApiClient>();

        _ = await sut.FundOrderCheckingAsync(_apiUrl, orderId, _AppId, _SecretKey)
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
    public async Task GetGameRecordAsync()
    {
        //var startTime = DateTimeOffset.UtcNow.AddMinutes(-10).ToOffset(TimeSpan.FromHours(8));
        //var endTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));

        var startTime = new DateTimeOffset(2022, 4, 20, 0, 0, 0, TimeSpan.FromHours(8));
        var endTime = new DateTimeOffset(2022, 4, 21, 0, 0, 0, TimeSpan.FromHours(8));

        string? nextId = null;

        //nextId = "eyIyMDIyMDQyMDA5NTgiOnsiQ3JlYXRlZFRpbWUiOiI1NTdmOGNjMC1jMDRkLTExZWMtYmVlYy1kZjk4OWNhYjE5MGMifX0=";

        var httpClient = new HttpClient();
        var tgsLog = Substitute.For<ITgsLog>();

        var sut = new JokerApiClient(httpClient, tgsLog, NullLogger<JokerApiClient>.Instance);
        //var sut = Substitute.For<IJokerApiClient>();

        var (success, response) = await sut.GetGameRecordAsync(_apiUrl, startTime, endTime, nextId, _AppId, _SecretKey)
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
    public async Task GetGameDetailUrlAsync()
    {
        var externalRoundId = "xmkeajuna89ne";
        var dataType = JokerDataType.Game;
        var language = "en";

        var httpClient = new HttpClient();
        var tgsLog = Substitute.For<ITgsLog>();

        var sut = new JokerApiClient(httpClient, tgsLog, NullLogger<JokerApiClient>.Instance);
        //var sut = Substitute.For<IJokerApiClient>();

        _ = await sut.GetGameDetailUrlAsync(_apiUrl, language, externalRoundId, dataType, _AppId, _SecretKey)
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