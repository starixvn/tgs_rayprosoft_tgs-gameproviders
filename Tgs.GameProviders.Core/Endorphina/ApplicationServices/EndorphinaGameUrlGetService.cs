using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.EndorphinaService.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameRecord.Endorphina.Session.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Endorphina.ApplicationServices;

internal class EndorphinaGameUrlGetService(
	IBrandRepository brandRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceEndorphinaClient client,
	IPlayerRepository playerRepository,
	IGameRepository gameRepository,
	IGameRecordEndorphinaSessionClient sessionClient) : IGetService<EndorphinaGameUrlQuery, string>
{
	public async ValueTask<string> GetAsync(EndorphinaGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.Endorphina,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var player = await playerRepository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await brandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: player.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetEndorphinaConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var game = await gameRepository.GetAsync(
			gameId: query.GameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var currency = await gameProviderRepository.GetGameProviderCurrencyCodeAsync(
			gameProviderId: gameProviderId,
			currencyCode: player.DefaultWalletCurrencyCode,
			cancellationToken: cancellationToken).ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		var response = await client.GameTokenAsync(
			apiUrl: configuration.ApiUrl,
			merchantId: configuration.MerchantId,
			secretKey: configuration.SecretKey,
			gameId: game.GameExternalId,
			currency: currency,
			username: query.PlayerCode,
			cancellationToken)
			.ConfigureAwait(false);

		await sessionClient.AddSessionAsync(
			token: response.Token,
			brandCode: player.BrandCode,
			playerCode: query.PlayerCode,
			merchantId: configuration.MerchantId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var sign = ComputeHash($"{response.Token}{configuration.SecretKey}");

		var url = $"{configuration.ApiUrl}icasino/sessions/launch?token={response.Token}&sign={sign}&lang={cultureCode}&profile=noexit.xml";

		return url;
	}

	private static string ComputeHash(string data)
	{
		var dataByte = Encoding.UTF8.GetBytes(data);

		var hash = MD5.HashData(dataByte);

		return Convert.ToHexString(hash).ToUpper();
	}
}
