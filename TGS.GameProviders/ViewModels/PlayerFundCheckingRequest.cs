using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TGS.GameProviders.ViewModels
{
    public class PlayerFundCheckingRequest
    {
        public string BrandCode { get; init; }
        public string PlayerCode { get; init; }
        public string CurrencyCode { get; init; }
        public string OrderId { get; set; }
    }
}
