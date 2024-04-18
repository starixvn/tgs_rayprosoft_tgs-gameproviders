using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.PragmaticPlay.Client;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

internal class PragmaticPlayTransferRoundDetailGetService(
	IRoundRepository roundRepository,
	IGameProviderRepository gameProviderRepository,
	IGameRepository gameRepository,
	IGameInterfacePragmaticPlayClient client) : IGetService<PragmaticPlayTransferRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(
		PragmaticPlayTransferRoundDetailQuery query,
		CancellationToken cancellationToken = default)
	{
		var roundInfo = await roundRepository.GetAsync(
			roundId: query.RoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(query.RoundId.ToString(), $"This game bet does not exist.");
		var configuration = await gameProviderRepository.GetPragmaticPlayTransferConfigurationAsync(
			gameProviderConfigurationId: roundInfo.GameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: roundInfo.GameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";

		var gmae = await gameRepository.GetAsync(
			gameId: roundInfo.GameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(roundInfo.GameId.ToString(), $"This game does not exist.{roundInfo.GameId}");

		var reply = await client.GameRecordDetailAsync(
			apiUrl: configuration.ApiUrl,
			secureLogin: configuration.SecureLogin,
			secretKey: configuration.SecretKey,
			playerCode: roundInfo.PlayerCode,
			externalRoundId: roundInfo.ExternalRoundId,
			externalGameId: gmae.GameExternalId,
			language: cultureCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new RoundDetailResult
		{
			Data =
			[
				new RoundDetailDataUrl
				{
					Url = reply.Url
				}
			]
		};
	}
}
