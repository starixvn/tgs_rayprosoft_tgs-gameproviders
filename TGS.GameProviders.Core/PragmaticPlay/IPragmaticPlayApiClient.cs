using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.PragmaticPlay.Models;

namespace TGS.GameProviders.Core.PragmaticPlay
{
    public interface IPragmaticPlayApiClient
    {
        ValueTask<PragmaticPlayGameUrlResponse> GetGameUrlAsync(
            Uri apiUrl,
            string md5Key,
            PragmaticPlayGameUrlRequest request);

        ValueTask<PragmaticPlayOpenHistoryExtendedResponse> GetRoundHistoryAsync(
            Uri apiUrl,
            string md5Key,
            PragmaticPlayOpenHistoryExtendedRequest request);
    }
}