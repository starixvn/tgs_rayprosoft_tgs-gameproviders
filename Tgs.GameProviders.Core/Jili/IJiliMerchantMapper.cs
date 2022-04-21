using System.Collections.Generic;
using Tgs.GameProviders.Domains.SettingConfiguration.Jili;

namespace Tgs.GameProviders.Core.Jili;

public interface IJiliMerchantMapper
{
    JiliMerchant GetMerchant(IEnumerable<JiliMerchant> merchants, string brandCode);
}
