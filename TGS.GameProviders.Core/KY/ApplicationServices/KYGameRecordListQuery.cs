using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core.KY.ApplicationServices
{
    public record KYGameRecordListQuery
    {
        public string BrandCode { get; init; }

        public string CurrencyCode { get; init; }

        public DateTimeOffset StartTime { get; init; }

        public DateTimeOffset EndTime { get; init; }
    }
}
