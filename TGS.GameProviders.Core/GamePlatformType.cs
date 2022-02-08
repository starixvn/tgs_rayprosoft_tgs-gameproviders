using System;

namespace TGS.GameProviders.Core
{
    [Flags]
    public enum GamePlatformType : byte
    {
        Desktop = 1,
        Mobile = 2,
        DesktopAndMobile = Desktop | Mobile
    }
}
