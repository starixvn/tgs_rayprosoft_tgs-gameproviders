using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using Tgs.GameInterface.SabaPlus.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameInterface.SabaPlus.Domains.FromProviders;
using Tgs.GameProviders.Domains.RoundDetail;

namespace Tgs.GameProviders.Core.SabaPlus.ApplicationServices;

public class SabaPlusBetHistoryUrlGetService(
	IGameInterfaceSabaPlusClient client,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameProviderRepository gameProviderRepository,
	IGameRepository gameRepository) : SabaPlusService, IGetService<SabaPlusBetHistoryUrlQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(SabaPlusBetHistoryUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			query.GameProviderCode,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		var gameProvider = await gameProviderRepository.GetAsync(
			gameProviderId,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		var player = await playerRepository.GetAsync(query.PlayerCode, cancellationToken).ConfigureAwait(false);

		var gameProviderConfigurationId = await brandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId,
			player.BrandId,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		var gameInfo = await gameRepository.GetAsync(query.GameId, cancellationToken).ConfigureAwait(false);

		var configuration = await gameProviderRepository.GetSabaPlusConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		var subProviderConfiguration = configuration.GetSubProviderConfiguration(gameInfo.SubGameProviderCode);
		if (subProviderConfiguration is null)
		{
			return new RoundDetailResult()
			{
				Data = [new RoundDetailDataIframe() {
					Value = null
				}]
			};
		}

		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		var result = await client.BetHistoryUrlAsync(new()
		{
			ResultId = query.ResultId,
			SettleDate = query.SettleDate,
			ExternalRoundId = query.ExternalRoundId,
			Language = cultureCode,
			GameType = gameInfo.SubGameProviderCode,
			Configuration = Map<SabaPlusSubProviderConfiguration, ProviderConfiguration>(subProviderConfiguration),
			Player = Map<PlayerInfo, Player>(player),
		}, cancellationToken).ConfigureAwait(false);

		return new RoundDetailResult()
		{
			Data = [new RoundDetailDataIframe() {
				Value = result.ReturnUrl
			}]
		};
	}
}
