using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.PragmaticPlay.Client;
using Tgs.GameInterface.PragmaticPlay.Protos;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.UnitTest.PragmaticPlayTransfer;

internal class PragmaticPlayTransferRoundDetailGetServiceUnitTest
{
    [Test]
    public async ValueTask Normal_Process()
    {
        var fakeRoundService = Substitute.For<IRoundRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandRepository = Substitute.For<IBrandRepository>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeClient = Substitute.For<IGameInterfacePragmaticPlayClient>();

        var query = new PragmaticPlayTransferRoundDetailQuery
        {
            RoundId = Guid.NewGuid(),
            Language = "zh-CN",
        };

        var roundInfo = new RoundInfo(
            RoundId: query.RoundId,
            ExternalRoundId: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
            GameProviderId: Guid.NewGuid(),
            PlayerCode: "4ucmqplxb029",
            GameId: 1,
            CurrencyCode: "CNY",
            Status: GameRoundStatus.Closed,
            GameProviderConfigurationId: Guid.NewGuid(),
            WalletId: Guid.NewGuid(),
            CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-10),
            ClosedOn: DateTimeOffset.UtcNow,
            UpdatedOn: DateTimeOffset.UtcNow,
            BrandId: Guid.NewGuid(),
            ExternalCreatedOn: DateTimeOffset.UtcNow.AddMinutes(-10),
            ExternalClosedOn: DateTimeOffset.UtcNow,
            BetAmount: 1m,
            ValidBetAmount: 1m,
            SettleAmount: 0,
            Commission: 0,
            Turnover: 1m,
            CompanyWinLoss: 1m,
            TieBetAdjustment: null,
            ClientStatus: GameRoundClientStatus.Loss,
            ReopenTurnoverAdjustment: null,
            ReopenedOn: null,
            EffectiveBettingPercentage: null,
            AdditionalData: JsonSerializer.Serialize(new
            {
                ExternalRoundId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
            }));
        _ = fakeRoundService
            .GetAsync(
                Arg.Is(query.RoundId))
            .Returns(roundInfo);

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository
            .GetGameProviderIdAsync(
                gameProviderCode: Arg.Is(GameProviderType.PragmaticPlayTransfer),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakePlayerRepository
            .GetAsync(
                playerCode: Arg.Is(roundInfo.PlayerCode),
                    cancellationToken: Arg.Any<CancellationToken>())
                .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandRepository
            .GetBrandGameProviderConfigurationIdAsync(
                 gameProviderId: Arg.Is(gameProviderId),
                     brandId: Arg.Is(player.BrandId),
                     cancellationToken: Arg.Any<CancellationToken>())
                .Returns(gameProviderConfigurationId);

        var configuration = new PragmaticPlayTransferConfiguration()
        {
            ApiUrl = new("https://localhost/"),
            SecureLogin = "secure_login",
            SecretKey = "secret_key"
        };
        _ = fakeGameProviderRepository
            .GetPragmaticPlayTransferConfigurationAsync(
                gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
                    cancellationToken: Arg.Any<CancellationToken>())
                .Returns(configuration);

        var cultureCode = "en";
        _ = fakeGameProviderRepository
            .GetGameProviderCultureCodeAsync(
                gameProviderId: Arg.Is(gameProviderId),
                    cultureCode: Arg.Is(query.Language),
                    cancellationToken: Arg.Any<CancellationToken>())
                .Returns(cultureCode);

        var game = new GameInfo(
            GameId: 1,
            GameCode: "test",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.PragmaticPlayTransfer,
            GameExternalId: "1",
            Type: GameType.LiveDealerLobby,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "test",
            SubGame: true,
            AvailableToBrand: false,
            HasTurnoverOnTieBet: true,
            Html5: true,
            Description: null,
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: GameProviderType.PragmaticPlayTransfer);
        _ = fakeGameRepository
            .GetAsync(
                Arg.Is(roundInfo.GameId),
                    cancellationToken: Arg.Any<CancellationToken>())
                .Returns(game);

        var gameRecordDetailReply = new GameRecordDetailReply
        {
            Url = "https://localhost/"
        };
        _ = fakeClient
            .GameRecordDetailAsync(
                apiUrl: Arg.Is(configuration.ApiUrl),
                secureLogin: Arg.Is(configuration.SecureLogin),
                secretKey: Arg.Is(configuration.SecretKey),
                playerCode: Arg.Is(roundInfo.PlayerCode),
                externalRoundId: Arg.Is(roundInfo.ExternalRoundId),
                externalGameId: Arg.Is(game.GameExternalId),
                language: Arg.Is(cultureCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameRecordDetailReply);

        var sut = new PragmaticPlayTransferRoundDetailGetService(
            fakeRoundService,
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandRepository,
            fakeGameRepository,
            fakeClient);

        var actual = await sut
            .GetAsync(
                query: query)
            .ConfigureAwait(false);

        Assert.NotNull(actual);
    }
}
