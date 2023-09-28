using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameInterface.DreamGamingService.Protos;
using Tgs.GameProviders.Core.DreamGamingTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.UnitTest.DreamGamingTransfer;

public class DreamGamingTransferRoundDetailGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeRoundService = Substitute.For<IRoundRepository>();
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeGameInterfaceDreamGamingClient = Substitute.For<IGameInterfaceDreamGamingClient>();

        var roundId = Guid.Parse("b764cf23-4dae-4b45-c613-08dae7cb5e69");
        var externalRoundId = "13540803433";

        var query = new DreamGamingTransferRoundDetailQuery
        {
            RoundId = roundId,
            Language = "en-us"
        };

        var gameProviderId = Guid.Parse("f52b4df6-4f71-e36e-9625-5065a4d11a28");

        var roundInfo = new RoundInfo(
            roundId,
            externalRoundId,
            gameProviderId,
            "4ucmqplxb029",
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

        _ = fakeRoundService.GetAsync(query.RoundId).Returns(roundInfo);

        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.DreamGamingTransfer)).Returns(gameProviderId);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");

        _ = fakePlayerRepository.GetAsync(Arg.Is("4ucmqplxb029")).Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(player.BrandId)).Returns(gameProviderConfigurationId);

        var configuration = new DreamGamingTransferConfiguration()
        {
            VideoGameApiUrl = new Uri("https://ag.dg66.info/"),
            ApiUrl = new("https://api.dg0.co/"),
            ApiKey = "d39da9da731b427b9349b80d9d97f2e4",
            AgentName = "DGTE0105IH",
        };
        _ = fakeGameProviderRepository.GetDreamGamingTransferConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var gameProviderCultureCode = "en";
        _ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(Arg.Is(gameProviderId), Arg.Is(query.Language)).Returns(gameProviderCultureCode);

        var response = new GameHistoryDetailReply()
        {
            GameResult = "https://ag.dg66.info/ag/result/result.html?language=en&agentFix=3213&id=13540803433"
        };

        _ = fakeGameInterfaceDreamGamingClient
            .GameHistoryDetailAsync(
                Arg.Is(configuration.VideoGameApiUrl),
                Arg.Is(configuration.AgentName),
                gameProviderCultureCode,
                roundInfo.ExternalRoundId,
                Arg.Any<CancellationToken>())
            .Returns(response);

        var sut = new DreamGamingTransferRoundDetailGetService(
            fakeRoundService,
            fakeGameInterfaceDreamGamingClient,
            fakeBrandService,
            fakeGameProviderRepository,
            fakePlayerRepository);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);

        _ = fakeGameInterfaceDreamGamingClient
            .Received(1)
            .GameHistoryDetailAsync(
            configuration.VideoGameApiUrl,
            configuration.AgentName,
            gameProviderCultureCode,
            externalRoundId);
    }
}
