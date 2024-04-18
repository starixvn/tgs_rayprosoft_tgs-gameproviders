using System;

namespace Tgs.GameProviders.Core.VRGaming.ApplicationServices;

public record VRGamingPlayerFundCheckingQuery : PlayerFundCheckingQuery
{
	public DateTimeOffset StartTime { get; init; }

	public DateTimeOffset EndTime { get; init; }
}
