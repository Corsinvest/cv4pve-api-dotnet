/*
 * SPDX-FileCopyrightText: 2019 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils
{
    /// <summary>
    /// Constant
    /// </summary>
    public static class PveConstants
    {
        /// <summary>
        /// Key api qemu
        /// </summary>
        public static string KeyApiQemu => "qemu";

        /// <summary>
        /// Key api lxc container
        /// </summary>
        public static string KeyApiLxc => "lxc";

        /// <summary>
        /// Key api stirage
        /// </summary>
        public static string KeyApiStorage => "storage";

        /// <summary>
        /// Key api node
        /// </summary>
        public static string KeyApiNode => "node";

        /// <summary>
        /// Key api cluster
        /// </summary>
        public static string KeyApiCluster => "cluster";

        /// <summary>
        /// Key api pool
        /// </summary>
        public static string KeyApiPool => "pool";

        /// <summary>
        /// Status Online
        /// </summary>
        public static string StatusOnline => "online";

        /// <summary>
        /// Status Available
        /// </summary>
        public static string StatusAvailable => "available";

        /// <summary>
        /// Status unknown
        /// </summary>
        public static string StatusUnknown => "unknown";

        /// <summary>
        /// Status stopped
        /// </summary>
        public static string StatusVmStopped => "stopped";

        /// <summary>
        /// Status running
        /// </summary>
        public static string StatusVmRunning => "running";

        /// <summary>
        /// Status paused
        /// </summary>
        public static string StatusVmPaused => "paused";

        /// <summary>
        /// Content backup
        /// </summary>
        public static string StorageContentBackup => "backup";
    }
}