using System.Collections.Generic;

namespace TGS.GameProviders.Core.PragmaticPlay
{
    public interface IPragmaticPlayUtility
    {
        public string Md5Hash(IDictionary<string, string> parameters, string md5Key);
    }
}