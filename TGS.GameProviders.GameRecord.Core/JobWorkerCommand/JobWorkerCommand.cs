using System;

namespace TGS.GameProviders.GameRecord.Core.JobWorkerCommand
{
    public record JobWorkerCommand
    {
        public string GameProviderCode { get; init; }
        public DateTimeOffset SendDateTime { get; init; }
    }
}
