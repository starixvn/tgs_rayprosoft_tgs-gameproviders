namespace TGS.GameProviders.Core.AsiaGaming
{
    public interface IAsiaGamingUtility
    {
        public string DesEncrypt(string original, string desKey);

        public string GetQueryString<TRequest>(TRequest request, string separator);

        public string Md5Hash(string original);
    }
}