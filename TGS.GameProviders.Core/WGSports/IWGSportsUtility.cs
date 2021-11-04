namespace TGS.GameProviders.Core.WGSports
{
    public interface IWGSportsUtility
    {
        /// <summary>
        /// Aeses the decrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="secretIV">The secret iv.</param>
        /// <param name="isDecodeUrl">if set to <c>true</c> [is decode URL].</param>
        /// <returns></returns>
        string AesDecrypt(string data, string secretKey, string secretIV, bool isDecodeUrl = true);

        /// <summary>
        /// Aeses the encrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="secretIV">The secret iv.</param>
        /// <param name="isEncodeUrl">if set to <c>true</c> [is encode URL].</param>
        /// <returns></returns>
        string AesEncrypt(string data, string secretKey, string secretIV, bool isEncodeUrl = true);
    }
}
