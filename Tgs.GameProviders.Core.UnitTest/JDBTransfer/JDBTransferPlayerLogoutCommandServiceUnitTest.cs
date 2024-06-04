using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.JDBService.Client;
using Tgs.GameProviders.Core.JDBTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.JDBTransfer;

public class JDBTransferPlayerLogoutCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameInterfaceJDBClient = Substitute.For<IGameInterfaceJDBClient>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandService = Substitute.For<IBrandRepository>();

		var command = new JDBTransferPlayerLogoutCommand
		{
			PlayerCode = "4ucmqplxb029"
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.JDBTransfer)).Returns(gameProviderId);

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

		var configuration = new JDBTransferConfiguration()
		{
			ApiUrl = new("http://localhost/"),
			Domain = "Domain",
			Agent = "Agent",
			AesIV = "AesIV",
			AesKey = "AesKey"
		};
		_ = fakeGameProviderRepository.GetJDBTransferConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

		var sut = new JDBTransferPlayerLogoutCommandService(
			fakeGameInterfaceJDBClient,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandService);

		await sut.ExecuteAsync(command);

		_ = fakeGameInterfaceJDBClient
			.Received(1)
			.PlayerKickAsync(
				Arg.Is(configuration.ApiUrl),
				Arg.Is(configuration.Domain),
				Arg.Is(configuration.AesKey),
				Arg.Is(configuration.AesIV),
				Arg.Is(configuration.Agent),
				Arg.Is(command.PlayerCode));
	}
}
