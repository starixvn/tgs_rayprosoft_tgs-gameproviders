using System.Collections.Generic;

namespace TGS.GameProviders.ApiClients.VR
{
    public class VRActionMapper
    {
        public static readonly Dictionary<VRAction, string> Mapper = new Dictionary<VRAction, string>() {
            {VRAction.GetGameUrl, "/Account/LoginValidate"},
            {VRAction.CreatePlayer, "/Account/CreateUser"},
            {VRAction.GetBalance, "/UserWallet/Balance"},
            {VRAction.FundOut, "/UserWallet/Transaction"},
            {VRAction.FundIn, "/UserWallet/Transaction"},
            {VRAction.FundChecking, "/UserWallet/TransactionRecord"},
            {VRAction.GameRecord, "/MerchantQuery/Bet"},
            {VRAction.Kick, "/Account/KickUser"},
        };
    }
}
