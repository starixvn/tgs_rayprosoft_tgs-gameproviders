using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.PragmaticPlay.Client;
using Tgs.GameInterface.PragmaticPlay.Protos;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.PragmaticPlayTransfer;

public class PragmaticPlayTransferRoundDetailGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameBetRepository = Substitute.For<IGameBetRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeClient = Substitute.For<IGameInterfacePragmaticPlayClient>();

		var query = new PragmaticPlayTransferRoundDetailQuery
		{
			GameBetId = Guid.NewGuid(),
			Language = "zh-CN",
		};

		var gameBetInfo = new GameBetInfo(
			Id: query.GameBetId,
			GameProviderId: Guid.NewGuid(),
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 2700588,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "40837670023002",
			ExternalBetId: "40837670023002",
			ExternalCreatedOn: new DateTimeOffset(2024, 05, 15, 07, 36, 54, TimeSpan.FromHours(8)),
			ExternalClosedOn: new DateTimeOffset(2024, 05, 15, 07, 36, 54, TimeSpan.FromHours(8)),
			ExternalPayoffOn: new DateTimeOffset(2024, 05, 15, 07, 36, 54, TimeSpan.FromHours(8)),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Loss,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.000000m,
			Bet: 2.000000m,
			ValidBet: 2.000000m,
			Settle: 0.500000m,
			Commission: 0.000000m,
			Turnover: 2.000000m,
			WinLoss: -1.500000m,
			Jackpot: 0.000000m,
			CreatedOn: new DateTimeOffset(2024, 05, 15, 07, 37, 06, TimeSpan.FromHours(8)),
			ClosedOn: new DateTimeOffset(2024, 05, 15, 07, 37, 06, TimeSpan.FromHours(8)),
			UpdatedOn: new DateTimeOffset(2024, 05, 15, 07, 37, 06, TimeSpan.FromHours(8)),
			ResetState: GameBets.Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: JsonNode.Parse("{\"Bet\": 2.00, \"Win\": 0.50, \"Type\": \"R\", \"GameID\": \"vs20olympx\", \"Status\": \"C\", \"EndDate\": \"2024-05-14 23:36:54\", \"Jackpot\": 0.00, \"Currency\": \"HKD\", \"PlayerID\": 95608387, \"StartDate\": \"2024-05-14 23:36:50\", \"ExtPlayerID\": \"4umtpoil5l4o\", \"PlaySessionID\": \"40837670023002\", \"ParentSessionID\": null}")!);

		_ = fakeGameBetRepository.GetAsync(
			Arg.Is(query.GameBetId),
			Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var configuration = new PragmaticPlayTransferConfiguration()
		{
			ApiUrl = new("https://localhost/"),
			SecureLogin = "secure_login",
			SecretKey = "secret_key"
		};
		_ = fakeGameProviderRepository.GetPragmaticPlayTransferConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameBetInfo.GameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var cultureCode = "en";
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: Arg.Is(gameBetInfo.GameProviderId),
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
		_ = fakeGameRepository.GetAsync(
			gameId: Arg.Is(gameBetInfo.GameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(game);

		var gameRecordDetailReply = new GameRecordDetailReply
		{
			Url = "https://localhost/"
		};
		_ = fakeClient.GameRecordDetailAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			secureLogin: Arg.Is(configuration.SecureLogin),
			secretKey: Arg.Is(configuration.SecretKey),
			playerCode: Arg.Is(gameBetInfo.PlayerCode),
			externalRoundId: Arg.Is(gameBetInfo.ExternalBetId),
			externalGameId: Arg.Is(game.GameExternalId),
			language: Arg.Is(cultureCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameRecordDetailReply);

		var sut = new PragmaticPlayTransferRoundDetailGetService(
			fakeGameBetRepository,
			fakeGameProviderRepository,
			fakeGameRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
	}
}
