using System.Collections.Generic;
using Tgs.GameProviders.Domains.SettingConfiguration.IMESports;

namespace Tgs.GameProviders.Core.IMESports
{
    public interface IIMESportsMerchantMapper
    {
        IMESportsBrandMerchant GetBrandMerchant(IEnumerable<IMESportsBrandMerchant> merchants, string brandCode);
    }
}
