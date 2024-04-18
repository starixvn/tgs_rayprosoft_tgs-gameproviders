using System;
using System.Text.Json.Nodes;

using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Funky.Client;
using Tgs.GameInterface.Funky.Protos;
using Tgs.GameProviders.Core.Funky.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.Funky;

public class FunkyPlayerFundOutGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfaceFunkyClient>();

		var query = new FunkyPlayerFundOutQuery
		{
			BrandCode = "GPI",
			PlayerCode = "4ucmqplxb029",
			CurrencyCode = "CNY",
			OrderId = Guid.NewGuid().ToString("N"),
			Amount = 100
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.Funky),
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
		_ = fakePlayerRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: Arg.Is(gameProviderId),
			brandId: Arg.Is(player.BrandId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var configuration = new FunkyConfiguration()
		{
			ApiUrl = new("http://trial-gp-api.funkytest.com/"),
			ApiReportUrl = new("http://trial-gp-api-report.funkytest.com/"),
			Authentication = "c69d57e5-93e7-488c-90a4-e20876b495bc",
			UserAgent = "109-PG"
		};
		_ = fakeGameProviderRepository.GetFunkyConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var currency = "CNY";
		_ = fakeGameProviderRepository.GetGameProviderCurrencyCodeAsync(
			gameProviderId: Arg.Is(gameProviderId),
			currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(currency);

		var reply = new FundReply
		{
			TxSerialId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
			TxId = query.OrderId,
			Balance = 100.0m,
			Res = new JsonObject { }.ToJsonString()
		};
		_ = fakeClient.FundOutAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			userAgent: Arg.Is(configuration.UserAgent),
			authentication: Arg.Is(configuration.Authentication),
			playerCode: Arg.Is(player.PlayerCode),
			currency: Arg.Is(currency),
			orderId: Arg.Is(query.OrderId),
			amount: Arg.Is(query.Amount),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(reply);

		var sut = new FunkyPlayerFundOutGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.Equal(actual.OrderId, reply.TxSerialId.ToString());
	}
}
