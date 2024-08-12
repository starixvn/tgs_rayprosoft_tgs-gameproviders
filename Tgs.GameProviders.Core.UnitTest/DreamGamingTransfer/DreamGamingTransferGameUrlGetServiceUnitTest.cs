using System;
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

public class DreamGamingTransferGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeBrandService = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameInterfaceDreamGamingClient = Substitute.For<IGameInterfaceDreamGamingClient>();

		var query = new DreamGamingTransferGameUrlQuery
		{
			PlayerCode = "4ucmqplxb029"
		};

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
		_ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(player);

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

		var response = new GameUrlReply()
		{
			GameUrl = "https://login.maidaosha.com/pc-index/direct1.html?token=05b4cf398a30421bb2cbfb00f2a817b1&language=en"
		};

		_ = fakeGameInterfaceDreamGamingClient
			.GameUrlAsync(
				Arg.Is(configuration.ApiUrl),
				Arg.Is(configuration.ApiKey),
				Arg.Is(configuration.AgentName),
				Arg.Is(player.PlayerCode),
				"DGTFP@ssw0rd",
				false,
				"en",
				Arg.Any<CancellationToken>())
			.Returns(response);

		var sut = new DreamGamingTransferGameUrlGetService(
			fakeBrandService,
			fakeGameProviderRepository,
			fakeGameInterfaceDreamGamingClient,
			fakePlayerRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(response.GameUrl, actual);
	}
}
