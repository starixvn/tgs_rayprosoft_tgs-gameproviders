using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.IMESports.Models;

namespace TGS.GameProviders.Core.IMESports
{
    public interface IIMESportsApiClient
    {
        ValueTask<IMESportsLaunchGameResponse> LaunchDesktopGameAsync(
            Uri apiUrl,
            IMESportsLaunchGameRequest request);

        ValueTask<IMESportsLaunchGameResponse> LaunchMobileGameAsync(
            Uri apiUrl,
            IMESportsLaunchGameRequest request);

        ValueTask<IMESportsRegisterPlayerResponse> RegisterPlayerAsync(Uri apiUrl,
            IMESportsRegisterPlayerRequest request);
    }
}