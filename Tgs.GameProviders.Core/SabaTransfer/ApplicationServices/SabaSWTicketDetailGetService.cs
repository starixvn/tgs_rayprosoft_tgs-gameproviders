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

		var url = new StringBuilder($"{configuration.ReportUrl}BetList/Refno?access_token={result.AccessToken}&refid={query.RefId}&lang={HttpUtility.UrlEncode(query.Language)}");

		if (query.BaseCurrency is not null)
		{
			url.Append($"&bc={query.BaseCurrency}");
		}
			
		// TO DO: For RNG Keno, Lottery, Table Game, LG Virtual Sports: Please provide username.
		if (!string.IsNullOrEmpty(query.PlayerUsername))
		{
			url.Append($"&urs={HttpUtility.UrlEncode(query.PlayerUsername)}");
		}
			
		return url.ToString();
	}
}
