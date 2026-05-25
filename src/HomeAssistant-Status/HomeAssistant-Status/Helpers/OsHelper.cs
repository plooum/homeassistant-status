using System.Runtime.InteropServices;
using HomeAssistant_Status.Enums;

namespace HomeAssistant_Status.Helpers;

public static class OsHelper
{
    public static Os GetCurrentOs() => true switch
    {
        _ when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => Os.Linux,
        _ when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => Os.Windows,
        _ when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => Os.MacOs,
        _ when RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD) => Os.Bsd,
        _ => throw new PlatformNotSupportedException()
    };
}