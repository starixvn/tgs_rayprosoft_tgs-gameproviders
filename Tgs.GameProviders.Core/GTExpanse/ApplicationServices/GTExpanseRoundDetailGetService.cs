using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.GlobalTechnology.Client;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.GTExpanse.ApplicationServices;

internal class GTExpanseRoundDetailGetService(
	IGameBetRepository gameBetRepository,
	IBrandRepository brandRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceGlobalTechnologyClient client) : IGetService<GTExpanseRoundDetailQuery, RoundDetailResult>
{
	public async ValueTask<RoundDetailResult> GetAsync(GTExpanseRoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var gameBetInfo = await gameBetRepository.GetAsync(
			gameBetId: query.GameBetId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await brandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameBetInfo.GameProviderId,
			brandId: gameBetInfo.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetGTExpanseConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await gameProviderRepository.GetGameProviderCultureCodeAsync(
			gameProviderId: gameBetInfo.GameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "EN";

		var reply = await client.GameRecordDetailAsync(
			dataApiUrl: configuration.DataApiUrl,
			brandId: configuration.BrandId,
			secretKey: configuration.SecretKey,
			externalProviderCode: GameInterface.GlobalTechnology.Protos.ProviderCodeType.Expanse,
			externalBetId: gameBetInfo.ExternalBetId,
			externalEndTime: gameBetInfo.ExternalClosedOn,
			language: cultureCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.IsSupport
			? new RoundDetailResult()
			{
				Data = [
				new RoundDetailDataUrl{ Url=reply.DetailUrl},
				new RoundDetailDataJson { Value = gameBetInfo.AdditionalData.ToString() }]
			}
			: new RoundDetailResult()
			{
				Data = [new RoundDetailDataJson { Value = gameBetInfo.AdditionalData.ToString() }]
			};
	}
}
