using System;

namespace Tgs.GameProviders.Core
{
    public interface ISystemClock
    {
        DateTimeOffset CurrentTime { get; }
    }
}
