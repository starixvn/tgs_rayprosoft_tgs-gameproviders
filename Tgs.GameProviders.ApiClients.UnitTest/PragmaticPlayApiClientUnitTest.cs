using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameProviders.Core.PragmaticPlay;
using Tgs.GameProviders.Core.PragmaticPlay.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest;

public class PragmaticPlayApiClientUnitTest
{
    private const string SecretKey = "testKey";
    private const string SecurityLogin = "tgs_tgs";
    private readonly Uri _apiUrl = new("https://api.prerelease-env.biz");

    [Fact]
    public async Task GetGameUrlAsync()
    {
        //var playerCode = "4ucmqplxb029";

        var request = new PragmaticPlayGameUrlRequest()
        {
            Token = "token",
            ExternalGameId = "vs50juicyfr",
            Language = "en",
            Platform = "WEB",
            LobbyUrl = "http://localhost:8081",
            SecureLogin = SecurityLogin
        };

        var client = Substitute.For<IPragmaticPlayApiClient>();

        _ = await client.GetGameUrlAsync(_apiUrl, SecretKey, request);
    }

    [Fact]
    public async Task GetRoundHistoryAsync()
    {
        var request = new PragmaticPlayOpenHistoryExtendedRequest
        {
            SecureLogin = SecurityLogin,
            PlayerCode = "4ued5l50heuf",
            RoundId = "2759382176",
            Language = "en"
        };

        var client = Substitute.For<IPragmaticPlayApiClient>();

        _ = await client.GetRoundHistoryAsync(_apiUrl, SecretKey, request);
    }

    [Fact]
    public async Task KickPlayerAsync()
    {
        var playerCode = "4ucmqplxb029";

        var request = new PragmaticPlayLogoutRequest
        {
            PlayerCode = playerCode,
            SecureLogin = SecurityLogin
        };

        var client = Substitute.For<IPragmaticPlayApiClient>();

        _ = await client.LogoutAsync(_apiUrl, SecretKey, request);
    }
}
