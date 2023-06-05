using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.Joker.Client;
using Tgs.GameInterface.Joker.Protos;
using Tgs.GameProviders.Core.Joker;
using Tgs.GameProviders.Core.Joker.ApplicationServices;
using Tgs.GameProviders.Core.Joker.Models;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.Joker;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.UnitTest.Joker;

[TestFixture]
public class JokerGameRecordListGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeService = Substitute.For<IJokerService>();
        var fakeAgentMapper = Substitute.For<IJokerAgentMapper>();
        var fakeApiClient = Substitute.For<IGameInterfaceJokerClient>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();

        var agent = new JokerAgent
        {
            AgentId = "TGVY",
            CurrencyCode = "HKD",
            SecretKey = "s5ghrn77baoto",
            Status = true
        };

        var configuration = new JokerConfiguration
        {
            ServiceEndpointUrl = new("http://game-joker"),
            ApiUrl = new("https://w.api788.net"),
            GameLaunchUrl = new Uri("http://www.gwp6868.net/"),
            BrandMerchants = new JokerBrandMerchant[]
            {
                new()
                {
                    BrandCode = "Default",
                    Agents = new[] { agent }
                }
            }
        };

        var query = new JokerGameRecordListQuery(
            "Default",
            "TGVY",
            DateTimeOffset.Parse("2022-04-20T00:00:00+00:00"),
            DateTimeOffset.Parse("2022-04-21T00:00:00+00:00"),
            null);

        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);

        _ = fakeAgentMapper
            .GetAgentById(
                Arg.Is(configuration.BrandMerchants),
                Arg.Is(query.BrandCode),
                Arg.Is(query.AgentId))
            .Returns(agent);

        var reply = new GameRecordReply
        {
            NextId = "eyIyMDIyMDQyMDA5NTgiOnsiQ3JlYXRlZFRpbWUiOiI1NTdmOGNjMC1jMDRkLTExZWMtYmVlYy1kZjk4OWNhYjE5MGMifX0="
        };
        reply.Items.Add(new GameRecordInfo()
        {
            ExternalRoundId = "xmkeajuna89ne",
            ExternalGameId = "7rw3tfwz11kaw",
            PlayerCode = "4UF6K014JP5X",
            ExternalCreatedOn = DateTimeOffset.Parse("2022-04-20T09:58:08.9660000+08:00").ToTimestamp(),
            ExternalClosedOn = DateTimeOffset.Parse("2022-04-20T09:58:08.9660000+08:00").ToTimestamp(),
            Bet = 1.60m,
            Settle = 3.20m,
            WinLoss = 1.6m,
            AdditionalData = JsonSerializer.Serialize(new { ransactionId = "xmkeajuna89ne" })
        });
        _ = fakeApiClient
            .GameRecordAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                appId: Arg.Is(agent.AgentId),
                secretKey: Arg.Is(agent.SecretKey),
                startTime: Arg.Is(query.StartTime),
                endTime: Arg.Is(query.EndTime),
                nextId: Arg.Is(query.NextId ?? string.Empty),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns((reply));

        var gameProviderCode = GameProviderType.Joker;
        var gameProviderId = Guid.Parse("2ebfe118-b36b-40fa-85b5-78363470addc");
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var sut = new JokerGameRecordListGetService(
            client: fakeApiClient,
            gameProvider: fakeGameProvider,
            service: fakeService,
            agentMapper: fakeAgentMapper);

        var actual = await sut
            .GetAsync(
                query: query)
            .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(true, actual.Success);
        Assert.AreEqual(gameProviderCode, actual.Datas[0].GameProviderCode);
        Assert.AreEqual(gameProviderId, actual.Datas[0].GameProviderId);
        Assert.AreEqual(reply.Items.Single().ExternalRoundId, actual.Datas[0].ExternalRoundId);
        Assert.AreEqual(reply.Items.Single().ExternalGameId, actual.Datas[0].ExternalGameId);
        Assert.AreEqual(GameRoundStatus.Closed, actual.Datas[0].Status);
        Assert.AreEqual(reply.Items.Single().ExternalCreatedOn.ToDateTimeOffset(), actual.Datas[0].ExternalCreatedOn);
        Assert.AreEqual(reply.Items.Single().ExternalClosedOn.ToDateTimeOffset(), actual.Datas[0].ExternalClosedOn);
        Assert.AreEqual(reply.Items.Single().Bet.ToDecimal().GetValueOrDefault(0), actual.Datas[0].BetAmount);
        Assert.AreEqual(reply.Items.Single().Bet.ToDecimal().GetValueOrDefault(0), actual.Datas[0].ValidBetAmount);
        Assert.AreEqual(reply.Items.Single().Settle.ToDecimal().GetValueOrDefault(0), actual.Datas[0].SettleAmount);
        Assert.AreEqual(reply.Items.Single().WinLoss.ToDecimal().GetValueOrDefault(0), actual.Datas[0].PlayerWinLoss);

    }
}
