using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.GameInterface.TFGamingService.Protos;
using Tgs.GameProviders.Core.TFGamingTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.TFGamingTransfer;

public class TFGamingTransferPlayerFundOutGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeBrandService = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeGameInterfaceTFGamingClient = Substitute.For<IGameInterfaceTFGamingClient>();

		var query = new TFGamingTransferPlayerFundOutQuery
		{
			PlayerCode = "4ucmqplxb029",
			Amount = 16800M,
			OrderId = Guid.NewGuid().ToString(),
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.TFGamingTransfer)).Returns(gameProviderId);

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

		var configuration = new TFGamingTransferConfiguration()
		{
			ApiUrl = new("https://cn.ucfclive.com"),
			PrivateToken = "00022362f50e16fa474c80fc24da29793420aff6ce4b5d05a4a7dc52a261078ce72ceef10e7dc28f7bcfe89fe4922624fe72992e1f2f4f78360e4360970ce1b3f2e4567d01b071190c9051058fc1c14202f4ea117497c29fdbe8c9eddfb0e4fa6a3efe3bcbc7cfd8665c5b311e4a7065b8ad37a1a30d64919d8a3f2ff82c2ead23eb",
			OperatorId = "3871"
		};
		_ = fakeGameProviderRepository.GetTFGamingTransferConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

		var response = new FundReply()
		{
			Balance = 16800M,
			RefTransactionCode = query.OrderId,
			TransactionId = "2263093",
		};

		_ = fakeGameInterfaceTFGamingClient
			.FundOutAsync(
				Arg.Is(configuration.ApiUrl),
				Arg.Is(configuration.OperatorId),
				Arg.Is(configuration.PrivateToken),
				Arg.Is(player.PlayerCode),
				Arg.Is(query.OrderId),
				Arg.Is(query.Amount),
				Arg.Any<CancellationToken>())
			.Returns(response);

		var sut = new TFGamingTransferPlayerFundOutGetService(
			fakeBrandService,
			fakeGameProviderRepository,
			fakeGameInterfaceTFGamingClient,
			fakePlayerRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(query.OrderId, actual.OrderId);
		Assert.Equal(response.TransactionId, actual.ExternalOrderId);
	}
}
