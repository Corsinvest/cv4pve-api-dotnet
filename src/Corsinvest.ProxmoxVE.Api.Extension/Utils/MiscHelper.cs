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
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            System.Diagnostics.Process.Start("xdg-open", url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            System.Diagnostics.Process.Start("open", url);
        }
    }
}
