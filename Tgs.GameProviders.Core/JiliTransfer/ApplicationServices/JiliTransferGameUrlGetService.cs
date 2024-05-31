using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Jili.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;

public class JiliTransferGameUrlGetService : IGetService<JiliTransferGameUrlQuery, Uri>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceJiliClient _client;
	private readonly ICommandService<JiliTransferCreatePlayerCommand> _createPlayerCommandService;
	private readonly IGameRepository _game;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public JiliTransferGameUrlGetService(
		IBrandRepository brand,
		ICommandService<JiliTransferCreatePlayerCommand> createPlayerCommandService,
		IGameRepository game,
		IGameProviderRepository gameProvider,
		IGameInterfaceJiliClient client,
		IPlayerRepository player)
	{
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_createPlayerCommandService = createPlayerCommandService ?? throw new ArgumentNullException(nameof(createPlayerCommandService));
		_game = game ?? throw new ArgumentNullException(nameof(game));
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_player = player ?? throw new ArgumentNullException(nameof(player));
	}

	public async ValueTask<Uri> GetAsync(JiliTransferGameUrlQuery command, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider
			.GetGameProviderIdAsync(
				gameProviderCode: GameProviderType.JiliTransfer,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var player = await _player
			.GetAsync(
				playerCode: command.PlayerCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await _brand
			.GetBrandGameProviderConfigurationIdAsync(
				gameProviderId: gameProviderId,
				brandId: player.BrandId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await _gameProvider
			.GetJiliTransferConfigurationAsync(
				gameProviderConfigurationId: gameProviderConfigurationId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await _gameProvider
			.GetGameProviderCultureCodeAsync(
				gameProviderId: gameProviderId,
				cultureCode: command.Language,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en-US";
		var game = await _game
			.GetAsync(
				gameId: command.GameId.Value,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await _client.GetGameUrlAsync(
			apiUrl: configuration.ApiUrl,
			agentId: configuration.AgentId,
			agentKey: configuration.AgentKey,
			playerCode: player.PlayerCode,
			externalGameId: game.GameExternalId,
			language: cultureCode,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		if (reply.Success && !reply.IsPlayerExist)
		{
			await _createPlayerCommandService.ExecuteAsync(new JiliTransferCreatePlayerCommand(
				Configuration: configuration,
				PlayerCode: player.PlayerCode),
				cancellationToken).ConfigureAwait(false);

			reply = await _client.GetGameUrlAsync(
				apiUrl: configuration.ApiUrl,
				agentId: configuration.AgentId,
				agentKey: configuration.AgentKey,
				playerCode: player.PlayerCode,
				externalGameId: game.GameExternalId,
				language: cultureCode,
				cancellationToken: cancellationToken).ConfigureAwait(false);
		}

		return !reply.IsPlayerExist
			? throw new Exception($"Player does not exist.({player.PlayerCode})")
			: new Uri(reply.GameUrl);
	}
}
