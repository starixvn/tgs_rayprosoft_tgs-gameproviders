using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.InplayMatrix.Client;
using Tgs.GameInterface.InplayMatrix.Common;
using Tgs.GameInterface.InplayMatrix.Protos;
using Tgs.GameProviders.Core.InplayMatrixESports.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.InplayMatrixESports;

public class InplayMatrixESportsGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeClient = Substitute.For<IGameInterfaceInplayMatrixClient>();

		var query = new InplayMatrixESportsGameUrlQuery
		{
			PlayerCode = "4ucmqplxb029",
			GameId = 1,
			LoginRecordId = Guid.NewGuid().ToString(),
			Language = "zh-CN",
			PlatformType = PlatformType.Desktop,
			IPAddress = "127.0.0.1",
			BackUrl = new("https://dev-t1.rpgp.cc/")
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository
			.GetGameProviderIdAsync(
				gameProviderCode: Arg.Is(GameProviderType.InplayMatrix_ESports),
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
				playerCode: Arg.Is(query.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandRepository
			.GetBrandGameProviderConfigurationIdAsync(
				 gameProviderId: Arg.Is(gameProviderId),
				 brandId: Arg.Is(player.BrandId),
				 cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var configuration = new InplayMatrixESportsConfiguration()
		{
			ApiUrl = new("http://localhost/"),
			ReportUrl = new("http://localhost/"),
			MerchantCode = "apiKey",
			MerchantPrefix = "agentId",
			Password = "1q2w3e4r"
		};
		_ = fakeGameProviderRepository
			.GetInplayMatrixESportsConfigurationAsync(
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
			GameCode: "ESPORTSBULL",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.InplayMatrix_ESports,
			GameExternalId: "1",
			Type: GameType.ESports,
			PlatformType: GamePlatformType.DesktopAndMobile,
			Name: "ESPORTSBULL",
			SubGame: true,
			AvailableToBrand: false,
			HasTurnoverOnTieBet: true,
			Html5: true,
			Description: null,
			EndpointPath: null,
			RuleUrl: null,
			AvailableCurrencies: null,
			Localization: null,
			SubGameProviderCode: GameProviderType.InplayMatrix_ESports);

		_ = fakeGameRepository
			.GetGameListAsync(
				Arg.Is(gameProviderId),
				Arg.Is(GameType.ESports),
				Arg.Any<CancellationToken>()
				)
			.Returns(new GameInfo[] { game }.ToAsyncEnumerable());

		var gameUrlReply = new GameUrlReply
		{
			IsPlayerExist = true,
			GameUrl = "http://localhost/"
		};

		_ = fakeClient
			.GameUrlAsync(
				apiUrl: Arg.Is(configuration.ApiUrl),
				merchantCode: Arg.Is(configuration.MerchantCode),
				merchantPrefix: Arg.Is(configuration.MerchantPrefix),
				playerCode: Arg.Is(player.PlayerCode),
				productWallet: Arg.Is((int)InplayMatrixProductWallet.IM_eSports_Wallet),
				gameCode: Arg.Is(game.GameExternalId),
				ipAddress: Arg.Is(query.IPAddress),
				isMobile: Arg.Is(query.PlatformType == PlatformType.Mobile),
				language: Arg.Is(string.IsNullOrEmpty(cultureCode) ? "en" : cultureCode),
				Arg.Any<CancellationToken>())
			.Returns(gameUrlReply);

		var sut = new InplayMatrixESportsGameUrlGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(gameUrlReply.GameUrl, actual);
	}
}
