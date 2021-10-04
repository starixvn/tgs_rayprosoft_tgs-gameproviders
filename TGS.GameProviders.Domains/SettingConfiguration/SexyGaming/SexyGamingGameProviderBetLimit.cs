using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Domains.SettingConfiguration.SexyGaming
{
    public record SexyGamingGameProviderBetLimit(Guid Id, string CurrencyCode, int MinBet, int MaxBet, int MaxBetPerMatch, string ExternalSetting = "");
}
