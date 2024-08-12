using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.InplayMatrix.Client;
using Tgs.GameInterface.InplayMatrix.Common;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.InplayMatrixESports.ApplicationServices;

public class InplayMatrixESportsGameUrlGetService : IGetService<InplayMatrixESportsGameUrlQuery, string>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceInplayMatrixClient _client;
	private readonly IGameRepository _game;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public InplayMatrixESportsGameUrlGetService(
		IGameProviderRepository gameProvider,
		IPlayerRepository player,
		IBrandRepository brand,
		IGameRepository game,
		IGameInterfaceInplayMatrixClient client)
	{
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_game = game ?? throw new ArgumentNullException(nameof(game));
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_player = player ?? throw new ArgumentNullException(nameof(player));
	}

	public async ValueTask<string> GetAsync(InplayMatrixESportsGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider
			.GetGameProviderIdAsync(
				gameProviderCode: GameProviderType.InplayMatrix_ESports,
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
			.GetInplayMatrixESportsConfigurationAsync(
				gameProviderConfigurationId: gameProviderConfigurationId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await _gameProvider
			.GetGameProviderCultureCodeAsync(
				gameProviderId: gameProviderId,
				cultureCode: query.Language,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		var game = (await _game
			.GetGameListAsync(
				gameProviderId: gameProviderId,
				gameType: GameType.ESports,
				cancellationToken: cancellationToken)
			.ToArrayAsync(
				cancellationToken: cancellationToken)
			.ConfigureAwait(false)).Single();

		var response = await _client
			.GameUrlAsync(
				apiUrl: configuration.ApiUrl,
				merchantCode: configuration.MerchantCode,
				merchantPrefix: configuration.MerchantPrefix,
				playerCode: player.PlayerCode,
				productWallet: (int)InplayMatrixProductWallet.IM_eSports_Wallet,
				gameCode: game.GameExternalId,
				ipAddress: query.IPAddress,
				isMobile: query.PlatformType == PlatformType.Mobile,
				language: string.IsNullOrEmpty(cultureCode) ? "en" : cultureCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!response.IsPlayerExist)
		{
			var currencyCode = await _gameProvider
				.GetGameProviderCurrencyCodeAsync(
					gameProviderId: gameProviderId,
					currencyCode: player.DefaultWalletCurrencyCode,
					cancellationToken)
				.ConfigureAwait(false);

			await _client
				.PlayerCreateAsync(
					apiUrl: configuration.ApiUrl,
					merchantCode: configuration.MerchantCode,
					merchantPrefix: configuration.MerchantPrefix,
					playerCode: player.PlayerCode,
					currency: currencyCode,
					password: configuration.Password,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			response = await _client
			   .GameUrlAsync(
				   apiUrl: configuration.ApiUrl,
				   merchantCode: configuration.MerchantCode,
				   merchantPrefix: configuration.MerchantPrefix,
				   playerCode: player.PlayerCode,
				   productWallet: (int)InplayMatrixProductWallet.IM_eSports_Wallet,
				   gameCode: game.GameExternalId,
				   ipAddress: query.IPAddress,
				   isMobile: query.PlatformType == PlatformType.Mobile,
				   language: string.IsNullOrEmpty(cultureCode) ? "en" : cultureCode,
				   cancellationToken: cancellationToken)
			   .ConfigureAwait(false);
		}

		return response.IsPlayerExist ? response.GameUrl : throw new Exception($"player does not exist.({player.PlayerCode})");
	}
}
