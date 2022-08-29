using System;
using System.Collections.Generic;

namespace Tgs.GameProviders.Client;

public record PragmaticPlayTransferGameRecordResponse
{
    public bool Success { get; init; }
    public DateTimeOffset? NextTime { get; init; }
    public IEnumerable<GameRecordModle>? Datas { get; init; }
}
