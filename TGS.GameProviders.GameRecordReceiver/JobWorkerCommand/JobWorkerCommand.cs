using System;

namespace TGS.GameProviders.GameRecordReceiver.JobWorkerCommand
{
    public record JobWorkerCommand
    {
        public string GameProviderCode { get; init; }
        public DateTimeOffset SendDateTime { get; init; }
    }
}
