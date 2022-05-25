using System;
using Tgs.GameProviders.Domains;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

public record PragmaticPlayTransferGameRecordListQuery(
    PragmaticPlayTransferGameDataType DataType,
    DateTimeOffset StartTime);