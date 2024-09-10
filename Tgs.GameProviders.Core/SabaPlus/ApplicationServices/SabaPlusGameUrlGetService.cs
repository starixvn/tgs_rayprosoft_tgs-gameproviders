using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using Tgs.GameInterface.SabaPlus.Client;
using Tgs.GameInterface.SabaPlus.Domains.Models;
using System;
using Tgs.GameInterface.SabaPlus.Domains.GetGameUrl;

namespace Tgs.GameProviders.Core.SabaPlus.ApplicationServices;

public class SabaPlusGameUrlGetService(IGameInterfaceSabaPlusClient client) : IGetService<SabaPlusGameUrlQuery, GetGameUrlResult>
{
	public async ValueTask<GetGameUrlResult> GetAsync(SabaPlusGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var provider = new ProviderInfo(new Uri("https://google.com"), "password", "signKey");
		var product = new ProductInfo("GameName", "GameId", "AppType");
		var cust = new CustInfo("Id", "Name", "en", "Mobile", "UUS", "BrowserAgent", "0.0.0.0", "SessionID");
		var agent = new AgentInfo("id", "key", "code", "extra");
		var homeUrl = "abac";
		var additional = "";

		var url = await client.GameUrlAsync(provider, product, cust, agent, homeUrl, additional, cancellationToken).ConfigureAwait(false);

		return url;
	}
}
