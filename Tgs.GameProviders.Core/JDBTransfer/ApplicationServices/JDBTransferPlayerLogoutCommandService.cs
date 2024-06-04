using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.JDBService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JDBTransfer.ApplicationServices;

public class JDBTransferPlayerLogoutCommandService(
	IGameInterfaceJDBClient client,
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository) : ICommandService<JDBTransferPlayerLogoutCommand>
{
	public async ValueTask ExecuteAsync(JDBTransferPlayerLogoutCommand command, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.JDBTransfer,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var playerInfo = await playerRepository.GetAsync(
			command.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await brandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId,
			playerInfo.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetJDBTransferConfigurationAsync(
			gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await client.PlayerLogoutAsync(
			configuration.ApiUrl,
			configuration.Domain,
			configuration.AesKey,
			configuration.AesIV,
			configuration.Agent,
			command.PlayerCode,
			cancellationToken).ConfigureAwait(false);
	}
}
