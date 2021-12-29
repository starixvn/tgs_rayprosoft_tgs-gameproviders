using System.Collections.Generic;
using TGS.GameProviders.Domains.SettingConfiguration.Jili;

namespace TGS.GameProviders.Core.Jili;

public interface IJiliMerchantMapper
{
    JiliMerchant GetMerchant(IEnumerable<JiliMerchant> merchants, string brandCode);
}