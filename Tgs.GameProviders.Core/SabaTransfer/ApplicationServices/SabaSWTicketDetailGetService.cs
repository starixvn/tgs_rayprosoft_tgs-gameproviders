using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameProviders.Core.SabaSW.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SabaTransfer.ApplicationServices;

public class SabaSWTicketDetailGetService(
	IGameInterfaceSabaClient client,
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository) : IGetService<SabaSWTicketDetailUrlQuery, string>
{
	public async ValueTask<string> GetAsync(SabaSWTicketDetailUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.SabaSW,
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

		var configuration = await gameProviderRepository.GetSabaSWConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var result = await client.StatementReportTokenAsync(
			apiUrl: configuration.ReportUrl,
			username: configuration.Username,
			password: configuration.Password,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		var builder = new UriBuilder(new Uri(configuration.ReportUrl, "BetList/Refno"));

		var ticketQuery = HttpUtility.ParseQueryString(builder.Query);
		//Require
		ticketQuery["access_token"] = result.AccessToken;
		ticketQuery["refid"] = query.RefId.ToString();

		//Optional
		ticketQuery["lang"] = query.Language;
		ticketQuery["bc"] = query.BaseCurrency?.ToString();
		ticketQuery["urs"] = query.PlayerUsername;

		builder.Query = ticketQuery.ToString();

		return builder.ToString();
	}
}
