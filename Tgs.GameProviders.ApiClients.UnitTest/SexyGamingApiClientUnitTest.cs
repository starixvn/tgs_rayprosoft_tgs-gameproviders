using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.SexyGaming;
using Tgs.GameProviders.Core.SexyGaming.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest;

[TestFixture]
public class SexyGamingApiClientUnitTest
{
    private const string AgentId = "tgs";
    private const string Cert = "OFIHXL0mfK8AcohvyTs";
    private readonly Uri _apiUrl = new("https://tttint.onlinegames22.com/");

    private readonly Dictionary<int, string> _betLimitMapper =
        new()
        {
            { 280301, "CNY" },
            { 280302, "CNY" },
            { 280303, "CNY" },
            { 282202, "EUR" },
            { 282204, "EUR" },
            { 282205, "EUR" },
            { 280605, "IDR_1000" },
            { 280606, "IDR_1000" },
            { 280607, "IDR_1000" },
            { 281301, "PTI" }, //IDR
            { 281302, "PTI" }, //IDR
            { 300403, "JPY" },
            { 280801, "KRW" },
            { 280802, "KRW" },
            { 280803, "KRW" },
            { 280101, "MYR" },
            { 280102, "MYR" },
            { 280103, "MYR" },
            { 280908, "THB" },
            { 280909, "THB" },
            { 280910, "THB" },
            { 280201, "HKD" },
            { 280203, "HKD" },
            { 280204, "HKD" },
            { 280205, "HKD" },
            { 280206, "HKD" },
            { 280701, "USD" },
            { 280702, "USD" },
            { 280703, "USD" },
            { 280704, "USD" },
            { 280705, "USD" },
            { 281101, "VND_1000" },
            { 281102, "VND_1000" },
            { 281103, "VND_1000" },
            { 281104, "VND_1000" },
            { 281105, "VND_1000" },
            { 281401, "PTV" } //VND
        };

    [Test]
    public async Task CreatePlayerAsync()
    {
        var playerCode = "4ucmqplxb029";
        var currencyCode = "CNY";
        var cultureCode = "cn";
        var BetLime = _betLimitMapper.Where(x => x.Value == currencyCode).Select(x => x.Key).ToArray();

        var request = new SexyGamingCreateMemberRequest
        {
            AgentId = AgentId,
            Cert = Cert,
            UserId = playerCode,
            Currency = currencyCode,
            BetLimit = JsonSerializer.Serialize(new
            {
                SEXYBCRT = new
                {
                    LIVE = new
                    {
                        limitId = BetLime
                    }
                }
            }),
            Language = cultureCode
        };

        var client = Substitute.For<ISexyGamingApiClient>();

        _ = await client.CreatePlayerAsync(_apiUrl, request);
    }

    [Test]
    public async Task GetGamingGameUrlAsync()
    {
        var request = new SexyGamingGameUrlRequest()
        {
            AgentId = AgentId,
            Cert = Cert,
            UserId = "4ucmqplxb029",
            IsMobileLogin = false,
            Extension1 = JsonSerializer.Serialize(new { token = "rDLIWMPV7dgRstCYBzSY6EidJQh51TWgg8b7XgefrRt5QkjiCB5pwwNvRKSdGDPsc" })
        };

        var client = Substitute.For<ISexyGamingApiClient>();

        _ = await client.GetGamingGameUrlAsync(_apiUrl, request);
    }

    [Test]
    public async Task GetGamingLoginGameAsync()
    {
        var request = new SexyGamingLoginGameRequest()
        {
            AgentId = AgentId,
            Cert = Cert,
            UserId = "4ucmqplxb029",
            IsMobileLogin = false,
            Extension1 = JsonSerializer.Serialize(new { token = "rDLIWMPV7dgRstCYBzSY6EidJQh51TWgg8b7XgefrRt5QkjiCB5pwwNvRKSdGDPsc" })
        };

        var client = Substitute.For<ISexyGamingApiClient>();

        _ = await client.GetGamingLoginGameAsync(_apiUrl, request);
    }

    [Test]
    public async Task LogoutAsync()
    {
        var request = new SexyGamingLogoutRequest()
        {
            AgentId = AgentId,
            Cert = Cert,
            PlayerCode = "4ucmqplxb029",
        };

        var client = Substitute.For<ISexyGamingApiClient>();

        _ = await client.LogoutAsync(_apiUrl, request);
    }
}
