using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core
{
    public enum GameActionType
    {
        PlaceBet = 0,
        WinBet = 1,
        LoseBet = 2,
        FreeBet = 3,
        TieBet = 4,
        CancelTransaction = 5,
        LockAmount = 6,
        UnlockAmount = 7,
        WinJackpot = 8
    }
}
