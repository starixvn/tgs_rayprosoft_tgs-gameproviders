using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core
{
    public record RoundDetailQuery
    {
        public Guid RoundId { get; set; } = default!;

        public string Language { get; init; } = default!;
    }
}
