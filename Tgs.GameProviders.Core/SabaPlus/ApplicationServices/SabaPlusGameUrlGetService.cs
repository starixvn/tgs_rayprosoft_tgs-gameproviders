using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using Tgs.GameInterface.SabaPlus.Client;
using Tgs.GameInterface.SabaPlus.Domains.GetGameUrl;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameInterface.SabaPlus.Domains.FromProviders;

namespace Tgs.GameProviders.Core.SabaPlus.ApplicationServices;

public class SabaPlusGameUrlGetService(
	IGameInterfaceSabaPlusClient client,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameProviderRepository gameProviderRepository,
	IGameRepository gameRepository) : SabaPlusService, IGetService<SabaPlusGameUrlQuery, GetGameUrlResult>
{
	public async ValueTask<GetGameUrlResult> GetAsync(SabaPlusGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameInfo = await gameRepository.GetAsync(query.GameId, cancellationToken).ConfigureAwait(false);
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameInfo.GameProviderCode,
			cancellationToken: cancellationToken).ConfigureAwait(false);
		var player = await playerRepository.GetAsync(query.PlayerCode, cancellationToken).ConfigureAwait(false);
		var gameProviderConfigurationId = await brandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId,
			player.BrandId,
			cancellationToken: cancellationToken).ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetSabaPlusConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken).ConfigureAwait(false);
		var subProviderConfiguration = configuration.GetSubProviderConfiguration(gameInfo.SubGameProviderCode);
		var gameProviderCurrencyCode = await gameProviderRepository.GetGameProviderCurrencyCodeAsync(
			gameProviderId,
			player.DefaultWalletCurrencyCode,
			cancellationToken).ConfigureAwait(false);

		var interfaceQuery = new GameInterface.SabaPlus.Domains.GetGameUrl.GameUrlQuery
		{
			Query = Map<SabaPlusGameUrlQuery, Query>(query),
			Configuration = Map<SabaPlusSubProviderConfiguration, ProviderConfiguration>(subProviderConfiguration),
			Player = Map<PlayerInfo, Player>(player),
			Game = Map<GameInfo, Game>(gameInfo),
			Currency = gameProviderCurrencyCode,
		};

		var url = await client.GameUrlAsync(interfaceQuery, cancellationToken).ConfigureAwait(false);

		return url;
	}
}
