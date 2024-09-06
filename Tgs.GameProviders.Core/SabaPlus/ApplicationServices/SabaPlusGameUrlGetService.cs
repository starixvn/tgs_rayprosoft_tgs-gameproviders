using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SabaPlus.ApplicationServices;

public class SabaPlusGameUrlGetService() : IGetService<SabaPlusGameUrlQuery, string>
{
	public async ValueTask<string> GetAsync(SabaPlusGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		return $"https://google.com";
	}
}
