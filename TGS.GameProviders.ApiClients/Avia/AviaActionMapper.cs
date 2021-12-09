using System.Collections.Generic;

namespace TGS.GameProviders.ApiClients.Avia
{
    internal class AviaActionMapper
    {
        internal static readonly IReadOnlyDictionary<AviaAction, string> Mapper = new Dictionary<AviaAction, string>() {
            {AviaAction.Login, "/api/user/login"},
            {AviaAction.Register, "/api/user/register"},
            {AviaAction.Balance, "/api/user/balance"},
            {AviaAction.TransferOut, "/api/user/transfer"},
            {AviaAction.TransferIn, "/api/user/transfer"},
            {AviaAction.Transferinfo, "/api/user/transferinfo"},
            {AviaAction.GameRecord, "/api/log/get"},
            {AviaAction.Logout, "/api/user/logout"},
        };
    }
}
