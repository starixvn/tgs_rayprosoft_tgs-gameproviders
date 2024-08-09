using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Pinnacle.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Pinnacle.ApplicationServices;

public class PinnacleGameUrlGetService : IGetService<PinnacleGameUrlQuery, string>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfacePinnacleClient _client;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public PinnacleGameUrlGetService(
		IGameProviderRepository gameProvider,
		IPlayerRepository player,
		IBrandRepository brand,
		IGameInterfacePinnacleClient client)
	{
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_player = player ?? throw new ArgumentNullException(nameof(player));
	}

	public async ValueTask<string> GetAsync(PinnacleGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider
			.GetGameProviderIdAsync(
				gameProviderCode: GameProviderType.Pinnacle,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var player = await _player
			.GetAsync(
				playerCode: query.PlayerCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await _brand
			.GetBrandGameProviderConfigurationIdAsync(
				gameProviderId: gameProviderId,
				brandId: player.BrandId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await _gameProvider
			.GetPinnacleConfigurationAsync(
				gameProviderConfigurationId: gameProviderConfigurationId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await _gameProvider
			.GetGameProviderCultureCodeAsync(
				gameProviderId: gameProviderId,
				cultureCode: query.Language,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		var gameUrl = await _client
			.GameUrlAsync(
				apiUrl: configuration.ApiUrl,
				agentCode: configuration.AgentCode,
				agentKey: configuration.AgentKey,
				secretKey: configuration.SecretKey,
				playerCode: player.PlayerCode,
				locale: string.IsNullOrEmpty(cultureCode) ? "en" : cultureCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return gameUrl;
	}
}
