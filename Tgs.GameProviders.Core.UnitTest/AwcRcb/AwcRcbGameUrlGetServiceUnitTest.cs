using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameInterface.Awc.Protos;
using Tgs.GameProviders.Core.AwcRcb.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.AwcRcb;

public class AwcRcbGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Create_PlayerAsync()
	{
		var fakeBrand = Substitute.For<IBrandRepository>();
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakeClient = Substitute.For<IGameInterfaceAwcClient>();
		var fakePlayer = Substitute.For<IPlayerRepository>();

		var query = new AwcRcbGameUrlQuery
		{
			PlayerCode = "4ucmqplxb029",
			GameId = 1,
			LoginRecordId = Guid.NewGuid().ToString(),
			Language = "zh-CN",
			PlatformType = PlatformType.Desktop,
			IPAddress = "127.0.0.1",
			BackUrl = new("https://dev-t1.rpgp.cc/")
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProvider
			.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.AWC_RCB),
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

		var configuration = new AwcRcbConfiguration
		{
			ApiUrl = new("https://tttint.onlinegames22.com/"),
			ApiRecordUrl = new("https://tttint.onlinegames22.com/"),
			AgentId = "testAgentId",
			Cert = "testCert",
			BetLimit = new JsonObject
			{
				["HORSEBOOK"] = new JsonObject
				{
					["LIVE"] = new JsonObject
					{
						["minbet"] = 10,
						["maxbet"] = 3000,
						["maxBetSumPerHorse"] = 10000,
						["minorMinbet"] = 10,
						["minorMaxbet"] = 1000,
						["minorMaxBetSumPerHorse"] = 10000
					}
				}
			}
		};
		_ = fakeGameProvider
			.GetAwcRcbConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var gameProviderCultureCode = "en";
		_ = fakeGameProvider
			.GetGameProviderCultureCodeAsync(
			gameProviderId: Arg.Is(gameProviderId),
			cultureCode: Arg.Is(query.Language),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderCultureCode);

		var gameUrl = "http://www.google.com";
		var response = new GameUrlReply()
		{
			IsPlayerExist = false,
			GameUrl = string.Empty
		};
		_ = fakeClient.GameUrlByAwcRcbAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			agentId: Arg.Is(configuration.AgentId),
			cert: Arg.Is(configuration.Cert),
			playerCode: Arg.Is(player.PlayerCode),
			isMobileLogin: Arg.Is(query.PlatformType == PlatformType.Mobile),
			backUrl: Arg.Is(query.BackUrl.ToString()),
			betLimit: Arg.Is(configuration.BetLimit.ToJsonString()),
			language: Arg.Is(gameProviderCultureCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(response);

		var currency = "CNY";
		_ = fakeGameProvider
			.GetGameProviderCurrencyCodeAsync(
			gameProviderId: Arg.Is(gameProviderId),
			currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(currency);

		var sut = new AwcRcbGameUrlGetService(
			fakeBrand,
			fakeGameProvider,
			fakeClient,
			fakePlayer);

		_ = await Assert.ThrowsAsync<Exception>(() => sut.GetAsync(query: query).AsTask());

		_ = fakeClient
			.Received(1)
			.PlayerCreateByBetLimitAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			agentId: Arg.Is(configuration.AgentId),
			cert: Arg.Is(configuration.Cert),
			playerCode: Arg.Is(query.PlayerCode),
			userName: Arg.Is(query.PlayerCode),
			currency: Arg.Is(currency),
			betLimit: Arg.Is(configuration.BetLimit.ToJsonString()),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Normal_Process()
	{
		var fakeBrand = Substitute.For<IBrandRepository>();
		var fakeGameProvider = Substitute.For<IGameProviderRepository>();
		var fakeClient = Substitute.For<IGameInterfaceAwcClient>();
		var fakePlayer = Substitute.For<IPlayerRepository>();

		var query = new AwcRcbGameUrlQuery
		{
			PlayerCode = "4ucmqplxb029",
			GameId = 1,
			LoginRecordId = Guid.NewGuid().ToString(),
			Language = "zh-CN",
			PlatformType = PlatformType.Desktop,
			IPAddress = "127.0.0.1",
			BackUrl = new("https://dev-t1.rpgp.cc/")
		};

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProvider
			.GetGameProviderIdAsync(
			gameProviderCode: Arg.Is(GameProviderType.AWC_RCB),
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

		var configuration = new AwcRcbConfiguration
		{
			ApiUrl = new("https://tttint.onlinegames22.com/"),
			ApiRecordUrl = new("https://tttint.onlinegames22.com/"),
			AgentId = "testAgentId",
			Cert = "testCert",
			BetLimit = new JsonObject
			{
				["HORSEBOOK"] = new JsonObject
				{
					["LIVE"] = new JsonObject
					{
						["minbet"] = 10,
						["maxbet"] = 3000,
						["maxBetSumPerHorse"] = 10000,
						["minorMinbet"] = 10,
						["minorMaxbet"] = 1000,
						["minorMaxBetSumPerHorse"] = 10000
					}
				}
			}
		};
		_ = fakeGameProvider
			.GetAwcRcbConfigurationAsync(
			gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var gameProviderCultureCode = "en";
		_ = fakeGameProvider
			.GetGameProviderCultureCodeAsync(
			gameProviderId: Arg.Is(gameProviderId),
			cultureCode: Arg.Is(query.Language),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderCultureCode);

		var gameUrl = "http://www.google.com";
		var response = new GameUrlReply()
		{
			IsPlayerExist = true,
			GameUrl = gameUrl.ToString()
		};
		_ = fakeClient.GameUrlByAwcRcbAsync(
			apiUrl: Arg.Is(configuration.ApiUrl),
			agentId: Arg.Is(configuration.AgentId),
			cert: Arg.Is(configuration.Cert),
			playerCode: Arg.Is(player.PlayerCode),
			isMobileLogin: Arg.Is(query.PlatformType == PlatformType.Mobile),
			backUrl: Arg.Is(query.BackUrl.ToString()),
			betLimit: Arg.Is(configuration.BetLimit.ToJsonString()),
			language: Arg.Is(gameProviderCultureCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(response);

		var sut = new AwcRcbGameUrlGetService(
			fakeBrand,
			fakeGameProvider,
			fakeClient,
			fakePlayer);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(gameUrl, actual);
	}
}
