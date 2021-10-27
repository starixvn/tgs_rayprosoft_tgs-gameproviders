using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core
{
    public record PlayerFundResult
    {
        /// <summary>
        /// The success
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// The balance
        /// </summary>
        public decimal? Balance { get; set; }
    }
}
