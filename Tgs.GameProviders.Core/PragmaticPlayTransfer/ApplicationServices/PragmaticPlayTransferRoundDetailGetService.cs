using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.PragmaticPlay.Client;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

internal class PragmaticPlayTransferRoundDetailGetService(
	IGameBetRepository gameBetRepository,
	IGameProviderRepository gameProviderRepository,
	IGameRepository gameRepository,
	IGameInterfacePragmaticPlayClient client) : IGetService<PragmaticPlayTransferRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(PragmaticPlayTransferRoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var gameBetInfo = await gameBetRepository.GetAsync(
			gameBetId: query.GameBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetPragmaticPlayTransferConfigurationAsync(
			gameProviderConfigurationId: gameBetInfo.GameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: gameBetInfo.GameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "en";
		var gmae = await gameRepository.GetAsync(
			gameId: gameBetInfo.GameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.GameRecordDetailAsync(
			apiUrl: configuration.ApiUrl,
			secureLogin: configuration.SecureLogin,
			secretKey: configuration.SecretKey,
			playerCode: gameBetInfo.PlayerCode,
			externalRoundId: gameBetInfo.ExternalBetId,
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
