using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.IMESports.Models;

namespace Tgs.GameProviders.Core.IMESports
{
    public interface IIMESportsApiClient
    {
        ValueTask<IMESportsGetRoundHistoryResponse> GetRoundHistoryAsync(
            Uri apiUrl,
            IMESportsGetRoundHistoryRequest request);

        ValueTask<IMESportsLaunchGameResponse> LaunchDesktopGameAsync(
                    Uri apiUrl,
            IMESportsLaunchGameRequest request);

        ValueTask<IMESportsLaunchGameResponse> LaunchMobileGameAsync(
            Uri apiUrl,
            IMESportsLaunchGameRequest request);

        ValueTask<IMESportsRegisterPlayerResponse> RegisterPlayerAsync(
            Uri apiUrl,
            IMESportsRegisterPlayerRequest request);
    }
}
