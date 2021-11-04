namespace TGS.GameProviders.Core.AsiaGaming
{
    public interface IAsiaGamingUtility
    {
        public string Md5Hash(string original);

        public string DesEncrypt(string original, string desKey);
    }
}
