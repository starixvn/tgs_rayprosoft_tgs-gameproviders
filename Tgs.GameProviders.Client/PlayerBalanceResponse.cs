using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Client;

public record PlayerBalanceResponse(
    decimal Balance,
    decimal FreeBalance,
    decimal OutstandingBalance);
