using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TGS.GameProviders.ViewModels
{
    public record PlayerFundCheckingResponse
    {
        public bool OrderStatus { get; init; }
    }
}
