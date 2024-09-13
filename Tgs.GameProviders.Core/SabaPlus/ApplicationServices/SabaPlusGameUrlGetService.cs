using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using Tgs.GameInterface.SabaPlus.Client;
using Tgs.GameInterface.SabaPlus.Domains.Models;
using Tgs.GameInterface.SabaPlus.Domains.GetGameUrl;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using System;

namespace Tgs.GameProviders.Core.SabaPlus.ApplicationServices;

public class SabaPlusGameUrlGetService(
	IGameInterfaceSabaPlusClient client,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameProviderRepository gameProviderRepository,
	IGameRepository gameRepository) : IGetService<SabaPlusGameUrlQuery, GetGameUrlResult>
{
	public async ValueTask<GetGameUrlResult> GetAsync(SabaPlusGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			query.ProviderCode,
			cancellationToken: cancellationToken).ConfigureAwait(false);
		var gameInfo = await gameRepository.GetAsync(query.GameId, cancellationToken).ConfigureAwait(false);
		var player = await playerRepository.GetAsync(query.PlayerCode, cancellationToken).ConfigureAwait(false);
		var gameProviderConfigurationId = await brandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId,
			player.BrandId,
			cancellationToken: cancellationToken).ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetSabaPlusConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken).ConfigureAwait(false);
		var subProviderConfiguration = configuration.GetSubProviderConfiguration(gameInfo.SubGameProviderCode);
		var homeUrl = query.BackUrl.ToString();
		var gameProviderCurrencyCode = await gameProviderRepository.GetGameProviderCurrencyCodeAsync(
			gameProviderId,
			player.DefaultWalletCurrencyCode,
			cancellationToken).ConfigureAwait(false);

		var walletToken = $"{query.ProviderCode}@{Guid.NewGuid()}";
		var providerInfo = GetProviderInfo(subProviderConfiguration);
		var productInfo = GetProductInfo(gameInfo, subProviderConfiguration.GameType);
		var custInfo = GetCustInfo(player, query, gameProviderCurrencyCode, walletToken);
		var agentInfo = GetAgentInfo(subProviderConfiguration);

		var url = await client.GameUrlAsync(
			providerInfo,
			productInfo,
			custInfo,
			agentInfo,
			homeUrl,
			subProviderConfiguration.Additional,
			cancellationToken).ConfigureAwait(false);

		return url;
	}

	private static ProviderInfo GetProviderInfo(SabaPlusSubProviderConfiguration configuration)
		=> new(configuration.ApiUrl, configuration.OperatorPassword, configuration.OperatorSignKey);

	private static AgentInfo GetAgentInfo(SabaPlusSubProviderConfiguration configuration)
		=> new(configuration.AgentId, configuration.AgentKey, configuration.AgentCode, configuration.AgentExtraInfo);

	private static CustInfo GetCustInfo(PlayerInfo player, SabaPlusGameUrlQuery query, string currency, string walletToken)
		=> new(
			player.PlayerCode,
			player.Username,
			query.Language,
			GetUserPlatform(query.PlatformType),
			currency,
			query.ExtraInfo.BrowserUserAgent,
			query.IPAddress,
			walletToken);

	private static ProductInfo GetProductInfo(GameInfo game, string gameType) =>
		new(gameType, game.GameExternalId, GetGamePlatform(game.PlatformType));

	private static string GetGamePlatform(GamePlatformType type)
		=> type switch
		{
			GamePlatformType.Desktop => "D",
			GamePlatformType.Mobile => "M",
			GamePlatformType.DesktopAndMobile => "M",
			_ => "M",
		};

	private static string GetUserPlatform(PlatformType type)
		=> type switch
		{
			PlatformType.Desktop => "D",
			PlatformType.Mobile => "M",
			_ => "M",
		};
}
