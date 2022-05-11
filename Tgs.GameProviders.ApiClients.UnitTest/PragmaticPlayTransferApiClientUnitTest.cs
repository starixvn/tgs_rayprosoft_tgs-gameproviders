using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.ApiClients.PragmaticPlay;
using Tgs.GameProviders.Core.PragmaticPlay;
using Tgs.GameProviders.Core.PragmaticPlay.Models;
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

        var response = await client.FundInAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            playerCode,
            orderId,
            amount).ConfigureAwait(false);

        await TestContext.Progress.WriteLineAsync(JsonSerializer.Serialize(response));
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

        var response = await client.FundOrderCheckingAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            orderId).ConfigureAwait(false);

        await TestContext.Progress.WriteLineAsync(JsonSerializer.Serialize(response));
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
        //var orderId = Guid.NewGuid().ToString("N");
        var orderId = "4c3415e7138942e1b9b48a5fd10e1d47";
        var amount = 500m;

        //var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        var client = new PragmaticPlayTransferApiClient(httpClient, logger, tgsLog);

        var response = await client.FundOutAsync(
            _apiUrl,
            securityLogin,
            secretKey,
            playerCode,
            orderId,
            amount).ConfigureAwait(false);

        await TestContext.Progress.WriteLineAsync(JsonSerializer.Serialize(response));
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

        await TestContext.Progress.WriteLineAsync(balance.GetValueOrDefault().ToString());
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

        //Console.WriteLine(data);

        //using var stringReader = new StringReader(data);
        //var timepoint = await stringReader.ReadLineAsync();

        //var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        //{
        //    HasHeaderRecord = true
        //    //HasHeaderRecord = false
        //};

        //using var csvReader = new CsvReader(stringReader, configuration);

        //var records = csvReader.GetRecords<RoundRecord>();

        //while (csvReader.Read())
        //{
        //    var playerId = csvReader.GetField<string>("playerID");
        //    var extPlayerID = csvReader.GetField<string>("extPlayerID");
        //    var gameId = csvReader.GetField<string>("gameID");
        //}
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
        var externalGameId = "vs20olympgate";
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
        var language = "en";

        //var client = Substitute.For<IPragmaticPlayTransferApiClient>();

        var client = new PragmaticPlayTransferApiClient(httpClient, logger, tgsLog);

        _ = await client
            .GetRoundHistoryAsync(
                _apiUrl,
                securityLogin,
                secretKey,
                playerCode,
                roundId,
                language)
            .ConfigureAwait(false);
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