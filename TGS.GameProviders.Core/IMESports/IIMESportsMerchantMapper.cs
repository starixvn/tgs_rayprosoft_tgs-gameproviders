using System.Collections.Generic;
using TGS.GameProviders.Domains.SettingConfiguration.IMESports;

namespace TGS.GameProviders.Core.IMESports
{
    public interface IIMESportsMerchantMapper
    {
        IMESportsBrandMerchant GetBrandMerchant(IEnumerable<IMESportsBrandMerchant> merchants, string brandCode);
    }
}