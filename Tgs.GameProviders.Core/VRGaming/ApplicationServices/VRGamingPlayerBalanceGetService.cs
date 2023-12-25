using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using Tgs.GameInterface.VRGaming.Client;
using Tgs.GameProviders.Domains;

namespace Tgs.GameProviders.Core.VRGaming.ApplicationServices;

public class VRGamingPlayerBalanceGetService(
    IGameProviderRepository gameProvider,
    IPlayerRepository player,
    IBrandRepository brand,
    IGameInterfaceVRGamingClient client) : IGetService<VRGamingPlayerBalanceQuery, PlayerBalanceResult>
{
    public async ValueTask<PlayerBalanceResult> GetAsync(VRGamingPlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await gameProvider.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.VRGaming,
            cancellationToken: cancellationToken)
           .ConfigureAwait(false);
        var playerInfo = await player.GetAsync(
            playerCode: query.PlayerCode,
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

        var reply = await client.PlayerBalanceAsync(
            apiUrl: configuration.ApiUrl,
            version: configuration.Version,
            merchantId: configuration.MerchantId,
            deskey: configuration.Deskey,
            playerCode: playerInfo.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            await client.PlayerCreateAsync(
                apiUrl: configuration.ApiUrl,
                version: configuration.Version,
                merchantId: configuration.MerchantId,
                deskey: configuration.Deskey,
                playerCode: playerInfo.PlayerCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            reply = await client.PlayerBalanceAsync(
               apiUrl: configuration.ApiUrl,
               version: configuration.Version,
               merchantId: configuration.MerchantId,
               deskey: configuration.Deskey,
               playerCode: playerInfo.PlayerCode,
               cancellationToken: cancellationToken)
               .ConfigureAwait(false);
        }

        return !reply.IsPlayerExist
            ? throw new Exception($"Player does not exist.{playerInfo.PlayerCode}")
            : new PlayerBalanceResult
            {
                FreeBalance = reply.Balance.ToDecimal().GetValueOrDefault(0)
            };
    }
}
