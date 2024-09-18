using System.Text.Json;

namespace Tgs.GameProviders.Core.SabaPlus.ApplicationServices;

public class SabaPlusService
{
	protected static Target Map<Source, Target>(Source source)
	{
		var raw = JsonSerializer.Serialize(source);

		return JsonSerializer.Deserialize<Target>(raw);
	}
}
