using System;
using Tgs.GameProviders.Core.PragmaticPlayTransfer;

namespace Tgs.GameProviders.ViewModels;

public class PragmaticPlayTransferGameRecordListRequest
{
    public PragmaticPlayTransferGameDataType DataType { get; init; }

    public DateTimeOffset StartTime { get; init; }
}