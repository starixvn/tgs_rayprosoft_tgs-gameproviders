using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Joker;
using Tgs.GameProviders.Core.Joker.Models;

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

        var sut = Substitute.For<IJokerApiClient>();

        _ = sut.CreateUserAsync(
                Arg.Is(_apiUrl),
                Arg.Is(playerCode),
                Arg.Is(_AppId),
                Arg.Is(_SecretKey))
            .Returns(true);

        var actual = await sut.CreateUserAsync(_apiUrl, playerCode, _AppId, _SecretKey)
            .ConfigureAwait(false);

        Assert.AreEqual(true, actual);
    }

    [Test]
    public async Task FundInAsync()
    {
        var playerCode = "4uf6k014jp5x";
        var amount = 500m;
        var orderId = Guid.NewGuid().ToString();

        var sut = Substitute.For<IJokerApiClient>();

        _ = sut.FundInAsync(
                Arg.Is(_apiUrl),
                Arg.Is(playerCode),
                Arg.Is(orderId),
                Arg.Is(amount),
                Arg.Is(_AppId),
                Arg.Is(_SecretKey))
            .Returns(true);

        var actual = await sut.FundInAsync(_apiUrl, playerCode, orderId, amount, _AppId, _SecretKey)
            .ConfigureAwait(false);

        Assert.AreEqual(true, actual);
    }

    [Test]
    public async Task FundOrderCheckingAsync()
    {
        var orderId = "00acb462-4301-4f83-82e2-9983a5705403";

        var sut = Substitute.For<IJokerApiClient>();

        _ = sut.FundOrderCheckingAsync(
                Arg.Is(_apiUrl),
                Arg.Is(orderId),
                Arg.Is(_AppId),
                Arg.Is(_SecretKey))
            .Returns(true);

        var actual = await sut.FundOrderCheckingAsync(_apiUrl, orderId, _AppId, _SecretKey)
            .ConfigureAwait(false);

        Assert.AreEqual(true, actual);
    }

    [Test]
    public async Task FundOutAsync()
    {
        var playerCode = "4uf6k014jp5x";
        var amount = 500m;
        var orderId = Guid.NewGuid().ToString();

        var sut = Substitute.For<IJokerApiClient>();

        _ = sut.FundOutAsync(
                Arg.Is(_apiUrl),
                Arg.Is(playerCode),
                Arg.Is(orderId),
                Arg.Is(amount),
                Arg.Is(_AppId),
                Arg.Is(_SecretKey))
            .Returns(true);

        var actual = await sut.FundOutAsync(_apiUrl, playerCode, orderId, amount, _AppId, _SecretKey)
            .ConfigureAwait(false);

        Assert.AreEqual(true, actual);
    }

    [Test]
    public async Task GetBalanceAsync()
    {
        var playerCode = "4uf6k014jp5x";

        var sut = Substitute.For<IJokerApiClient>();

        _ = sut.GetBalanceAsync(
                Arg.Is(_apiUrl),
                Arg.Is(playerCode),
                Arg.Is(_AppId),
                Arg.Is(_SecretKey))
            .Returns((true, 1000m));

        var actual = await sut
            .GetBalanceAsync(_apiUrl, playerCode, _AppId, _SecretKey)
            .ConfigureAwait(false);

        Assert.AreEqual(true, actual.IsPlayerExist);
        Assert.AreEqual(1000m, actual.Balance);
    }

    [Test]
    public async Task GetGameRecordAsync()
    {
        var startTime = new DateTimeOffset(2022, 4, 20, 1, 30, 0, TimeSpan.FromHours(0));
        var endTime = new DateTimeOffset(2022, 4, 20, 2, 0, 0, TimeSpan.FromHours(0));

        string? nextId = null;

        var response = new JokerGameRecordInfoResponse
        {
            NextId = "eyIyMDIyMDQyMDA5NTgiOnsiQ3JlYXRlZFRpbWUiOiI1NTdmOGNjMC1jMDRkLTExZWMtYmVlYy1kZjk4OWNhYjE5MGMifX0=",
            Data = new JokerGameRecordInfoData
            {
                Game = new JokerGameRecordData[]
                {
                    new()
                    {
                        TransactionId = "xmkeajuna89ne",
                        Username = "4UF6K014JP5X",
                        GameExternalId= "7rw3tfwz11kaw",
                        Description= "Main Spin",
                        BetAmount = 1.60m,
                        SettleAmount= 3.20m,
                        Time= DateTimeOffset.Parse("2022-04-20T09:58:08.9660000+08:00"),
                        Type = "Game"
                    }
                }
            }
        };

        var sut = Substitute.For<IJokerApiClient>();

        _ = sut.GetGameRecordAsync(
                Arg.Is(_apiUrl),
                Arg.Is(startTime),
                Arg.Is(endTime),
                Arg.Is(nextId),
                Arg.Is(_AppId),
                Arg.Is(_SecretKey))
            .Returns((true, response));

        var actual = await sut.GetGameRecordAsync(
                _apiUrl,
                startTime,
                endTime,
                nextId,
                _AppId,
                _SecretKey)
             .ConfigureAwait(false);

        Assert.AreEqual(true, actual.Success);
        Assert.AreEqual(response, actual.Response);
    }

    [Test]
    public async Task GetGameTokenAsync()
    {
        var playerCode = "4uf6k014jp5x";

        var sut = Substitute.For<IJokerApiClient>();

        var token = "token";

        _ = sut.GetGameTokenAsync(
                Arg.Is(_apiUrl),
                Arg.Is(playerCode),
                Arg.Is(_AppId),
                Arg.Is(_SecretKey))
            .Returns(token);

        var actual = await sut.GetGameTokenAsync(_apiUrl, playerCode, _AppId, _SecretKey)
            .ConfigureAwait(false);

        Assert.AreEqual(token, actual);
    }

    [Test]
    public async Task GetGameDetailUrlAsync()
    {
        var externalRoundId = "xmkeajuna89ne";
        var dataType = JokerDataType.Game;
        var language = "en";

        var sut = Substitute.For<IJokerApiClient>();

        var historyUrl = "//www.google.com";

        _ = sut.GetGameDetailUrlAsync(
                Arg.Is(_apiUrl),
                Arg.Is(language),
                Arg.Is(externalRoundId),
                Arg.Is(dataType),
                Arg.Is(_AppId),
                Arg.Is(_SecretKey))
            .Returns(historyUrl);

        var actual = await sut
            .GetGameDetailUrlAsync(_apiUrl, language, externalRoundId, dataType, _AppId, _SecretKey)
            .ConfigureAwait(false);

        Assert.AreEqual(historyUrl, actual);
    }

    [Test]
    public async Task KickPlayerAsync()
    {
        var playerCode = "4uf6k014jp5x";

        var sut = Substitute.For<IJokerApiClient>();

        await sut.KickPlayerAsync(_apiUrl, playerCode, _AppId, _SecretKey)
            .ConfigureAwait(false);
    }
}