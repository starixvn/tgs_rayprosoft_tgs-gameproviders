using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.LY;

namespace Tgs.GameProviders.ApiClients.UnitTest;

[TestFixture]
public class LYApiClientUnitTest
{
    private const string AgentId = "200995";
    private const string DesKey = "706EB65DD6E6DF4C";
    private const string Md5Key = "3E00054B89B31B7A";
    private readonly Uri _apiRecordUrl = new("https://wc1-record.wwdlw78.com/getRecordHandle");
    private readonly Uri _apiUrl = new("https://wc1-api.wwdlw78.com/channelHandle");

    [Test]
    public async Task FundInAsync()
    {
        var playerCode = "4ucmqplxb029";

        var client = Substitute.For<ILYApiClient>();

        _ = await client.FundInAsync(
            _apiUrl,
            DesKey,
            Md5Key,
            AgentId,
            playerCode,
            10).ConfigureAwait(false);
    }

    [Test]
    public async Task FundOutAsync()
    {
        var playerCode = "4ucmqplxb029";

        var client = Substitute.For<ILYApiClient>();

        _ = await client.FundOutAsync(
            _apiUrl,
            DesKey,
            Md5Key,
            AgentId,
            playerCode,
            10).ConfigureAwait(false);
    }

    [Test]
    public async Task GetBalanceAsync()
    {
        var playerCode = "4ucmqplxb029";

        var client = Substitute.For<ILYApiClient>();

        _ = await client.GetBalanceAsync(
            _apiUrl,
            DesKey,
            Md5Key,
            AgentId,
            playerCode).ConfigureAwait(false);
    }

    [Test]
    public async Task GetGameRecordAsync()
    {
        var dateTimeOffset = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
        var startTime = dateTimeOffset.AddHours(-1).ToUnixTimeMilliseconds();//1634537632446
        var endTime = dateTimeOffset.ToUnixTimeMilliseconds();//1634541232446

        var client = Substitute.For<ILYApiClient>();

        _ = await client.GetGameRecordAsync(
            _apiRecordUrl,
            DesKey,
            Md5Key,
            AgentId,
            startTime,
            endTime).ConfigureAwait(false);
    }

    [Test]
    public async Task GetGameUrlAsync()
    {
        var playerCode = "4ucmqplxb029";

        var client = Substitute.For<ILYApiClient>();

        _ = await client.GetGameUrlAsync(
            _apiUrl,
            DesKey,
            Md5Key,
            AgentId,
            playerCode,
            "220.132.206.70",
            "GPI",
            510).ConfigureAwait(false);
    }

    [Test]
    public async Task KickAsync()
    {
        var playerCode = "4ucmqplxb029";

        var client = Substitute.For<ILYApiClient>();

        await client.KickAsync(
            _apiUrl,
            DesKey,
            Md5Key,
            AgentId,
            playerCode).ConfigureAwait(false);
    }
}
