using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.JDB;
using Tgs.GameProviders.Core.JDB.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest;

[TestFixture]
public class JDBApiClientUnitTest
{
    private const string AesIv = "756edb2fc85fe908";
    private const string AesKey = "4f09f467e6a8fb9f";
    private const string AgentId = "wgvb3rmb";
    private const string ClientDomain = "VB3";
    private readonly Uri _apiUrl = new("http://api.jdb711.com");

    [Test]
    public async Task GetBetHistoryAsync()
    {
        var playerCode = "4ucmqplxb029";

        var request = new JDBBetHistoryRequest
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Parent = AgentId,
            UserId = playerCode,
            Language = "en",
            GameType = 0,
            SequenceNo = 5250172280333
        };

        var client = Substitute.For<IJDBApiClient>();

        _ = await client
            .GetBetHistoryUrlAsync(
                _apiUrl,
                ClientDomain,
                AesIv,
                AesKey,
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
            Parent = AgentId,
            UserId = playerCode,
            Language = "en",
            GameType = "0",
            MachineType = "14011",
            MoreGame = 0
        };

        var client = Substitute.For<IJDBApiClient>();

        _ = await client
            .GetTokenAsync(
                _apiUrl,
                ClientDomain,
                AesIv,
                AesKey,
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
            Parent = AgentId,
            UserId = playerCode
        };

        var client = Substitute.For<IJDBApiClient>();

        _ = await client
            .LogoutAsync(
                _apiUrl,
                ClientDomain,
                AesIv,
                AesKey,
                request)
            .ConfigureAwait(false);
    }
}
