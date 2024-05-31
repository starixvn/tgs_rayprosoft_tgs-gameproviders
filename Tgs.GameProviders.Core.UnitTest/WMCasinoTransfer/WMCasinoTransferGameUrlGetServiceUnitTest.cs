using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.UnitTest.WMCasinoTransfer;

public class WMCasinoTransferGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeBrandService = Substitute.For<IBrandRepository>();
		var fakeCreatePlayerCommandService = Substitute.For<ICommandService<WMCasinoTransferCreatePlayerCommand>>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakeClient = Substitute.For<IGameInterfaceWMCasinoServiceClient>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();

		var gameUrl = new Uri("http://www.google.com");

		var configuration = new WMCasinoTransferConfiguration
		{
			ApiUrl = new Uri("https://api.a45.me/api/public/Gateway.php"),
			VendorId = "testVendorId",
			Signature = "testsignature"
		};

		var playerModel = new PlayerInfo(
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

		var gameInfo = new GameInfo(
			GameId: 1,
			GameCode: "test",
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: GameProviderType.WMCasinoTransfer,
			GameExternalId: "1",
			Type: GameType.LiveDealer,
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
			SubGameProviderCode: GameProviderType.WMCasinoTransfer);

		var query = new WMCasinoTransferGameUrlQuery
		{
			GameType = gameInfo.Type,
			GameId = gameInfo.GameId,
			BrandCode = "GPI",
			PlayerCode = "4ucmqplxb029",
			CurrencyCode = "CNY",
			LoginRecordId = Guid.NewGuid(),
			PlatformType = PlatformType.Desktop,
			Language = "0",
			IPAddress = "127.0.0.1",
			BackUrl = new("https://dev-t1.rpgp.cc/")
		};
		var gameProviderCultureCode = "0";
		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.WMCasinoTransfer)).Returns(gameInfo.GameProviderId);
		_ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
		_ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
			 Arg.Is(gameInfo.GameProviderId),
			 Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
		_ = fakeGameProviderRepository.GetWMCasinoTransferConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);
		_ = fakeGameProviderRepository.GetGameProviderCultureCodeAsync(
			Arg.Is(gameInfo.GameProviderId),
			Arg.Is(query.Language)).Returns(gameProviderCultureCode);
		_ = fakeGameRepository.GetAsync(Arg.Is(query.GameId.Value)).Returns(gameInfo);

		var response = new GameUrlReply()
		{
			Success = true,
			IsPlayerExist = true,
			GameUrl = gameUrl.ToString()
		};

		var user = $"wmtf_{playerModel.PlayerCode}_{playerModel.BrandCode}";

		_ = fakeClient.GameUrlAsync(
			 apiUrl: Arg.Is(configuration.ApiUrl),
			 vendorId: Arg.Is(configuration.VendorId),
			 signature: Arg.Is(configuration.Signature),
			 user: Arg.Is(user),
			 password: Arg.Any<string>(),
			 language: Arg.Any<int>()
			).Returns(response);

		var sut = new WMCasinoTransferGameUrlGetService(
			fakeBrandService,
			fakeCreatePlayerCommandService,
			fakeGameProviderRepository,
			fakeClient,
			fakePlayerRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(gameUrl, actual);
	}
}
