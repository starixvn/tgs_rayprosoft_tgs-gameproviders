using System.Collections.Generic;
using System.Linq;
using Tgs.GameProviders.Core.Jili;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration.Jili;

namespace Tgs.GameProviders.Services.Jili;

internal class JiliMerchantMapper : IJiliMerchantMapper
{
    public JiliMerchant GetMerchant(IEnumerable<JiliMerchant> merchants, string brandCode)
    {
        if (merchants == null)
        {
            return null;
        }

        var jiliMerchants = merchants.ToArray();

        return jiliMerchants.FirstOrDefault(m => m.BrandCode == brandCode) ??
               jiliMerchants.FirstOrDefault(m => m.BrandCode == CustomConst.BrandCodeDefault);
    }
}
