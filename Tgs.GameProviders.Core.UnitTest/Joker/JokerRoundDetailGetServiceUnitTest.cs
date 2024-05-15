using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameInterface.Joker.Client;
using Tgs.GameInterface.Joker.Protos;
using Tgs.GameProviders.Core.Joker.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.Joker;

public class JokerRoundDetailGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameBetRepository = Substitute.For<IGameBetRepository>();
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakeClient = Substitute.For<IGameInterfaceJokerClient>();

		var query = new JokerRoundDetailQuery
		{
			GameBetId = Guid.NewGuid(),
			Language = "en-US"
		};

		var gameBetInfo = new GameBetInfo(
			Id: query.GameBetId,
			GameProviderId: Guid.NewGuid(),
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 2600271,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "qxrkpxczg5a94",
			ExternalBetId: "qxrkpxczg5a94",
			ExternalCreatedOn: new DateTimeOffset(2024, 05, 15, 08, 52, 08, TimeSpan.FromHours(8)),
			ExternalClosedOn: new DateTimeOffset(2024, 05, 15, 08, 52, 08, TimeSpan.FromHours(8)),
			ExternalPayoffOn: new DateTimeOffset(2024, 05, 15, 08, 52, 08, TimeSpan.FromHours(8)),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Loss,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.000000m,
			Bet: 0.100000m,
			ValidBet: 0.100000m,
			Settle: 0.000000m,
			Commission: 0.000000m,
			Turnover: 0.100000m,
			WinLoss: -0.100000m,
			Jackpot: 0.000000m,
			CreatedOn: new DateTimeOffset(2024, 05, 15, 08, 58, 08, TimeSpan.FromHours(8)),
			ClosedOn: new DateTimeOffset(2024, 05, 15, 08, 58, 08, TimeSpan.FromHours(8)),
			UpdatedOn: new DateTimeOffset(2024, 05, 15, 08, 58, 08, TimeSpan.FromHours(8)),
			ResetState: GameBets.Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: JsonNode.Parse("{\"Time\": \"2024-05-15T00:52:08.8960000+00:00\", \"Type\": \"Game\", \"AppID\": \"FJJV\", \"OCode\": \"qxrkpxczg5a94\", \"Amount\": 0.10, \"Result\": 0.00, \"Details\": null, \"RoundID\": \"G2ac41f0b9b67\", \"GameCode\": \"qmufydacbwbyg\", \"Username\": \"4UMK7QLKOK6E\", \"EndBalance\": 0.02, \"FreeAmount\": 0.00, \"Description\": \"G2ac41f0b9b67\", \"CurrencyCode\": \"HKD\", \"StartBalance\": 0.12, \"TransactionOCode\": \"xmbbqquhqeryc\"}")!);

		_ = fakeGameBetRepository.GetAsync(
			Arg.Is(query.GameBetId),
			Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var configuration = new JokerConfiguration
		{
			ApiUrl = new("http://localhost/"),
			GameLaunchUrl = new Uri("http://localhost/"),
			AppId = "appId",
			SecretKey = "secretKey"
		};
		_ = fakeGameProvider.GetJokerConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameBetInfo.GameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var gameProviderCultureCode = "en";
		_ = fakeGameProvider.GetGameProviderCultureCodeAsync(
			gameProviderId: Arg.Is(gameBetInfo.GameProviderId),
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
			externalRoundId: Arg.Is(gameBetInfo.ExternalBetId),
			language: Arg.Is(gameProviderCultureCode))
			.Returns(reply.Url);

		var sut = new JokerRoundDetailGetService(
			fakeGameBetRepository,
			fakeGameProvider,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(reply.Url, (actual.Data!.Single() as RoundDetailDataUrl)!.Url.ToString());
	}
}
