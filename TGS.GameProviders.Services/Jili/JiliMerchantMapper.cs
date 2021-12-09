using System.Collections.Generic;
using System.Linq;
using TGS.GameProviders.Core.Jili;
using TGS.GameProviders.Domains.SettingConfiguration.Jili;

namespace TGS.GameProviders.Services.Jili
{
    internal class JiliMerchantMapper : IJiliMerchantMapper
    {
        public JiliMerchant GetMerchant(IEnumerable<JiliMerchant> merchants, string brandCode)
            => merchants.FirstOrDefault(m => m.Brands.Contains(brandCode));
    }
}