using System;
using System.Collections.Generic;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.Models;

public record PragmaticPlayTransferGameRecordInfoResponse(
    DateTimeOffset NextTime,
    IEnumerable<PragmaticPlayTransferGameRecordData> GameRecords);