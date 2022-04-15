using System.Collections.Generic;

namespace Tgs.GameProviders.Core.PragmaticPlay
{
    public interface IPragmaticPlayUtility
    {
        public string Md5Hash(IDictionary<string, string> parameters, string md5Key);
    }
}
