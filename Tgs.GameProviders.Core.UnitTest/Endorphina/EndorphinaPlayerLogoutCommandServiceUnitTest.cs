using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.EndorphinaService.Client;
using Tgs.GameProviders.Core.Endorphina.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.Endorphina;

public class EndorphinaPlayerLogoutCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeBrandService = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameInterfaceEndorphinaClient = Substitute.For<IGameInterfaceEndorphinaClient>();

		var command = new EndorphinaPlayerLogoutCommand()
		{
			PlayerCode = "4ucmqplxb029"
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.Endorphina)).Returns(gameProviderId);

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
		_ = fakePlayerRepository.GetAsync(Arg.Is(command.PlayerCode)).Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
			 Arg.Is(gameProviderId),
			 Arg.Is(player.BrandId)).Returns(gameProviderConfigurationId);

		var configuration = new EndorphinaConfiguration()
		{
			ApiUrl = new("https://test.endorphina.network"),
			MerchantId = "1494",
			SecretKey = "9BC9E97FFCD74B16B87BD6FA82C8C2AF",
		};
		_ = fakeGameProviderRepository.GetEndorphinaConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

		var sut = new EndorphinaPlayerLogoutCommandService(
			fakeBrandService,
			fakeGameProviderRepository,
			fakeGameInterfaceEndorphinaClient,
			fakePlayerRepository);

		await sut.ExecuteAsync(command);

		_ = fakeGameInterfaceEndorphinaClient
			.Received(1)
			.PlayerLogoutAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			merchantId: Arg.Is(configuration.MerchantId),
			secretKey: Arg.Is(configuration.SecretKey),
			username: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
