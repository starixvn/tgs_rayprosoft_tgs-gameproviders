using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.HabaneroTransfer.ApplicationServices;

internal class HabaneroTransferRoundDetailGetService(
	IGameBetRepository gameBetRepository,
	IGameProviderRepository gameProviderRepository) : IGetService<HabaneroTransferRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(HabaneroTransferRoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var gameBetInfo = await gameBetRepository.GetAsync(
			gameBetId: query.GameBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetHabaneroTransferConfigurationAsync(
			gameProviderConfigurationId: gameBetInfo.GameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: gameBetInfo.GameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		var hash = Sha256Hash($"{gameBetInfo.ExternalRoundId.ToLower()}{configuration.BrandId.ToLower()}{configuration.ApiKey.ToLower()}");

		var queryStrings = HttpUtility.ParseQueryString(string.Empty);

		queryStrings.Add("brandId", configuration.BrandId);
		queryStrings.Add("FriendlyId", gameBetInfo.ExternalBetId);
		queryStrings.Add("hash", hash);
		queryStrings.Add("viewtype", "game");
		queryStrings.Add("locale", cultureCode);

		var url = new Uri(configuration.GameApiUrl, $"/games/history?{queryStrings}");

		return new RoundDetailResult()
		{
			Data = [new RoundDetailDataUrl { Url = url.ToString() }]
		};
	}

	private static string Sha256Hash(string value)
		=> new BigInteger(SHA256.HashData(Encoding.UTF8.GetBytes(value)), isBigEndian: true).ToString("x");
}
