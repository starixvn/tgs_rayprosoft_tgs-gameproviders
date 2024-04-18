using System;

namespace Tgs.GameProviders.Core;

internal class SystemClock : ISystemClock
{
	public DateTimeOffset CurrentTime => DateTimeOffset.UtcNow;
}
