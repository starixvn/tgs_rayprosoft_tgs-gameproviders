using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameProviders.Core.AwcVenus.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.AwcVenus;

public class AwcVenusPlayerFundCheckingGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeBrand = Substitute.For<IBrandRepository>();
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakeClient = Substitute.For<IGameInterfaceAwcClient>();
		var fakePlayer = Substitute.For<IPlayerRepository>();

		var query = new AwcVenusPlayerFundCheckingQuery
		{
			BrandCode = "GPI",
			PlayerCode = "4ucmqplxb029",
			CurrencyCode = "CNY",
			OrderId = Guid.NewGuid().ToString("N")
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProvider
			.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.AWC_VENUS),
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
			.Returns(gameProviderConfigurationId);

		var configuration = new AwcVenusConfiguration
		{
			ApiUrl = new("https://tttint.onlinegames22.com/"),
			ApiRecordUrl = new("https://tttint.onlinegames22.com/"),
			AgentId = "testAgentId",
			Cert = "testCert",
			BetLimit = new JsonObject
			{
				["SV388"] = new JsonObject
				{
					["LIVE"] = new JsonObject
					{
						["maxbet"] = 2400,
						["minbet"] = 1,
						["mindraw"] = 1,
						["matchlimit"] = 4800,
						["maxdraw"] = 1200
					}
				}
			}
		};
		_ = fakeGameProvider
			.GetAwcVenusConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		_ = fakeClient
			.FundStatusAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			agentId: Arg.Is(configuration.AgentId),
			cert: Arg.Is(configuration.Cert),
			orderId: Arg.Is(query.OrderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new AwcVenusPlayerFundCheckingGetService(
			fakeBrand,
			fakeGameProvider,
			fakeClient,
			fakePlayer);

		var actual = await sut.GetAsync(query);

		Assert.True(actual);
	}
}
