using System;
using System.Collections.Generic;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.Models;

public class PragmaticPlayTransferGameRecordResult
{
    public IEnumerable<GameRecordListModel> Datas { get; init; } = Array.Empty<GameRecordListModel>();

    public DateTimeOffset? NextTime { get; init; }

    public bool Success { get; init; }
}
