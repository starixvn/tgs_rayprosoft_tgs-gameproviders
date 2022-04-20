using System.Collections.Generic;
using System.Linq;
using Tgs.GameProviders.Core.IMESports;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration.IMESports;

namespace Tgs.GameProviders.Services.IMESports
{
    internal class IMESportsMerchantMapper : IIMESportsMerchantMapper
    {
        public IMESportsBrandMerchant GetBrandMerchant(IEnumerable<IMESportsBrandMerchant> merchants, string brandCode)
        {
            var merchant = merchants.FirstOrDefault(b => b.BrandCode == brandCode);

            return merchant ?? merchants.FirstOrDefault(b => b.BrandCode == DefConst.BrandCodeDefault);
        }
    }
}
