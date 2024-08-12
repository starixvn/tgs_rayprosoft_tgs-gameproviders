using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices;

public class WMCasinoTransferGameUrlGetService : IGetService<WMCasinoTransferGameUrlQuery, string>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceWMCasinoServiceClient _client;
	private readonly ICommandService<WMCasinoTransferCreatePlayerCommand> _createPlayerCommandService;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public WMCasinoTransferGameUrlGetService(
		IBrandRepository brand,
		ICommandService<WMCasinoTransferCreatePlayerCommand> createPlayerCommandService,
		IGameProviderRepository gameProvider,
		IGameInterfaceWMCasinoServiceClient client,
		IPlayerRepository player)
	{
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_createPlayerCommandService = createPlayerCommandService ?? throw new ArgumentNullException(nameof(createPlayerCommandService));
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_player = player ?? throw new ArgumentNullException(nameof(player));
	}

	public async ValueTask<string> GetAsync(WMCasinoTransferGameUrlQuery command, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.WMCasinoTransfer,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var player = await _player.GetAsync(
			playerCode: command.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: player.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await _gameProvider.GetWMCasinoTransferConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: command.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "0";

		var password = "TgsP@ssword";
		var user = $"wmtf_{command.PlayerCode}_{player.BrandCode}";

		var reply = await _client.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			vendorId: configuration.VendorId,
			signature: configuration.Signature,
			user: user,
			password: password,
			language: int.Parse(cultureCode),
			cancellationToken: cancellationToken).ConfigureAwait(false);

		if (reply.Success && !reply.IsPlayerExist)
		{
			await _createPlayerCommandService.ExecuteAsync(new WMCasinoTransferCreatePlayerCommand(
				Configuration: configuration,
				PlayerCode: player.PlayerCode,
				CurrencyCode: player.DefaultWalletCurrencyCode),
				cancellationToken).ConfigureAwait(false);

			reply = await _client.GameUrlAsync(
				apiUrl: configuration.ApiUrl,
				vendorId: configuration.VendorId,
				signature: configuration.Signature,
				user: user,
				password: password,
				language: 0,
				cancellationToken: cancellationToken).ConfigureAwait(false);
		}

		return !reply.IsPlayerExist
			? throw new Exception($"Player does not exist.({player.PlayerCode})")
			: reply.GameUrl;
	}
}
