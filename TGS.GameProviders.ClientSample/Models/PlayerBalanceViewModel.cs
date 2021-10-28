using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TGS.GameProviders.ClientSample.Models
{
    public record PlayerBalanceViewModel
    {
        public string GameProviderCode { get; init; } = default!;
        public string BrandCode { get; init; } = default!;
        public string PlayerCode { get; init; } = default!;
        public string CurrencyCode { get; init; } = default!;
    }
}
