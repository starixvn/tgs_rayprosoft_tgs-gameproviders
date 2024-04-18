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
		var fakeRoundRepository = Substitute.For<IRoundRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeClient = Substitute.For<IGameInterfaceTPhereClient>();

		var query = new TPhereRoundDetailQuery
		{
			RoundId = Guid.NewGuid(),
			Language = "en"
		};

		var roundInfo = new RoundInfo(
			RoundId: query.RoundId,
			ExternalRoundId: "TPhere_mimi_mm-136817074",
			GameProviderId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			GameId: 1,
			CurrencyCode: "CNY",
			Status: GameRounds.Common.GameRoundStatus.Closed,
			GameProviderConfigurationId: Guid.NewGuid(),
			WalletId: Guid.NewGuid(),
			CreatedOn: DateTimeOffset.UtcNow,
			ClosedOn: DateTimeOffset.UtcNow,
			UpdatedOn: DateTimeOffset.UtcNow,
			BrandId: Guid.NewGuid(),
			ExternalCreatedOn: new DateTimeOffset(2024, 04, 18, 13, 14, 00, TimeSpan.FromHours(8)),
			ExternalClosedOn: new DateTimeOffset(2024, 04, 18, 13, 14, 00, TimeSpan.FromHours(8)),
			BetAmount: 3.000000m,
			ValidBetAmount: 3.000000m,
			SettleAmount: 6.000000m,
			Commission: 0.0m,
			Turnover: 3.000000m,
			CompanyWinLoss: -3.000000m,
			TieBetAdjustment: 0.0m,
			ClientStatus: GameRounds.Common.GameRoundClientStatus.Win,
			ReopenTurnoverAdjustment: null,
			ReopenedOn: null,
			EffectiveBettingPercentage: null,
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
		_ = fakeRoundRepository.GetAsync(
			 roundId: Arg.Is(query.RoundId),
			 cancellationToken: Arg.Any<CancellationToken>())
			.Returns(roundInfo);

		var configuration = new TPhereConfiguration()
		{
			ApiUrl = new("http://trial-gp-api.TPheretest.com/"),
			ApiKey = "6309572cf90658e17847a7b021723106",
			ApiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
		};
		_ = fakeGameProviderRepository.GetTPhereConfiguration(
			gameProviderConfigurationId: Arg.Is(roundInfo.GameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var cultureCode = "EN";
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: Arg.Is(roundInfo.GameProviderId),
			cultureCode: Arg.Is(query.Language),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(cultureCode);

		var reply = "https:\\/\\/m-client.yahutech.com\\/GUI\\/slot_fortunegod\\/query\\/index.html";
		_ = fakeClient.TP2GameWagerDetailAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			apiKey: Arg.Is(configuration.ApiKey),
			apiToken: Arg.Is(configuration.ApiToken),
			externalWagerId: Arg.Is(roundInfo.ExternalRoundId),
			externalCreatedOn: Arg.Is(roundInfo.ExternalCreatedOn!.Value),
			language: Arg.Is(cultureCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(reply);

		var sut = new TPhereRoundDetailGetService(
			fakeRoundRepository,
			fakeGameProviderRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(reply, (actual.Data!.First()! as RoundDetailDataUrl)!.Url);
	}
}
