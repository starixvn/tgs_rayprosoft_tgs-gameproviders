using System;

namespace Tgs.GameProviders.Core.VR.ApplicationServices;

public record VRPlayerFundCheckingQuery : PlayerFundCheckingQuery
{
    public DateTimeOffset StartTime { get; init; }

    public DateTimeOffset EndTime { get; init; }
}
