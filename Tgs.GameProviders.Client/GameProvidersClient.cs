using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.RoundDetail;

namespace Tgs.GameProviders.Client;

internal class GameProvidersClient(HttpClient httpClient) : IGameProvidersClient
{
	private static readonly string _GameURL = "/api/{0}/gameurl";
	private static readonly string _PlayerLogoutPath = "/api/{0}/player/{1}/logout";
	private static readonly string _PlayerBalancePath = "/api/{0}/player/{1}/balance";
	private static readonly string _PlayerFundChecking = "/api/{0}/player/{1}/fund/checking";
	private static readonly string _PlayerFundInPath = "/api/{0}/player/{1}/fund/in";
	private static readonly string _PlayerFundOutPath = "/api/{0}/player/{1}/fund/out";
	private static readonly string _TicketDetailPath = "/api/{0}/ticketdetail";
	private static readonly string _RoundDetail = "/api/{0}/round/{1}/detail";
	private static readonly string _PlayerOnline = "/api/{0}/player/{1}/online";
	private static readonly string _BetResultCheck = "/api/{0}/betresultcheck";

	private static readonly JsonSerializerOptions? _SerializerOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters =
		{
			new JsonStringEnumConverter()
		}
	};

	public async ValueTask<Uri> GameUrlAsync(string gameProviderCode, string playerCode, int gameId, string loginRecordId, string language, PlatformType platformType, string ipAddress, Uri? backUrl, object? extraInfo = null, CancellationToken cancellationToken = default)
	{
		var request = new
		{
			PlayerCode = playerCode,
			GameId = gameId,
			LoginRecordId = loginRecordId,
			PlatformType = platformType,
			Language = language,
			IPAddress = ipAddress,
			BackUrl = backUrl,
			ExtraInfo = extraInfo
		};

		var requestContent = JsonContent.Create(request, options: _SerializerOptions);

		var response = await httpClient.PostAsync(
			requestUri: string.Format(_GameURL, gameProviderCode),
			content: requestContent,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return response.StatusCode == HttpStatusCode.NoContent
			? throw new Exception($"取得{gameProviderCode}遊戲網址失敗.")
			: JsonSerializer.Deserialize<GameUrlResponse>(
				await response
				.Content
				.ReadAsStringAsync(
					cancellationToken: cancellationToken)
				.ConfigureAwait(false),
				_SerializerOptions)?.Url
				?? throw new Exception($"取得{gameProviderCode}遊戲網址失敗.");
	}

	public async ValueTask<PlayerLogoutResponse> PlayerLogoutAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default)
	{
		var response = await httpClient
		  .DeleteAsync(
			  requestUri: string.Format(_PlayerLogoutPath, gameProviderCode, playerCode),
			  cancellationToken: cancellationToken).ConfigureAwait(false);

		if (response.StatusCode == HttpStatusCode.NotFound)
		{
			return new PlayerLogoutResponse(GameProviderSupport: false);
		}

		_ = response.EnsureSuccessStatusCode();

		return new PlayerLogoutResponse(GameProviderSupport: true);
	}

	public async ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default)
	{
		var response = await httpClient
			.GetAsync(
				requestUri: string.Format(_PlayerBalancePath, gameProviderCode, playerCode),
				cancellationToken: cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return response.StatusCode == HttpStatusCode.NoContent
			? throw new Exception($"取得{gameProviderCode}餘額失敗.")
			: JsonSerializer.Deserialize<PlayerBalanceResponse>(
				await response
				.Content
				.ReadAsStringAsync(
					cancellationToken: cancellationToken)
				.ConfigureAwait(false),
				_SerializerOptions)!;
	}

	public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(string gameProviderCode, string playerCode, string orderId, DateTimeOffset orderOn, CancellationToken cancellationToken = default)
	{
		var path = new Uri(httpClient.BaseAddress!, string.Format(_PlayerFundChecking, gameProviderCode, playerCode));
		var builder = new UriBuilder(path);
		var query = HttpUtility.ParseQueryString(builder.Query);
		query["orderId"] = orderId;
		query["orderOn"] = orderOn.ToString("u");
		builder.Query = query.ToString();
		string url = builder.ToString();

		return await httpClient.GetAsync(url, cancellationToken)
			.ToAsyncEnumerable()
			.Do(response => response.EnsureSuccessStatusCode())
			.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
			.SelectMany(response => response.Content
				.ReadFromJsonAsync<PlayerFundCheckingResponse>(_SerializerOptions, cancellationToken)
				.ToAsyncEnumerable())
			.Where(info => info is not null)
			.Select(info => info!)
			.FirstOrDefaultAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<PlayerFundResponse> PlayerFundInAsync(string gameProviderCode, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default)
	{
		var source = new
		{
			OrderId = orderId,
			Amount = amount
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);

		var response = await httpClient.PostAsync(
			requestUri: string.Format(_PlayerFundInPath, gameProviderCode, playerCode),
			content: requestContent,
			cancellationToken: cancellationToken)
		   .ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return (await response.Content.ReadFromJsonAsync<PlayerFundResponse>(
			_SerializerOptions,
			cancellationToken)
			.ConfigureAwait(false))!;
	}

	public async ValueTask<PlayerFundResponse> PlayerFundOutAsync(string gameProviderCode, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default)
	{
		var source = new
		{
			OrderId = orderId,
			Amount = amount
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);

		var response = await httpClient.PostAsync(
			requestUri: string.Format(_PlayerFundOutPath, gameProviderCode, playerCode),
			content: requestContent,
			cancellationToken: cancellationToken)
		   .ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return (await response.Content.ReadFromJsonAsync<PlayerFundResponse>(
			_SerializerOptions,
			cancellationToken)
			.ConfigureAwait(false))!;
	}

	public async ValueTask<PlayerOnlineResponse> PlayerOnlineAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default)
	{
		var response = await httpClient.GetAsync(
			requestUri: string.Format(_PlayerOnline, gameProviderCode, playerCode),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (response.StatusCode == HttpStatusCode.NotFound)
		{
			return new PlayerOnlineResponse(
				GameProviderSupport: false,
				Online: false);
		}

		var httpResponseMessage = response.EnsureSuccessStatusCode();

		var playerOnlineResponse = JsonSerializer.Deserialize<PlayerOnlineResponse>(await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false), _SerializerOptions)!;

		return new PlayerOnlineResponse(
			GameProviderSupport: true,
			Online: playerOnlineResponse.Online);
	}

	public async ValueTask<JsonDocument?> TicketDetailAsync(TicketDetailRequest request, CancellationToken cancellationToken = default)
	{
		var requestContent = JsonContent.Create(request, options: _SerializerOptions);

		var response = await httpClient.PostAsync(
			requestUri: string.Format(_TicketDetailPath, request.GameProviderCode),
			content: requestContent,
			cancellationToken: cancellationToken)
		   .ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return (await response.Content.ReadFromJsonAsync<JsonDocument>(
			_SerializerOptions,
			cancellationToken)
			.ConfigureAwait(false))!;
	}

	public async ValueTask<RoundDetailResult?> RoundDetailAsync(string gameProviderCode, Guid roundId, DateTimeOffset externalCreatedOn, string language, CancellationToken cancellationToken = default)
	{
		var path = new Uri(httpClient.BaseAddress!, string.Format(_RoundDetail, gameProviderCode, roundId));
		var builder = new UriBuilder(path);
		var query = HttpUtility.ParseQueryString(builder.Query);
		query["language"] = language;
		query["externalCreatedOn"] = externalCreatedOn.ToString("u");
		builder.Query = query.ToString();
		string url = builder.ToString();

		return await httpClient.GetAsync(url, cancellationToken)
			.ToAsyncEnumerable()
			.Do(response => response.EnsureSuccessStatusCode())
			.Where(response => response.StatusCode == HttpStatusCode.OK)
			.SelectMany(response => response.Content
				.ReadFromJsonAsync<RoundDetailResult>(_SerializerOptions, cancellationToken)
				.ToAsyncEnumerable())
			.Where(info => info is not null)
			.Select(info => info!)
			.FirstOrDefaultAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<TResult> BetResultCheck<TResult>(string gameProviderCode, string externalGameId, string externalRoundId, string playerCode, CancellationToken cancellationToken = default)
	{
		var uri = string.Format(_BetResultCheck, gameProviderCode);
		var request = new
		{
			ExternalGameId = externalGameId,
			ExternalRoundId = externalRoundId,
			PlayerCode = playerCode,
		};
		var requestContent = JsonContent.Create(request, options: _SerializerOptions);

		var response = await httpClient.PostAsync(
			requestUri: uri,
			content: requestContent,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		response.EnsureSuccessStatusCode();

		var dataInString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
		var data = JsonSerializer.Deserialize<TResult>(dataInString, _SerializerOptions);

		return data ?? throw new Exception($"Error cheking player {playerCode}'s bet result in game {externalGameId}({externalRoundId}) of {gameProviderCode} provider");
	}
}
