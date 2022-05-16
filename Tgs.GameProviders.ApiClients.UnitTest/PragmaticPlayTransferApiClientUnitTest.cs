using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.PragmaticPlayTransfer;

namespace Tgs.GameProviders.ApiClients.UnitTest;

internal class PragmaticPlayTransferApiClientUnitTest
{
    private readonly Uri _apiUrl = new("https://api.prerelease-env.biz");

    [Test]
    public async Task CreatePlayerAsync()
    {
        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";
        var playerCode = "4ucmqplxb029";
        var currencyCode = "CNY";

        var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        _ = client.CreatePlayerAsync(
             Arg.Is(_apiUrl),
             Arg.Is(securityLogin),
             Arg.Is(secretKey),
             Arg.Is(playerCode),
             Arg.Is(currencyCode))
            .Returns(true);

        var actual = await client.CreatePlayerAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            playerCode,
            currencyCode).ConfigureAwait(false);

        Assert.AreEqual(true, actual);
    }

    [Test]
    public async Task FundInAsync()
    {
        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";
        var playerCode = "4ucmqplxb029";
        var orderId = Guid.NewGuid().ToString("N");
        var amount = 200m;

        var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        _ = client.FundInAsync(
            Arg.Is(_apiUrl),
            Arg.Is(securityLogin),
            Arg.Is(secretKey),
            Arg.Is(playerCode),
            Arg.Is(orderId),
            Arg.Is(amount))
            .Returns(true);

        var actual = await client.FundInAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            playerCode,
            orderId,
            amount).ConfigureAwait(false);

        Assert.AreEqual(true, actual);
    }

    [Test]
    public async Task FundOrderCheckingAsync()
    {
        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";
        var orderId = "4c3415e7138942e1b9b48a5fd10e1d47";

        var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        _ = client.FundOrderCheckingAsync(
                Arg.Is(_apiUrl),
                Arg.Is(securityLogin),
                Arg.Is(secretKey),
                Arg.Is(orderId))
            .Returns(true);

        var actual = await client.FundOrderCheckingAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            orderId).ConfigureAwait(false);

        Assert.AreEqual(true, actual);
    }

    [Test]
    public async Task FundOutAsync()
    {
        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";
        var playerCode = "4ucmqplxb029";
        var orderId = Guid.NewGuid().ToString("N");
        var amount = 500m;

        var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        _ = client.FundOutAsync(
                Arg.Is(_apiUrl),
                Arg.Is(securityLogin),
                Arg.Is(secretKey),
                Arg.Is(playerCode),
                Arg.Is(orderId),
                Arg.Is(amount))
            .Returns(true);

        var actual = await client.FundOutAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            playerCode,
            orderId,
            amount).ConfigureAwait(false);

        Assert.AreEqual(true, actual);
    }

    [Test]
    public async Task GetBalanceAsync()
    {
        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";
        var playerCode = "4ucmqplxb029";
        var balance = 1000m;

        var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        _ = client.GetBalanceAsync(
            Arg.Is(_apiUrl),
            Arg.Is(securityLogin),
            Arg.Is(secretKey),
            Arg.Is(playerCode))
            .Returns((true, balance));

        var actual = await client.GetBalanceAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            playerCode).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(true, actual.IsPlayerExist);
        Assert.AreEqual(balance, actual.Balance);
    }

    [Test]
    public async Task GetGameRoundRecordsAsync()
    {
        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";

        var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        var time = DateTimeOffset.UtcNow.AddMinutes(-10);

        var data = await client.GetGameRoundRecordsAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            "RNG",
            time).ConfigureAwait(false);
    }

    [Test]
    public async Task GetGameUrlAsync()
    {
        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";
        var playerCode = "4ucmqplxb029";
        var externalGameId = "vs25bkofkngdm";
        var language = "en";
        var platform = "WEB";

        var gameUrl = "https://tgs.prerelease-env.biz/gs2c/openGame.do?tc=97YBxmEGn5PCQv1zxvHGDlfRWeqGsKxLc2K1HsdyJKiIDck46D5azpqgW0Be3PrO&stylename=tgs_tgstfw&dummy=";

        var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        _ = client.GetGameUrlAsync(
            Arg.Is(_apiUrl),
            Arg.Is(securityLogin),
            Arg.Is(secretKey),
            Arg.Is(playerCode),
            Arg.Is(externalGameId),
            Arg.Is(language),
            Arg.Is(platform),
            Arg.Is((string?)null))
            .Returns(gameUrl);

        var actual = await client.GetGameUrlAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            playerCode,
            externalGameId,
            language,
            platform,
            null).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual);
    }

    [Test]
    public async Task GetGameDetailUrlAsync()
    {
        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";
        var playerCode = "4ucmqplxb029";
        var roundId = "2759382176";
        var gameId = "vs20kraken";
        var language = "en";

        var historyUrl = "https://tgs.prerelease-env.biz/gs2c/parentRoundHistoryDetails.do?playSessionId=2806662804&stylename=tgs_tgstfw&hash=2213f4013b94f67808ff131af8110f7d&language=en";

        var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        _ = client
            .GetGameDetailUrlAsync(
                Arg.Is(_apiUrl),
                Arg.Is(securityLogin),
                Arg.Is(secretKey),
                Arg.Is(playerCode),
                Arg.Is(roundId),
                Arg.Is(gameId),
                Arg.Is(language))
            .Returns(historyUrl);

        var actual = await client
            .GetGameDetailUrlAsync(
                _apiUrl,
                securityLogin,
                secretKey,
                playerCode,
                roundId,
                gameId,
                language)
            .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(historyUrl, actual);
    }

    [Test]
    public async Task KickPlayerAsync()
    {
        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";
        var playerCode = "4ucmqplxb029";

        var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        await client
            .LogoutAsync(
                _apiUrl, securityLogin, secretKey, playerCode)
            .ConfigureAwait(false);
    }
}