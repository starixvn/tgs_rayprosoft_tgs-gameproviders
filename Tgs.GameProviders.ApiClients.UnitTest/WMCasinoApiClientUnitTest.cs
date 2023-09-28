using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameProviders.Core.WMCasino;
using Tgs.GameProviders.Core.WMCasino.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest;

public class WMCasinoApiClientUnitTest
{
    private const string AgentId = "hpgtwapi";
    private const string AgentPrefix = "pg";
    private const string AgentSuffix = "cny";
    private const string Signature = "97ffc7ab86c2f54be533e90da6f6ab9f";
    private readonly Uri _apiUrl = new("https://api.a45.me/api/wallet/Gateway.php");

    [Fact]
    public async Task CreatePlayerAsync()
    {
        var playerCode = "4ucmqplxb029";
        var playerId = ToWMCasinoPlayerId(playerCode);

        var request = new WMCasinoMemberRegisterRequest
        {
            Signature = Signature,
            VendorId = AgentId,
            User = playerId,
            Username = playerCode
        };

        var client = Substitute.For<IWMCasinoApiClient>();

        _ = await client.MemberRegisterAsync(_apiUrl, request);
    }

    [Fact]
    public async Task GetGameUrlAsync()
    {
        var playerCode = "4ucmqplxb029";
        var playerId = ToWMCasinoPlayerId(playerCode);

        var request = new WMCasinoLoginGameRequest
        {
            Signature = Signature,
            VendorId = AgentId,
            User = playerId
        };

        var client = Substitute.For<IWMCasinoApiClient>();

        _ = await client.LoginGameAsync(_apiUrl, request);
    }

    [Fact]
    public async Task KickAsync()
    {
        var playerCode = "4ucmqplxb029";
        var playerId = ToWMCasinoPlayerId(playerCode);

        var request = new WMCasinoLogoutGameRequest
        {
            Signature = Signature,
            VendorId = AgentId,
            User = playerId
        };

        var client = Substitute.For<IWMCasinoApiClient>();

        _ = await client.LogoutGameAsync(_apiUrl, request);
    }

    private static string ToWMCasinoPlayerId(string playerCode)
        => $"{AgentPrefix}_{playerCode}_{AgentSuffix}";
}
