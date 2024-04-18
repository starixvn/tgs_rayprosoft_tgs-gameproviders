using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.CrPati.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.CrPati.ApplicationServices;

public class CrPatiGameUrlGetService : IGetService<CrPatiGameUrlQuery, string>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceCrPatiClient _client;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public CrPatiGameUrlGetService(
		IGameProviderRepository gameProvider,
		IPlayerRepository player,
		IBrandRepository brand,
		IGameInterfaceCrPatiClient client
		)
	{
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_player = player ?? throw new ArgumentNullException(nameof(player));
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public async ValueTask<string> GetAsync(CrPatiGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.CrPati,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var player = await _player.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: player.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await _gameProvider.GetCrPatiConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "0";

		var reply = await _client.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			apiKey: configuration.ApiKey,
			apiSecret: configuration.ApiSecret,
			playerCode: player.PlayerCode,
			ipAddress: query.IPAddress,
			lang: int.Parse(cultureCode),
			isMobile: query.PlatformType == PlatformType.Mobile,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			await _client.PlayerCreateAsync(
				apiUrl: configuration.ApiUrl,
				apiKey: configuration.ApiKey,
				apiSecret: configuration.ApiSecret,
				playerCode: player.PlayerCode,
				ipAddress: query.IPAddress,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			reply = await _client.GameUrlAsync(
				apiUrl: configuration.ApiUrl,
				apiKey: configuration.ApiKey,
				apiSecret: configuration.ApiSecret,
				playerCode: player.PlayerCode,
				ipAddress: query.IPAddress,
				lang: int.Parse(cultureCode),
				isMobile: query.PlatformType == PlatformType.Mobile,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return !reply.IsPlayerExist
			? throw new Exception($"Player does not exist.({player.PlayerCode})")
			: reply.GameUrl;
	}
}
