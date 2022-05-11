using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.ApiClients.PragmaticPlay;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.UnitTest;

internal class PragmaticPlayTransferApiClientUnitTest
{
    private readonly Uri _apiUrl = new("https://api.prerelease-env.biz");

    [Test]
    public async Task CreatePlayerAsync()
    {
        var httpClient = new HttpClient();
        var logger = NullLogger<PragmaticPlayTransferApiClient>.Instance;
        var tgsLog = Substitute.For<ITgsLog>();

        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";
        var playerCode = "4ucmqplxb029";
        var currencyCode = "CNY";

        //var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        var client = new PragmaticPlayTransferApiClient(httpClient, logger, tgsLog);

        var created = await client.CreatePlayerAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            playerCode,
            currencyCode).ConfigureAwait(false);

        await TestContext.Progress.WriteLineAsync(created.ToString());
    }

    [Test]
    public async Task FundInAsync()
    {
        var httpClient = new HttpClient();
        var logger = NullLogger<PragmaticPlayTransferApiClient>.Instance;
        var tgsLog = Substitute.For<ITgsLog>();

        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";
        var playerCode = "4ucmqplxb029";
        var orderId = Guid.NewGuid().ToString("N");
        var amount = 200m;

        //var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        var client = new PragmaticPlayTransferApiClient(httpClient, logger, tgsLog);

        var success = await client.FundInAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            playerCode,
            orderId,
            amount).ConfigureAwait(false);

        await TestContext.Progress.WriteLineAsync(success.ToString());
    }

    [Test]
    public async Task FundOrderCheckingAsync()
    {
        var httpClient = new HttpClient();
        var logger = NullLogger<PragmaticPlayTransferApiClient>.Instance;
        var tgsLog = Substitute.For<ITgsLog>();

        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";
        var orderId = "4c3415e7138942e1b9b48a5fd10e1d47";

        //var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        var client = new PragmaticPlayTransferApiClient(httpClient, logger, tgsLog);

        var exist = await client.FundOrderCheckingAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            orderId).ConfigureAwait(false);

        await TestContext.Progress.WriteLineAsync(exist.ToString());
    }

    [Test]
    public async Task FundOutAsync()
    {
        var httpClient = new HttpClient();
        var logger = NullLogger<PragmaticPlayTransferApiClient>.Instance;
        var tgsLog = Substitute.For<ITgsLog>();

        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";
        var playerCode = "4ucmqplxb029";
        var orderId = Guid.NewGuid().ToString("N");
        //var orderId = "4c3415e7138942e1b9b48a5fd10e1d47";
        var amount = 500m;

        //var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        var client = new PragmaticPlayTransferApiClient(httpClient, logger, tgsLog);

        var success = await client.FundOutAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            playerCode,
            orderId,
            amount).ConfigureAwait(false);

        await TestContext.Progress.WriteLineAsync(success.ToString());
    }

    [Test]
    public async Task GetBalanceAsync()
    {
        var httpClient = new HttpClient();
        var logger = NullLogger<PragmaticPlayTransferApiClient>.Instance;
        var tgsLog = Substitute.For<ITgsLog>();

        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";
        var playerCode = "4ucmqplxb029";

        //var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        var client = new PragmaticPlayTransferApiClient(httpClient, logger, tgsLog);

        var (exist, balance) = await client.GetBalanceAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            playerCode).ConfigureAwait(false);

        var amount = !balance.HasValue ? "null" : $"{balance.Value}";

        await TestContext.Progress.WriteLineAsync($"exist: {exist}");
        await TestContext.Progress.WriteLineAsync($"balance: {amount}");
    }

    [Test]
    public async Task GetGameRoundRecordsAsync()
    {
        var httpClient = new HttpClient();
        var logger = NullLogger<PragmaticPlayTransferApiClient>.Instance;
        var tgsLog = Substitute.For<ITgsLog>();

        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";

        //var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        var client = new PragmaticPlayTransferApiClient(httpClient, logger, tgsLog);

        var time = DateTimeOffset.UtcNow.AddMinutes(-10);

        Console.WriteLine("timepoint: {0}", time.ToUnixTimeMilliseconds());

        var data = await client.GetGameRoundRecordsAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            "RNG",
            time).ConfigureAwait(false);

        await TestContext.Progress.WriteLineAsync(data);
    }

    [Test]
    public async Task GetGameUrlAsync()
    {
        var httpClient = new HttpClient();
        var logger = NullLogger<PragmaticPlayTransferApiClient>.Instance;
        var tgsLog = Substitute.For<ITgsLog>();

        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";
        var playerCode = "4ucmqplxb029";
        var externalGameId = "vs25bkofkngdm";
        var language = "en";
        var platform = "WEB";

        //var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        var client = new PragmaticPlayTransferApiClient(httpClient, logger, tgsLog);

        var url = await client.GetGameUrlAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            playerCode,
            externalGameId,
            language,
            platform,
            null).ConfigureAwait(false);

        await TestContext.Progress.WriteLineAsync(url);
    }

    [Test]
    public async Task GetRoundHistoryAsync()
    {
        var httpClient = new HttpClient();
        var logger = NullLogger<PragmaticPlayTransferApiClient>.Instance;
        var tgsLog = Substitute.For<ITgsLog>();

        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";
        var playerCode = "4ucmqplxb029";
        var roundId = "2759382176";
        var gameId = "vs20kraken";
        var language = "en";

        //var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        var client = new PragmaticPlayTransferApiClient(httpClient, logger, tgsLog);

        var url = await client
            .GetRoundHistoryAsync(
                _apiUrl,
                securityLogin,
                secretKey,
                playerCode,
                roundId,
                gameId,
                language)
            .ConfigureAwait(false);

        await TestContext.Progress.WriteLineAsync(url);
    }

    [Test]
    public async Task KickPlayerAsync()
    {
        var httpClient = new HttpClient();
        var logger = NullLogger<PragmaticPlayTransferApiClient>.Instance;
        var tgsLog = Substitute.For<ITgsLog>();

        var securityLogin = "tgs_tgstfw";
        var secretKey = "testKey";
        var playerCode = "4ucmqplxb029";

        //var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        var client = new PragmaticPlayTransferApiClient(httpClient, logger, tgsLog);

        await client
            .LogoutAsync(_apiUrl, securityLogin, secretKey, playerCode)
            .ConfigureAwait(false);
    }
}