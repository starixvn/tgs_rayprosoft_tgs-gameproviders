namespace TGS.GameProviders.ApiClients
{
    public interface ITCGUtility
    {
        /// <summary>
        /// Descs the decrypt.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string DESCDecrypt(string str, string key);

        /// <summary>
        /// Descs the encrypt.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string DESCEncrypt(string str, string key);

        /// <summary>
        /// Gets the hash256.
        /// </summary>
        /// <param name="datas">The datas.</param>
        /// <param name="hash_key">The hash key.</param>
        /// <returns></returns>
        string GetHash256(string datas, string hash_key);
    }
}
