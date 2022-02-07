using System.Collections.Generic;
using System.Linq;
using TGS.GameProviders.Core.Jili;
using TGS.GameProviders.Domains.SettingConfiguration.Jili;

namespace TGS.GameProviders.Services.Jili;

internal class JiliMerchantMapper : IJiliMerchantMapper
{
    private static readonly string _Default = "Default";

    public JiliMerchant GetMerchant(IEnumerable<JiliMerchant> merchants, string brandCode)
    {
        if (merchants == null)
        {
            return null;
        }

        var jiliMerchants = merchants.ToArray();

        return jiliMerchants.FirstOrDefault(m => m.BrandCode == brandCode) ??
               jiliMerchants.FirstOrDefault(m => m.BrandCode == _Default);
    }
}
