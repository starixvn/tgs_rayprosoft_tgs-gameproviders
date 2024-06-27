using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameInterface.DreamGamingService.Protos;
using Tgs.GameProviders.Core.DreamGamingTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.DreamGamingTransfer;

public class DreamGamingTransferPlayerLogoutCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeBrandService = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameInterfaceDreamGamingClient = Substitute.For<IGameInterfaceDreamGamingClient>();

		var command = new DreamGamingTransferPlayerLogoutCommand() { PlayerCode = "4ucmqplxb029" };

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.DreamGamingTransfer)).Returns(gameProviderId);

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

		var configuration = new DreamGamingTransferConfiguration()
		{
			ApiUrl = new("https://api.dg0.co/"),
			ApiKey = "d39da9da731b427b9349b80d9d97f2e4",
			AgentName = "DGTE0105IH",
		};
		_ = fakeGameProviderRepository.GetDreamGamingTransferConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

		var playerOnlines = new PlayerOnline[]
		{
			new() {
				PlayerCode = "4ucmqplxb029",
				MemberId = 94030972,
			}
		};

		_ = fakeGameInterfaceDreamGamingClient.PlayerOnlineListAsync(
			configuration.ApiUrl,
			configuration.ApiKey,
			configuration.AgentName)
			.Returns(playerOnlines.ToAsyncEnumerable());

		var sut = new DreamGamingTransferPlayerLogoutCommandService(
			fakeBrandService,
			fakeGameProviderRepository,
			fakeGameInterfaceDreamGamingClient,
			fakePlayerRepository);

		await sut.ExecuteAsync(command);

		_ = fakeGameInterfaceDreamGamingClient
			.Received(1)
			.PlayerLogoutAsync(
				Arg.Is(configuration.ApiUrl),
				Arg.Is(configuration.ApiKey),
				Arg.Is(configuration.AgentName),
				Arg.Is(playerOnlines.FirstOrDefault(x => x.PlayerCode == player.PlayerCode)!.MemberId),
				Arg.Any<CancellationToken>());
	}
}
