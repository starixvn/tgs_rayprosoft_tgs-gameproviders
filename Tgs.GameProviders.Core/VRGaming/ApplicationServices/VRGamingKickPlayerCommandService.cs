using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Tgs.GameProviders.Core.VRGaming.ApplicationServices;
using Tgs.GameInterface.VRGaming.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains;

namespace Tgs.GameProviders.Core.VRGaming.ApplicationServices;

public class VRGamingKickPlayerCommandService(
    IGameProviderRepository gameProvider,
    IPlayerRepository player,
    IBrandRepository brand,
    IGameInterfaceVRGamingClient client) : ICommandService<VRGamingKickPlayerCommand>
{
    public async ValueTask ExecuteAsync(VRGamingKickPlayerCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await gameProvider.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.VRGaming,
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
        var configuration = await gameProvider.GetVRGamingConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await client.PlayerLogoutAsync(
           apiUrl: configuration.ApiUrl,
           version: configuration.Version,
           merchantId: configuration.MerchantId,
           deskey: configuration.Deskey,
           playerCode: playerInfo.PlayerCode,
           cancellationToken: cancellationToken)
           .ConfigureAwait(false);
    }
}
