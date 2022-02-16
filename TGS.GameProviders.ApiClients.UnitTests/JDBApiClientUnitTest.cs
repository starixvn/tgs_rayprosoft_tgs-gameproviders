using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.JDB;
using TGS.GameProviders.Core.JDB;
using TGS.GameProviders.Core.JDB.Models;

namespace TGS.GameProviders.ApiClients.UnitTests;

[TestFixture]
public class JDBApiClientUnitTest
{
    private readonly string _aesIv = "756edb2fc85fe908";
    private readonly string _aesKey = "4f09f467e6a8fb9f";
    private readonly string _agentId = "wgvb3rmb";
    private readonly Uri _apiUrl = new("http://api.jdb711.com");
    private readonly IJDBApiClient _client;
    private readonly string _clientDomain = "VB3";

    public JDBApiClientUnitTest()
    {
        var httpClient = new HttpClient();
        var logger = NullLogger<JDBApiClient>.Instance;
        var tgsLog = Substitute.For<ITgsLog>();

        //_client = new JDBApiClient(logger, httpClient, tgsLog);
        _client = Substitute.For<IJDBApiClient>();
    }

    [Test]
    public async Task GetBetHistoryAsync()
    {
        var playerCode = "4ucmqplxb029";

        var request = new JDBBetHistoryRequest
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Parent = _agentId,
            UserId = playerCode,
            Language = "en",
            GameType = 0,
            SequenceNo = 5250172280333
        };

        var response = await _client
            .GetBetHistoryUrlAsync(
                _apiUrl,
                _clientDomain,
                _aesIv,
                _aesKey,
                request)
            .ConfigureAwait(false);
    }

    [Test]
    public async Task GetTokenAsync()
    {
        var playerCode = "4ucmqplxb029";

        var request = new JDBGetTokenRequest
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Parent = _agentId,
            UserId = playerCode,
            Language = "en",
            GameType = "0",
            MachineType = "14011",
            MoreGame = 0
        };

        var response = await _client
            .GetTokenAsync(
                _apiUrl,
                _clientDomain,
                _aesIv,
                _aesKey,
                request)
            .ConfigureAwait(false);
    }

    [Test]
    public async Task LogoutAsync()
    {
        var playerCode = "4ucmqplxb029";

        var request = new JDBLogoutRequest
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Parent = _agentId,
            UserId = playerCode
        };

        var response = await _client
            .LogoutAsync(
                _apiUrl,
                _clientDomain,
                _aesIv,
                _aesKey,
                request)
            .ConfigureAwait(false);
    }
}
