using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using Tgs.GameInterface.SabaPlus.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameInterface.SabaPlus.Domains.FromProviders;
using Tgs.GameInterface.SabaPlus.Domains.BetResultCheck;

namespace Tgs.GameProviders.Core.SabaPlus.ApplicationServices;

public class SabaPlusBetResultCheckService(
	IGameInterfaceSabaPlusClient client,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameProviderRepository gameProviderRepository,
	IGameRepository gameRepository) : SabaPlusService, IGetService<SabaPlusBetResultCheckQuery, BetResultCheckResult>
{
	public async ValueTask<BetResultCheckResult> GetAsync(SabaPlusBetResultCheckQuery query, CancellationToken cancellationToken = default)
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

		var interfaceQuery = new BetResultCheckQuery
		{
			Configuration = Map<SabaPlusSubProviderConfiguration, ProviderConfiguration>(subProviderConfiguration),
			Player = Map<PlayerInfo, Player>(player),
			TicketNo = query.ExternalRoundId
		};

		var result = await client.BetResultCheck(interfaceQuery, cancellationToken).ConfigureAwait(false);

		return result;
	}
}
