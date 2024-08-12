using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.OBet.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.OBet.ApplicationServices;

public class OBetGameUrlGetService : IGetService<OBetGameUrlQuery, string>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceOBetClient _client;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public OBetGameUrlGetService(
		IGameProviderRepository gameProvider,
		IPlayerRepository player,
		IBrandRepository brand,
		IGameInterfaceOBetClient client)
	{
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_player = player ?? throw new ArgumentNullException(nameof(player));
	}

	public async ValueTask<string> GetAsync(OBetGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider
			.GetGameProviderIdAsync(
				gameProviderCode: GameProviderType.OBet,
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
			.GetOBetConfigurationAsync(
				gameProviderConfigurationId: gameProviderConfigurationId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await _gameProvider
			.GetGameProviderCultureCodeAsync(
				gameProviderId: gameProviderId,
				cultureCode: query.Language,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		await _client
			.PlayerUpdateBetLimitAsync(
				apiUrl: configuration.ApiUrl,
				apiKey: configuration.ApiKey,
				agentId: configuration.AgentId,
				playerCode: player.PlayerCode,
				maxBetGrpHdp: configuration.MaxBetGrpHdp,
				maxBetGrpBasketBall: configuration.MaxBetGrpBasketBall,
				maxBetGrpOthersSport: configuration.MaxBetGrpOthersSport,
				maxBetGrp1x2: configuration.MaxBetGrp1x2,
				maxBetGrpMixParlay: configuration.MaxBetGrpMixParlay,
				maxBetGrpOthersOdds: configuration.MaxBetGrpOthersOdds,
				maxBetGrpSpecial: configuration.MaxBetGrpSpecial,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var response = await _client
			.GameUrlAsync(
				apiUrl: configuration.ApiUrl,
				apiKey: configuration.ApiKey,
				agentId: configuration.AgentId,
				playerCode: player.PlayerCode,
				lang: string.IsNullOrEmpty(cultureCode) ? "en" : cultureCode,
				isMobile: query.PlatformType == PlatformType.Mobile,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return response.IsPlayerExist ? response.GameUrl : throw new Exception($"player does not exist.({player.PlayerCode})");
	}
}
