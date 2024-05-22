using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameInterface.SBOService.Protos;
using Tgs.GameProviders.Core.SBOTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.SBOTransfer;

public class SBOTransferPlayerFundOutGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameInterfaceSBOClient = Substitute.For<IGameInterfaceSBOClient>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandService = Substitute.For<IBrandRepository>();

		var query = new SBOTransferPlayerFundOutQuery
		{
			BrandCode = "GPI",
			PlayerCode = "4ucmqplxb029",
			CurrencyCode = "CNY",
			OrderId = "D1714154181705543700",
			Amount = 100m
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.SBOTransfer)).Returns(gameProviderId);

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

		var configuration = new SBOTransferConfiguration()
		{
			ApiUrl = new("http://localhost/"),
			CompanyKey = "companyKey",
			Agent = "Agent",
			Currency = "CNY",
			Min = 1,
			Max = 5000,
			MaxPerMatch = 20000,
			CasinoTableLimit = 1
		};
		_ = fakeGameProviderRepository.GetSBOTransferConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);


		var reply = new FundReply()
		{
			TxnId = query.OrderId,
			Resp = new JsonObject { ["TxnId"] = query.OrderId }.ToJsonString()
		};
		_ = fakeGameInterfaceSBOClient.FundOutAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			companyKey: Arg.Is(configuration.CompanyKey),
			agent: Arg.Is(configuration.Agent),
			playerCode: Arg.Is(player.PlayerCode),
			orderId: Arg.Is("D1714154181705543700"),
			amount: Arg.Is(query.Amount))
			.Returns(reply);

		var sut = new SBOTransferPlayerFundOutGetService(
			fakeGameInterfaceSBOClient,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandService);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(query.OrderId, actual.GameProviderTransactionId);
		Assert.Equal(reply.TxnId, actual.ExternalGameProviderTransactionId);
	}
}
