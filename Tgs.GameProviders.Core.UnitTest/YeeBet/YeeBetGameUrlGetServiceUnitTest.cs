using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.YeeBet.Client;
using Tgs.GameProviders.Core.YeeBet.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.UnitTest.YeeBet;

public class YeeBetGameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeClient = Substitute.For<IGameInterfaceYeeBetClient>();
		var fakePlayerCreateService = Substitute.For<ICommandService<YeeBetPlayerCreateCommand>>();

		var query = new YeeBetGameUrlQuery
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
		_ = fakeGameProviderRepository
			.GetGameProviderIdAsync(
				gameProviderCode: Arg.Is(GameProviderType.YeeBet),
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

		var configuration = new YeeBetConfiguration()
		{
			ApiUrl = new("http://api.98kbus.top/"),
			AppId = "xtdA15OTVGEP",
			SecretKey = "4885366DF30D880224FF6AA46FBDE4F0",
		};
		_ = fakeGameProviderRepository
			.GetYeeBetConfigurationAsync(
				gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(configuration);

		var cultureCode = "1";
		_ = fakeGameProviderRepository
			.GetGameProviderCultureCodeAsync(
				gameProviderId: Arg.Is(gameProviderId),
				cultureCode: Arg.Is(query.Language),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(cultureCode);

		var gameUrl = "https://web.fcg666.net/index?Params=eyJpdiI6IndFU3VWOVdsV0MvWG1aNGlYS2RVdHc9PSIsInZhbHVlIjoiVTIzOE00ZUJ0YjJGaXVDbnRuajZUNDBXdFVXc0dMYlZKQllKbXN2Mm1aNTJqOXN1UGg4ZHJyS0ZBZnN6VTJsWlFpVUFTeE5kdFFCa3VCM3RDU00xNHMrRTNwd3pKOGdVRVFsSGphN1NTMXlKem15Mmc2aGMxTFdwNC9ERUU4ZFByczhGdTYzemNGNjAxb29mUHd3d3dnPT0iLCJtYWMiOiIzYWUwMWJjZDJlYzcxYzdmMmJjNmVjMzY0ZWM1YTU3ODc2OGQ4MjMzZjAxZTExYzkxMWE3MDVhZjkxODNhODA1In0%3D&languageID=2";

		_ = fakeClient
			.GameUrlAsync(
				apiUrl: Arg.Is(configuration.ApiUrl),
				appId: Arg.Is(configuration.AppId),
				secretKey: Arg.Is(configuration.SecretKey),
				playerCode: Arg.Is(player.PlayerCode),
				clientType: 1,
				languageId: Arg.Is(int.Parse(cultureCode)),
				Arg.Any<CancellationToken>())
			.Returns(gameUrl);

		var sut = new YeeBetGameUrlGetService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandRepository,
			fakeClient,
			fakePlayerCreateService);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(gameUrl, actual);
	}
}
