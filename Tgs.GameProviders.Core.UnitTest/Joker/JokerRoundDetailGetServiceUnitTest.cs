using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameInterface.Joker.Client;
using Tgs.GameInterface.Joker.Protos;
using Tgs.GameProviders.Core.Joker.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.UnitTest.Joker;

public class JokerRoundDetailGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeRound = Substitute.For<IRoundRepository>();
        var fakeGameProvider = Substitute.For<IGameProviderRepository>();
        var fakeClient = Substitute.For<IGameInterfaceJokerClient>();

        var query = new JokerRoundDetailQuery
        {
            RoundId = Guid.NewGuid(),
            Language = "en-US"
        };

        var roundInfo = new RoundInfo(
            RoundId: query.RoundId,
            ExternalRoundId: "GN0022160907S",
            GameProviderId: Guid.Parse("629101fc-3b1c-4719-881b-dcf3c7263c15"),
            PlayerCode: "4ucmqplxb029",
            GameId: 1,
            CurrencyCode: "CNY",
            Status: GameRoundStatus.Closed,
            GameProviderConfigurationId: Guid.NewGuid(),
            WalletId: Guid.NewGuid(),
            CreatedOn: DateTime.Now.AddMinutes(-1),
            ClosedOn: DateTime.Now,
            UpdatedOn: DateTime.Now,
            BrandId: Guid.NewGuid(),
            ExternalCreatedOn: DateTime.Now.AddMinutes(-1),
            ExternalClosedOn: DateTime.Now,
            BetAmount: 20m,
            ValidBetAmount: 20m,
            SettleAmount: 45m,
            Commission: null,
            Turnover: 20m,
            CompanyWinLoss: -25m,
            TieBetAdjustment: null,
            ClientStatus: GameRoundClientStatus.Win,
            ReopenTurnoverAdjustment: null,
            ReopenedOn: null,
            EffectiveBettingPercentage: null,
            AdditionalData: JsonSerializer.Serialize(new { }));
        _ = fakeRound
            .GetAsync(
                roundId: Arg.Is(query.RoundId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(roundInfo);

        var configuration = new JokerConfiguration
        {
            ApiUrl = new("http://localhost/"),
            GameLaunchUrl = new Uri("http://localhost/"),
            AppId = "appId",
            SecretKey = "secretKey"
        };
        _ = fakeGameProvider
            .GetJokerConfigurationAsync(
                gameProviderConfigurationId: Arg.Is(roundInfo.GameProviderConfigurationId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(configuration);

        var gameProviderCultureCode = "en";
        _ = fakeGameProvider
            .GetGameProviderCultureCodeAsync(
                gameProviderId: Arg.Is(roundInfo.GameProviderId),
                cultureCode: Arg.Is(query.Language),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderCultureCode);

        var reply = new GameRecordDetailsReply
        {
            Url = "http://localhost/"
        };
        _ = fakeClient
            .GameRecordDetailsAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                appId: Arg.Is(configuration.AppId),
                secretKey: Arg.Is(configuration.SecretKey),
                externalRoundId: Arg.Is(roundInfo.ExternalRoundId),
                language: Arg.Is(gameProviderCultureCode))
            .Returns(reply.Url);

        var sut = new JokerRoundDetailGetService(
            fakeGameProvider,
            fakeClient,
            fakeRound);

        var actual = await sut
            .GetAsync(
               query: query)
            .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(reply.Url, (actual.Data!.Single() as RoundDetailDataUrl)!.Url.ToString());
    }
}
