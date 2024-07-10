using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.GlobalTechnology.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.GTExpanse.ApplicationServices;

public class GTExpansePlayerLogoutCommandService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameInterfaceGlobalTechnologyClient client) : ICommandService<GTExpansePlayerLogoutCommand>
{
	public async ValueTask ExecuteAsync(GTExpansePlayerLogoutCommand command, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.GT_Expanse,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var playerInfo = await player.GetAsync(
			playerCode: command.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await brand.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: playerInfo.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProvider.GetGTExpanseConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await client.PlayerLogoutAsync(
			apiUrl: configuration.ApiUrl,
			brandId: configuration.BrandId,
			secretKey: configuration.SecretKey,
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
