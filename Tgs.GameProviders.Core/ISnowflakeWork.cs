using System.Net;

namespace Tgs.GameProviders.Core;

public interface ISnowflakeWork
{
    long GetDatacenterId();

    long GetId();

    IPAddress GetIPAddress();

    long GetWorkerId();
}
