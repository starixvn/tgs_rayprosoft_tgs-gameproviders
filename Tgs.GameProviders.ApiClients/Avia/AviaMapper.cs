using System.Collections.Generic;

namespace Tgs.GameProviders.ApiClients.Avia
{
    internal class AviaMapper
    {
        internal static readonly IReadOnlyDictionary<AviaAction, string> AviaActionMapper = new Dictionary<AviaAction, string>() {
            {AviaAction.Login, "/api/user/login"},
            {AviaAction.Register, "/api/user/register"},
            {AviaAction.Balance, "/api/user/balance"},
            {AviaAction.TransferOut, "/api/user/transfer"},
            {AviaAction.TransferIn, "/api/user/transfer"},
            {AviaAction.Transferinfo, "/api/user/transferinfo"},
            {AviaAction.GameRecord, "/api/log/get"},
            {AviaAction.Logout, "/api/user/logout"}
        };

        internal static readonly IReadOnlyDictionary<string, AviaTransferInfoStatus> AviaTransferInfoStatusMapper = new Dictionary<string, AviaTransferInfoStatus>() {
            {"None",AviaTransferInfoStatus.None},
            {"Finish",AviaTransferInfoStatus.Finish},
            {"Faild",AviaTransferInfoStatus.Faild},
        };
    }
}
