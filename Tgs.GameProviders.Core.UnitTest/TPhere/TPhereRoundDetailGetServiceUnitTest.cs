using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameInterface.TPhere.Client;
using Tgs.GameProviders.Core.TPhere.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;

using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.TPhere;

public class TPhereRoundDetailGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameBetRepository = Substitute.For<IGameBetRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeClient = Substitute.For<IGameInterfaceTPhereClient>();

		var query = new TPhereRoundDetailQuery
		{
			GameBetId = Guid.NewGuid(),
			Language = "en"
		};

		var gameBetInfo = new GameBetInfo(
			Id: query.GameBetId,
			GameProviderId: Guid.NewGuid(),
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 1,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "TPhere_mimi_mm-136817074",
			ExternalBetId: "TPhere_mimi_mm-136817074",
			ExternalCreatedOn: new DateTimeOffset(2024, 04, 18, 13, 14, 00, TimeSpan.FromHours(8)),
			ExternalClosedOn: new DateTimeOffset(2024, 04, 18, 13, 14, 00, TimeSpan.FromHours(8)),
			ExternalPayoffOn: new DateTimeOffset(2024, 04, 18, 13, 14, 00, TimeSpan.FromHours(8)),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Win,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.000000m,

			Bet: 3.000000m,
			ValidBet: 3.000000m,
			Settle: 6.000000m,
			Commission: 0.0m,
			Turnover: 3.000000m,
			WinLoss: 3.000000m,
			Jackpot: 0.0m,
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			ResetState: GameBets.Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["refNo"] = "TPhere_mimi_mm-136817074",
				["status"] = "W",
				["betTime"] = "2024-04-18 05:14:10",
				["winloss"] = 3.0,
				["currency"] = "CNY",
				["gameCode"] = "1302642",
				["gameName"] = "Super All Stars",
				["playerId"] = "4ufhtneqo2ia",
				["betAmount"] = 3.0,
				["statementDate"] = "2024-04-18",
				["effectiveStake"] = 3.0,
				["isTransferGame"] = false
			}.ToJsonString());
		_ = fakeGameBetRepository.GetAsync(
			gameBetId: Arg.Is(query.GameBetId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameBetInfo);

		var configuration = new TPhereConfiguration()
		{
			ApiUrl = new("http://trial-gp-api.TPheretest.com/"),
			ApiKey = "6309572cf90658e17847a7b021723106",
			ApiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
		};
		_ = fakeGameProviderRepository.GetTPhereConfiguration(
			gameProviderConfigurationId: Arg.Is(gameBetInfo.GameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var cultureCode = "EN";
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: Arg.Is(gameBetInfo.GameProviderId),
			cultureCode: Arg.Is(query.Language),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(cultureCode);

		var reply = "https:\\/\\/m-client.yahutech.com\\/GUI\\/slot_fortunegod\\/query\\/index.html";
		_ = fakeClient.TP2GameWagerDetailAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			apiKey: Arg.Is(configuration.ApiKey),
			apiToken: Arg.Is(configuration.ApiToken),
			externalWagerId: Arg.Is(gameBetInfo.ExternalBetId),
			externalCreatedOn: Arg.Is(gameBetInfo.ExternalCreatedOn),
			language: Arg.Is(cultureCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(reply);

		var sut = new TPhereRoundDetailGetService(
			fakeGameBetRepository,
			fakeGameProviderRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(reply, (actual.Data!.First()! as RoundDetailDataUrl)!.Url);
	}
}
