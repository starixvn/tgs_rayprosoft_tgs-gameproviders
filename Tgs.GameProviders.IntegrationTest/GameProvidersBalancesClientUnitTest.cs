using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.AwcAelotto.ApplicationServices;
using Tgs.GameProviders.Core.AwcAws.ApplicationServices;
using Tgs.GameProviders.Core.AwcCockfight.ApplicationServices;
using Tgs.GameProviders.Core.AwcFacai.ApplicationServices;
using Tgs.GameProviders.Core.AwcFs.ApplicationServices;
using Tgs.GameProviders.Core.AwcKm.ApplicationServices;
using Tgs.GameProviders.Core.AwcLudo.ApplicationServices;
using Tgs.GameProviders.Core.AwcPgs.ApplicationServices;
using Tgs.GameProviders.Core.AwcRcb.ApplicationServices;
using Tgs.GameProviders.Core.AwcSg.ApplicationServices;
using Tgs.GameProviders.Core.AwcSpade.ApplicationServices;
using Tgs.GameProviders.Core.AwcVenus.ApplicationServices;
using Tgs.GameProviders.Core.AwcYl.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.IntegrationTests;

public class GameProvidersBalancesClientUnitTest
{
	[Fact]
	public async Task AwcAelotto_GetBalances()
	{
		var brandCode = "GPI";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IQueryService<AwcAelottoBalancesQuery, BalanceResult>>();

				var query = new AwcAelottoBalancesQuery(brandCode);

				_ = fakeService.QueryAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(new BalanceResult[] {
					new(
						PlayerCode: "test",
						Balance: 1m,
						LastDateTime: DateTime.Now)
				}.ToAsyncEnumerable());

				_ = services.AddTransient(_ => fakeService);
			}));

		var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

		var httpClient = web.CreateDefaultClient([fakeLoggingHandler]);

		var sut = new GameProvidersBalancesClient(httpClient);

		var actual = await sut.AwcAelottoGetBalances(
			brandCode: brandCode)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task AwcAws_GetBalances()
	{
		var brandCode = "GPI";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IQueryService<AwcAwsBalancesQuery, BalanceResult>>();

				var query = new AwcAwsBalancesQuery(brandCode);

				_ = fakeService.QueryAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(new BalanceResult[] {
					new(
						PlayerCode: "test",
						Balance: 1m,
						LastDateTime: DateTime.Now)
				}.ToAsyncEnumerable());

				_ = services.AddTransient(_ => fakeService);
			}));

		var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

		var httpClient = web.CreateDefaultClient([fakeLoggingHandler]);

		var sut = new GameProvidersBalancesClient(httpClient);

		var actual = await sut.AwcAwsGetBalances(
			brandCode: brandCode)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task AwcCockfight_GetBalances()
	{
		var brandCode = "GPI";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IQueryService<AwcCockfightBalancesQuery, BalanceResult>>();

				var query = new AwcCockfightBalancesQuery(brandCode);

				_ = fakeService.QueryAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(new BalanceResult[] {
					new(
						PlayerCode: "test",
						Balance: 1m,
						LastDateTime: DateTime.Now)
				}.ToAsyncEnumerable());

				_ = services.AddTransient(_ => fakeService);
			}));

		var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

		var httpClient = web.CreateDefaultClient([fakeLoggingHandler]);

		var sut = new GameProvidersBalancesClient(httpClient);

		var actual = await sut.AwcCockfightGetBalances(
			brandCode: brandCode)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task AwcFacai_GetBalances()
	{
		var brandCode = "GPI";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IQueryService<AwcFacaiBalancesQuery, BalanceResult>>();

				var query = new AwcFacaiBalancesQuery(brandCode);

				_ = fakeService.QueryAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(new BalanceResult[] {
					new(
						PlayerCode: "test",
						Balance: 1m,
						LastDateTime: DateTime.Now)
				}.ToAsyncEnumerable());

				_ = services.AddTransient(_ => fakeService);
			}));

		var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

		var httpClient = web.CreateDefaultClient([fakeLoggingHandler]);

		var sut = new GameProvidersBalancesClient(httpClient);

		var actual = await sut.AwcFacaiGetBalances(
			brandCode: brandCode)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task AwcFs_GetBalances()
	{
		var brandCode = "GPI";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IQueryService<AwcFsBalancesQuery, BalanceResult>>();

				var query = new AwcFsBalancesQuery(brandCode);

				_ = fakeService.QueryAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(new BalanceResult[] {
					new(
						PlayerCode: "test",
						Balance: 1m,
						LastDateTime: DateTime.Now)
				}.ToAsyncEnumerable());

				_ = services.AddTransient(_ => fakeService);
			}));

		var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

		var httpClient = web.CreateDefaultClient([fakeLoggingHandler]);

		var sut = new GameProvidersBalancesClient(httpClient);

		var actual = await sut.AwcFsGetBalances(
			brandCode: brandCode)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task AwcKm_GetBalances()
	{
		var brandCode = "GPI";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IQueryService<AwcKmBalancesQuery, BalanceResult>>();

				var query = new AwcKmBalancesQuery(brandCode);

				_ = fakeService.QueryAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(new BalanceResult[] {
					new(
						PlayerCode: "test",
						Balance: 1m,
						LastDateTime: DateTime.Now)
				}.ToAsyncEnumerable());

				_ = services.AddTransient(_ => fakeService);
			}));

		var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

		var httpClient = web.CreateDefaultClient([fakeLoggingHandler]);

		var sut = new GameProvidersBalancesClient(httpClient);

		var actual = await sut.AwcKmGetBalances(
			brandCode: brandCode)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task AwcLudo_GetBalances()
	{
		var brandCode = "GPI";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IQueryService<AwcLudoBalancesQuery, BalanceResult>>();

				var query = new AwcLudoBalancesQuery(brandCode);

				_ = fakeService.QueryAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(new BalanceResult[] {
					new(
						PlayerCode: "test",
						Balance: 1m,
						LastDateTime: DateTime.Now)
				}.ToAsyncEnumerable());

				_ = services.AddTransient(_ => fakeService);
			}));

		var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

		var httpClient = web.CreateDefaultClient([fakeLoggingHandler]);

		var sut = new GameProvidersBalancesClient(httpClient);

		var actual = await sut.AwcLudoGetBalances(
			brandCode: brandCode)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task AwcPgs_GetBalances()
	{
		var brandCode = "GPI";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IQueryService<AwcPgsBalancesQuery, BalanceResult>>();

				var query = new AwcPgsBalancesQuery(brandCode);

				_ = fakeService.QueryAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(new BalanceResult[] {
					new(
						PlayerCode: "test",
						Balance: 1m,
						LastDateTime: DateTime.Now)
				}.ToAsyncEnumerable());

				_ = services.AddTransient(_ => fakeService);
			}));

		var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

		var httpClient = web.CreateDefaultClient([fakeLoggingHandler]);

		var sut = new GameProvidersBalancesClient(httpClient);

		var actual = await sut.AwcPgsGetBalances(
			brandCode: brandCode)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task AwcRcb_GetBalances()
	{
		var brandCode = "GPI";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IQueryService<AwcRcbBalancesQuery, BalanceResult>>();

				var query = new AwcRcbBalancesQuery(brandCode);

				_ = fakeService.QueryAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(new BalanceResult[] {
					new(
						PlayerCode: "test",
						Balance: 1m,
						LastDateTime: DateTime.Now)
				}.ToAsyncEnumerable());

				_ = services.AddTransient(_ => fakeService);
			}));

		var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

		var httpClient = web.CreateDefaultClient([fakeLoggingHandler]);

		var sut = new GameProvidersBalancesClient(httpClient);

		var actual = await sut.AwcRcbGetBalances(
			brandCode: brandCode)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task AwcSg_GetBalances()
	{
		var brandCode = "GPI";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IQueryService<AwcSgBalancesQuery, BalanceResult>>();

				var query = new AwcSgBalancesQuery(brandCode);

				_ = fakeService.QueryAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(new BalanceResult[] {
					new(
						PlayerCode: "test",
						Balance: 1m,
						LastDateTime: DateTime.Now)
				}.ToAsyncEnumerable());

				_ = services.AddTransient(_ => fakeService);
			}));

		var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

		var httpClient = web.CreateDefaultClient([fakeLoggingHandler]);

		var sut = new GameProvidersBalancesClient(httpClient);

		var actual = await sut.AwcSgGetBalances(
			brandCode: brandCode)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task AwcSpade_GetBalances()
	{
		var brandCode = "GPI";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IQueryService<AwcSpadeBalancesQuery, BalanceResult>>();

				var query = new AwcSpadeBalancesQuery(brandCode);

				_ = fakeService.QueryAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(new BalanceResult[] {
					new(
						PlayerCode: "test",
						Balance: 1m,
						LastDateTime: DateTime.Now)
				}.ToAsyncEnumerable());

				_ = services.AddTransient(_ => fakeService);
			}));

		var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

		var httpClient = web.CreateDefaultClient([fakeLoggingHandler]);

		var sut = new GameProvidersBalancesClient(httpClient);

		var actual = await sut.AwcSpadeGetBalances(
			brandCode: brandCode)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task AwcVenus_GetBalances()
	{
		var brandCode = "GPI";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IQueryService<AwcVenusBalancesQuery, BalanceResult>>();

				var query = new AwcVenusBalancesQuery(brandCode);

				_ = fakeService.QueryAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(new BalanceResult[] {
					new(
						PlayerCode: "test",
						Balance: 1m,
						LastDateTime: DateTime.Now)
				}.ToAsyncEnumerable());

				_ = services.AddTransient(_ => fakeService);
			}));

		var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

		var httpClient = web.CreateDefaultClient([fakeLoggingHandler]);

		var sut = new GameProvidersBalancesClient(httpClient);

		var actual = await sut.AwcVenusGetBalances(
			brandCode: brandCode)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task AwcYl_GetBalances()
	{
		var brandCode = "GPI";

		var web = new TestGameProvidersApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IQueryService<AwcYlBalancesQuery, BalanceResult>>();

				var query = new AwcYlBalancesQuery(brandCode);

				_ = fakeService.QueryAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(new BalanceResult[] {
					new(
						PlayerCode: "test",
						Balance: 1m,
						LastDateTime: DateTime.Now)
				}.ToAsyncEnumerable());

				_ = services.AddTransient(_ => fakeService);
			}));

		var fakeLoggingHandler = new LoggingHandler(web.Services.GetRequiredService<ILogger<LoggingHandler>>());

		var httpClient = web.CreateDefaultClient([fakeLoggingHandler]);

		var sut = new GameProvidersBalancesClient(httpClient);

		var actual = await sut.AwcYlGetBalances(
			brandCode: brandCode)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}
}
