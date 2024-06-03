using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameInterface.Awc.Protos;
using Tgs.GameProviders.Core.AwcVenus.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.AwcVenus;

public class AwcVenusPlayerFundOutGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeBrand = Substitute.For<IBrandRepository>();
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakeClient = Substitute.For<IGameInterfaceAwcClient>();
		var fakePlayer = Substitute.For<IPlayerRepository>();

		var query = new AwcVenusPlayerFundOutQuery
		{
			PlayerCode = "4ucmqplxb029",
			OrderId = Guid.NewGuid().ToString("N"),
			Amount = 100m
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

		var gameProviderTransactionId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var reply = new FundReply()
		{
			OrderId = query.OrderId,
			GameProviderTransactionId = gameProviderTransactionId,
			CurrentBalance = 100m,
			Amount = query.Amount,
			Resp = new JsonObject
			{
				["transactionId"] = gameProviderTransactionId
			}.ToJsonString()
		};
		_ = fakeClient.FundOutAsync(
			Arg.Is(configuration.ApiUrl),
			Arg.Is(configuration.AgentId),
			Arg.Is(configuration.Cert),
			Arg.Is(query.PlayerCode),
			Arg.Is(query.OrderId),
			Arg.Is(query.Amount)).Returns(reply);

		var sut = new AwcVenusPlayerFundOutGetService(
			fakeBrand,
			fakeGameProvider,
			fakeClient,
			fakePlayer);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(query.OrderId, actual.OrderId);
		Assert.Equal(reply.GameProviderTransactionId, actual.ExternalOrderId);
	}
}
