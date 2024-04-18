using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Kaiyuan.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests;

public partial class GameProvidersClientUnitTest
{
	[Fact]
	public async Task Game_Url()
	{
		var gameProviderCode = "KY";  // en-us
		var gameType = GameProvidersClientGameType.CardGameLobby;
		var gameId = 1;
		var brandCode = "GPI";
		var playerCode = "test";
		var currencyCode = "CNY";
		var loginRecordId = Guid.NewGuid();
		var platformType = GameProvidersClientPlatformType.Desktop;
		var language = "zh-TW";
		var ipAddress = "127.0.0.1";
		var backUrl = new Uri("https://dev-t1.rpgp.cc/");

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IGetService<KaiyuanGameUrlQuery, string>>();

				var query = new KaiyuanGameUrlQuery
				{
					GameType = (GameType)gameType,
					GameId = gameId,
					BrandCode = brandCode,
					PlayerCode = playerCode,
					CurrencyCode = currencyCode,
					LoginRecordId = loginRecordId,
					PlatformType = (PlatformType)platformType,
					Language = language,
					IPAddress = ipAddress,
					BackUrl = backUrl
				};
				_ = fakeService
					.GetAsync(
						query: Arg.Is(query),
						cancellationToken: Arg.Any<CancellationToken>())
					.Returns("http://localhost:8088/");

				_ = services.AddTransient(_ => fakeService);
			}));

		var httpClient = web.CreateClient();

		var sut = new GameProvidersClient(httpClient);

		var actual = await sut
			.GameUrlAsync(
			gameProviderCode: gameProviderCode,
			gameType: gameType,
			gameId: gameId,
			brandCode: brandCode,
			playerCode: playerCode,
			currencyCode: currencyCode,
			loginRecordId: loginRecordId,
			platformType: platformType,
			language: language,
			ipAddress: ipAddress,
			backUrl: backUrl);

		Assert.NotNull(actual);
	}

	[Fact]
	public async void Game_Url_HttpRequestException()
	{
		var gameProviderCode = "KY";  // en-us
		var gameType = GameProvidersClientGameType.CardGameLobby;
		var gameId = 1;
		var brandCode = "GPI";
		var playerCode = "test";
		var currencyCode = "CNY";
		var loginRecordId = Guid.NewGuid();
		var platformType = GameProvidersClientPlatformType.Desktop;
		var language = "zh-TW";
		var ipAddress = "127.0.0.1";
		var backUrl = new Uri("https://dev-t1.rpgp.cc/");

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IGetService<KaiyuanGameUrlQuery, GameUrlResult>>();

				var query = new KaiyuanGameUrlQuery
				{
					GameType = (GameType)gameType,
					GameId = gameId,
					BrandCode = brandCode,
					PlayerCode = playerCode,
					CurrencyCode = currencyCode,
					LoginRecordId = loginRecordId,
					PlatformType = (PlatformType)platformType,
					Language = language,
					IPAddress = ipAddress,
					BackUrl = backUrl
				};
				_ = fakeService
					.GetAsync(
						query: Arg.Is(query),
						cancellationToken: Arg.Any<CancellationToken>())
					.ThrowsForAnyArgs<Exception>();

				_ = services.AddTransient(_ => fakeService);
			}));
		var httpClient = web.CreateClient();

		var sut = new GameProvidersClient(httpClient);

		_ = await Assert.ThrowsAsync<HttpRequestException>(() => sut
		.GameUrlAsync(
			gameProviderCode: gameProviderCode,
			gameType: gameType,
			gameId: gameId,
			brandCode: brandCode,
			playerCode: playerCode,
			currencyCode: currencyCode,
			loginRecordId: loginRecordId,
			platformType: platformType,
			language: language,
			ipAddress: ipAddress,
			backUrl: backUrl)
		.AsTask());
	}
}
