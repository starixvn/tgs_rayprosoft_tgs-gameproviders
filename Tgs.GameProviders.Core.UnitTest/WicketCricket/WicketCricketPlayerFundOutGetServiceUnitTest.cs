using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.WicketCricket.Client;
using Tgs.GameInterface.WicketCricket.Protos;
using Tgs.GameProviders.Core.WicketCricket.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.WicketCricket;

public class WicketCricketPlayerFundOutGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfaceWicketCricketClient>();

		var query = new WicketCricketPlayerFundOutQuery
		{
			BrandCode = "GPI",
			PlayerCode = "4ucmqplxb029",
			CurrencyCode = "CNY",
			Amount = 100M,
			OrderId = Guid.Parse("7f5a44a8-74bf-4927-8152-b9482a42af9c").ToString("N"),
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository
			.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.WicketCricket),
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

		var configuration = new WicketCricketConfiguration()
		{
			ApiUrl = new("http://localhost"),
			ExchangeApiUrl = new("http://localhost"),
			Website = "Website",
			Cert = "Cert",
			Agent = "Agent"
		};
		_ = fakeGameProviderRepository
			.GetWicketCricketConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var response = new FundReply()
		{
			Resp = "{\"amount\":1,\"loginId\":\"4ucmqplxb029\",\"userCode\":\"V020100001\",\"availableBalance\":1.00}"
		};

		_ = fakeClient
			.FundOutAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			website: Arg.Is(configuration.Website),
			cert: Arg.Is(configuration.Cert),
			playerCode: Arg.Is(player.PlayerCode),
			txid: Arg.Is(query.OrderId),
			amount: Arg.Is(query.Amount),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(response);

		var sut = new WicketCricketPlayerFundOutGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(query.OrderId, actual.GameProviderTransactionId);
	}
}
