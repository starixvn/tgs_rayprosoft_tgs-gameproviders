using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.WGFmGaming;

public class WGFmGamingPlayerFundInGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameInterfaceWGFmGamingClient = Substitute.For<IGameInterfaceWGFmGamingClient>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();

		var query = new WGFmGamingPlayerFundInQuery
		{
			PlayerCode = "test",
			OrderId = Guid.NewGuid().ToString("N"),
			Amount = 1
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.WGFmGaming)).Returns(ValueTask.FromResult(gameProviderId));

		var player = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "GPI",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "GPI",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "test",
			Username: "test",
			DefaultWalletId: Guid.NewGuid(),
			DefaultWalletCurrencyCode: "CNY");
		_ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandRepository.GetBrandGameProviderConfigurationIdAsync(
			Arg.Is(gameProviderId),
			Arg.Is(player.BrandId)).Returns(ValueTask.FromResult(gameProviderConfigurationId));

		var configuration = new WGFmGamingConfiguration
		{
			ApiUrl = new("http://localhost/"),
			PartnerCode = "PartnerCode",
			PartnerKey = "PartnerKey",
			IV = "IV"
		};
		_ = fakeGameProviderRepository.GetWGFmGamingConfigurationAsync(
			Arg.Is(gameProviderConfigurationId)
			).Returns(ValueTask.FromResult(configuration));

		_ = fakeGameInterfaceWGFmGamingClient
			.FundInAsync(
				Arg.Is(configuration.ApiUrl),
				Arg.Is(configuration.PartnerCode),
				Arg.Is(configuration.PartnerKey),
				Arg.Is(configuration.IV),
				Arg.Is(query.PlayerCode),
				Arg.Is(query.OrderId),
				Arg.Is(query.Amount));

		var sut = new WGFmGamingPlayerFundInGetService(
			fakeGameInterfaceWGFmGamingClient,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(query.OrderId, actual.OrderId);
	}
}
