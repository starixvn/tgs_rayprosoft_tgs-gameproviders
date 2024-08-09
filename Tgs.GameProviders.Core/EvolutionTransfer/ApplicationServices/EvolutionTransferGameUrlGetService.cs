using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.EvolutionTransfer.ApplicationServices;

public class EvolutionTransferGameUrlGetService : IGetService<EvolutionTransferGameUrlQuery, string>
{
	private readonly IGameRepository _game;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IGameInterfaceEvolutionClient _grpc;
	private readonly IPlayerRepository _player;
	private readonly ISettingRepository _setting;
	private readonly ITokenProvider _tokenProvider;

	public EvolutionTransferGameUrlGetService(
		IGameProviderRepository gameProvider,
		IGameRepository game,
		IGameInterfaceEvolutionClient grpc,
		IPlayerRepository player,
		ISettingRepository setting,
		ITokenProvider tokenProvider)
	{
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_game = game ?? throw new ArgumentNullException(nameof(game));
		_grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
		_player = player ?? throw new ArgumentNullException(nameof(player));
		_setting = setting ?? throw new ArgumentNullException(nameof(setting));
		_tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
	}

	public async ValueTask<string> GetAsync(EvolutionTransferGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var encryptedToken = await _tokenProvider.EncryptAsync(query.LoginRecordId).ConfigureAwait(false);
		var gameProviderId = await _gameProvider
			.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.EvolutionTransfer,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await _setting
			.GetEvolutionTransferConfigurationAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var player = await _player.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await _gameProvider
			.GetGameProviderCultureCodeAsync(
			gameProviderId,
			query.Language,
			cancellationToken)
			.ConfigureAwait(false) ?? "en-US";
		var currencyCode = await _gameProvider
			.GetGameProviderCurrencyCodeAsync(
			gameProviderId: gameProviderId,
			currencyCode: player.DefaultWalletCurrencyCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var game = await _game
			.GetAsync(
			gameId: query.GameId!,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var response = game.Type switch
		{
			GameType.LiveDealerLobby =>
				await _grpc.GetLiveDealerGameUrlAsync(
				configuration.Ua2AuthenticationUrl,
				player.BrandCode,
				player.PlayerCode,
				(short)query.PlatformType,
				query.IPAddress,
				encryptedToken,
				cultureCode,
				currencyCode,
				cancellationToken).ConfigureAwait(false),
			_ =>
				await _grpc.GetSlotGameUrlAsync(
				configuration.Ua2AuthenticationUrl,
				player.BrandCode,
				player.PlayerCode,
				(short)query.PlatformType,
				query.IPAddress,
				encryptedToken,
				cultureCode,
				currencyCode,
				game.GameExternalId,
				cancellationToken).ConfigureAwait(false)
		};

		var path = response.Entry[1..];

		return $"{configuration.ApiUrl}{path}";
	}
}
