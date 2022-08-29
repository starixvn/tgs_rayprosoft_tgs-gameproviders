using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.KY;

namespace Tgs.GameProviders.ApiClients.UnitTest;

[TestFixture]
public class KYApiClientUnitTest
{
    private const string AgentId = "72412";
    private const string DesKey = "E2F3A416006F84F8";
    private const string Md5Key = "B563DF2A3DE58DFB";
    private readonly Uri _apiRecordUrl = new("https://wc1-record.hddv1.com/getRecordHandle");
    private readonly Uri _apiUrl = new("https://wc1-api.hddv1.com/channelHandle");

    [Test()]
    [TestCase("72412202110271546265054ucmqplxb029", true)]
    [TestCase("72412202110271546265055ucmqplxb029", false)]
    public async Task FundCheckingAsync(string orderId, bool status)
    {
        var client = Substitute.For<IKYApiClient>();

        _ = client.FundCheckingAsync(
            _apiUrl,
            DesKey,
            Md5Key,
            AgentId,
            orderId).Returns(ValueTask.FromResult<bool>(status));
        var response = await client.FundCheckingAsync(
            _apiUrl,
            DesKey,
            Md5Key,
            AgentId,
            orderId).ConfigureAwait(false);
        Assert.AreEqual(status, response);
    }

    [Test]
    public async Task FundInAsync()
    {
        var playerCode = "4ucmqplxb029";

        var client = Substitute.For<IKYApiClient>();

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

        var client = Substitute.For<IKYApiClient>();

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

        var client = Substitute.For<IKYApiClient>();

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

        var client = Substitute.For<IKYApiClient>();

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

        var client = Substitute.For<IKYApiClient>();

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

        var client = Substitute.For<IKYApiClient>();

        await client.KickAsync(
            _apiUrl,
            DesKey,
            Md5Key,
            AgentId,
            playerCode).ConfigureAwait(false);
    }
}
