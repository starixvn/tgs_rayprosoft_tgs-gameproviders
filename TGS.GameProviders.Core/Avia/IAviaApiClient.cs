using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core.Avia
{
    public interface IAviaApiClient
    {
        ValueTask<bool> RegisterAsync(Uri apiUrl, string authorization, string playerCode, string password, string currency);


        ValueTask<bool> TranslateinfoAsync(Uri apiUrl, string authorization, string orderId);

        ValueTask<(bool Success, string orderId)> TransferInAsync(Uri apiUrl, string authorization, string playerCode, string currency, string orderId, decimal amount);

        ValueTask<(bool Success, string orderId)> TransferOutAsync(Uri apiUrl, string authorization, string playerCode, string currency, string orderId, decimal amount);

        ValueTask<(bool IsPlayer, decimal? Balance)> BalanceAsync(Uri apiUrl, string authorization, string playerCode);

        ValueTask<Uri> GetGameUrlAsync(Uri apiUrl, string authorization, string playerCode, string lang, int? cateId, int? matchId);

        ValueTask KickAsync(Uri apiUrl, string authorization, string playerCode);
    }
}
