using System;
using System.Collections.Generic;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.Models;

public class PragmaticPlayTransferGameRecordResult
{
    public bool Success { get; init; }
    public DateTimeOffset? NextTime { get; init; }
    public IEnumerable<GameRecordListModel> Datas { get; init; } = Array.Empty<GameRecordListModel>();
}