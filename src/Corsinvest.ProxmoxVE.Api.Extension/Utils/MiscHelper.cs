/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Runtime.InteropServices;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils;

/// <summary>
/// Misc helpers.
/// </summary>
public static class MiscHelper
{
    /// <summary>
    /// Opens a URL in the default system browser (cross-platform).
    /// </summary>
    public static void OpenBrowser(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            StartProcess(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            StartProcess(new System.Diagnostics.ProcessStartInfo("xdg-open", url));
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            StartProcess(new System.Diagnostics.ProcessStartInfo("open", url));
        }
    }

    //starts the program that opens the address: replaced by the tests
    internal static Action<System.Diagnostics.ProcessStartInfo> StartProcess { get; set; } = a => System.Diagnostics.Process.Start(a);
}
