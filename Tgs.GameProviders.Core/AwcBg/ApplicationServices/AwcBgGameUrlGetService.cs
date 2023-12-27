using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcBg.ApplicationServices;

public class AwcBgGameUrlGetService(
    IBrandRepository brand,
    IGameProviderRepository gameProvider,
    IGameInterfaceAwcClient client,
    IPlayerRepository player) : IGetService<AwcBgGameUrlQuery, string>
{
    public async ValueTask<string> GetAsync(AwcBgGameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await gameProvider.GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.AWC_BG,
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

        var configuration = await gameProvider.GetAwcBgConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var cultureCode = await gameProvider.GetGameProviderCultureCodeAsync(
            gameProviderId: gameProviderId,
            cultureCode: query.Language,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false) ?? "en";

        var reply = await client.GameUrlByAwcBgAsync(
            apiUrl: configuration.ApiUrl,
            agentId: configuration.AgentId,
            cert: configuration.Cert,
            playerCode: playerInfo.PlayerCode,
            isMobileLogin: query.PlatformType == PlatformType.Mobile,
            backUrl: query.BackUrl.ToString(),
            betLimit: configuration.BetLimit.ToJsonString(),
            language: cultureCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!reply.IsPlayerExist)
        {
            var currency = await gameProvider.GetGameProviderCurrencyCodeAsync(
                gameProviderId: gameProviderId,
                currencyCode: playerInfo.DefaultWalletCurrencyCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await client
                .PlayerCreateByBetLimitAsync(
                apiUrl: configuration.ApiUrl,
                agentId: configuration.AgentId,
                cert: configuration.Cert,
                playerCode: playerInfo.PlayerCode,
                userName: playerInfo.PlayerCode,
                currency: currency,
                betLimit: configuration.BetLimit.ToJsonString(),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            reply = await client.GameUrlByAwcBgAsync(
                apiUrl: configuration.ApiUrl,
                agentId: configuration.AgentId,
                cert: configuration.Cert,
                playerCode: playerInfo.PlayerCode,
                isMobileLogin: query.PlatformType == PlatformType.Mobile,
                backUrl: query.BackUrl.ToString(),
                betLimit: configuration.BetLimit.ToJsonString(),
                language: cultureCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        return !reply.IsPlayerExist
            ? throw new Exception($"Player does not exist.({playerInfo.PlayerCode})")
            : reply.GameUrl;
    }
}
