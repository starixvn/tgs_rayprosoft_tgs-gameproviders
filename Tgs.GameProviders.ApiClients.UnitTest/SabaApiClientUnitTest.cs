using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Core.Saba;
using Tgs.GameProviders.Core.Saba.Models;

namespace Tgs.GameProviders.ApiClients.UnitTest;

[TestFixture]
public class SabaApiClientUnitTest
{
    private const string OperatorId = "ZhiFeng055";
    private const string VendorId = "zzdaa12nu1";
    private readonly Uri _apiUrl = new("http://t829tsa.bw6688.com");

    [Test]
    public async Task CreatePlayerAsync()
    {
        var playerCode = "4ucmqplxb029";
        var playerId = $"{playerCode}_test";

        var request = new SabaCreateMemberRequest
        {
            VendorId = VendorId,
            PlayerId = playerId,
            CurrencyCode = "20",
            OperatorId = OperatorId,
            OddsType = "1",
            Username = playerId
        };

        var client = Substitute.For<ISabaApiClient>();

        _ = await client.CreateMemberAsync(_apiUrl, request)
            .ConfigureAwait(false);
    }

    [Test]
    public async Task GetBetDetailByTransIdAsync()
    {
        var externalTransactionId = "113995497149";

        var request = new SabaGetRoundHistoryRequest
        {
            VendorId = VendorId,
            ExternalTransactionId = externalTransactionId
        };

        var client = Substitute.For<ISabaApiClient>();

        _ = await client.GetBetDetailByTransIdAsync(_apiUrl, request).ConfigureAwait(false);
    }

    [Test]
    public async Task GetGameUrlAsync()
    {
        var playerCode = "4ucmqplxb029";
        var playerId = $"{playerCode}_test";

        var request = new SabaGameUrlRequest
        {
            VendorId = VendorId,
            Platform = "1",
            PlayerId = playerId
        };

        var client = Substitute.For<ISabaApiClient>();

        _ = await client.GetGameUrlAsync(_apiUrl, request).ConfigureAwait(false);
    }

    [Test]
    public async Task KickUserAsync()
    {
        var playerCode = "4ucmqplxb029";
        var playerId = $"{playerCode}_test";

        var request = new SabaKickUserRequest
        {
            VendorId = VendorId,
            PlayerId = playerId
        };

        var client = Substitute.For<ISabaApiClient>();

        _ = await client.KickUserAsync(_apiUrl, request).ConfigureAwait(false);
    }
}
