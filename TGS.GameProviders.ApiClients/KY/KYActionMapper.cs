using System.Collections.Generic;

namespace TGS.GameProviders.ApiClients.KY
{
    public class KYActionMapper
    {
        public static readonly Dictionary<KYAction, int> Mapper = new Dictionary<KYAction, int>() {
            { KYAction.GetGameUrl, 0 },
            { KYAction.GetBalance, 7 },
            { KYAction.FundOut, 2 },
            { KYAction.FundIn, 3 },
            { KYAction.FundChecking, 4 },
            { KYAction.Kick, 8 },
            { KYAction.GameRecord, 6 }
            };
    }
}
