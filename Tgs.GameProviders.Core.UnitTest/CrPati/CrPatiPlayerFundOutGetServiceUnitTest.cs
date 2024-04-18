using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.CrPati.Client;
using Tgs.GameInterface.CrPati.Protos;
using Tgs.GameProviders.Core.CrPati.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.CrPati;

public class CrPatiPlayerFundOutGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var ipAddressOptions = Options.Create(new IPAddressOptions
		{
			IPAddress = "127.0.0.1"
		});
		var fakeClient = Substitute.For<IGameInterfaceCrPatiClient>();
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakePlayer = Substitute.For<IPlayerRepository>();
		var fakeBrand = Substitute.For<IBrandRepository>();

		var query = new CrPatiPlayerFundOutQuery
		{
			BrandCode = "brandcode",
			PlayerCode = "test",
			CurrencyCode = "CNY",
			OrderId = Guid.NewGuid().ToString("N"),
			Amount = 1
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProvider.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.CrPati),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(gameProviderId));

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
			DefaultWalletCurrencyCode: "HKD");
		_ = fakePlayer
			.GetAsync(
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(player);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrand
			.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: Arg.Is(gameProviderId),
			brandId: Arg.Is(player.BrandId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(gameProviderConfigurationId));

		var configuration = new CrPatiConfiguration
		{
			ApiUrl = new("http://localhost/"),
			ApiKey = "apiKey",
			ApiSecret = "apiSecret"
		};
		_ = fakeGameProvider
			.GetCrPatiConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>()).
			Returns(ValueTask.FromResult(configuration));

		var reply = new FundReply
		{
			ApiOrderNo = "2311081542127340855",
			Balance = 1000.0m,
			Resp = new JsonObject
			{
				["IsSuccess"] = true,
				["Code"] = 0,
				["Message"] = "执行成功",
				["Data"] = new JsonObject
				{
					["APIOrderNo"] = "2311081542127340855",
					["Balance"] = 1000.0
				}
			}.ToJsonString()
		};
		_ = fakeClient
			.FundOutAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			apiKey: Arg.Is(configuration.ApiKey),
			apiSecret: Arg.Is(configuration.ApiSecret),
			playerCode: Arg.Is(player.PlayerCode),
			ipAddress: Arg.Is(ipAddressOptions.Value.IPAddress),
			orderId: Arg.Is(query.OrderId),
			amount: Arg.Is(query.Amount),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(reply);

		var sut = new CrPatiPlayerFundOutGetService(
			ipAddressOptions,
			fakeGameProvider,
			fakePlayer,
			fakeBrand,
			fakeClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(reply.ApiOrderNo, actual.OrderId);
	}
}
