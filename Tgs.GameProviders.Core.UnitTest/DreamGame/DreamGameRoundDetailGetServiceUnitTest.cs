using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameInterface.DreamGamingService.Protos;
using Tgs.GameProviders.Core.DreamGame;
using Tgs.GameProviders.Core.DreamGame.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.UnitTest.DreamGame;

public class DreamGameRoundDetailGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeService = Substitute.For<IDreamGameService>();
        var fakeClient = Substitute.For<IGameInterfaceDreamGamingClient>();
        var fakeRoundService = Substitute.For<IRoundRepository>();

        var query = new DreamGameRoundDetailQuery()
        {
            RoundId = Guid.NewGuid(),
            Language = "zh-cn"
        };

        var configuration = new DreamGameConfiguration
        {
            ServiceEndpointUrl = new("http://game-dg"),
            ApiUrl = new("http://api.dg99web.com"),
            VideoGameApiUrl = new("https://ag.dg66.info"),
            ApiKey = "974e8e9a072e4ad59a9e3f43a06ff9ae",
            AgentName = "DGTE0105CO",
            PlayerPassword = "TgsP@ssw0rd"
        };

        var roundInfo = new RoundInfo(
            query.RoundId,
            "123456",
            Guid.NewGuid(),
            "test",
            1,
            "CNY",
            GameRoundStatus.Closed,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.Now,
            DateTimeOffset.Now,
            DateTimeOffset.Now,
            Guid.NewGuid(),
            DateTimeOffset.Now,
            DateTimeOffset.Now,
            10m,
            10m,
            20m,
            0m,
            10m,
            -10m,
            null,
            GameRoundClientStatus.Win,
            null,
            null,
            null,
            "{}");

        _ = fakeRoundService.GetAsync(query.RoundId).Returns(ValueTask.FromResult<RoundInfo?>(roundInfo));
        _ = fakeService.GetSettingConfigurationAsync().Returns(configuration);
        var gameProviderCultureCode = "en";
        _ = fakeService.GetGameProviderLanguageCodeAsync(query.Language).Returns(ValueTask.FromResult(gameProviderCultureCode));
        var response = new GameHistoryDetailReply()
        {
            GameResult = "https://ag.dg66.info/ag/result/result.html?language=en&agentFix=3213&id=13540803433"
        };

        _ = fakeClient
            .GameHistoryDetailAsync(
                Arg.Is(configuration.VideoGameApiUrl),
                Arg.Is(configuration.AgentName),
                gameProviderCultureCode,
                roundInfo.ExternalRoundId,
                Arg.Any<CancellationToken>())
            .Returns(response);
        var sut = new DreamGameRoundDetailGetService(
            fakeClient,
            fakeRoundService,
            fakeService);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
    }
}
