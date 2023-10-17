namespace Tgs.GameProviders.Core;

public interface ISnowflakeWork
{
    long GetDatacenterId();

    long GetWorkerId();

    long GetId();
}
