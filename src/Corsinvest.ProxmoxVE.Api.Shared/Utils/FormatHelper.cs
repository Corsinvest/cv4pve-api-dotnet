/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Humanizer.Bytes;
using System;

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils
{
    /// <summary>
    /// Info helper
    /// </summary>
    public static class FormatHelper
    {
        /// <summary>
        /// Formatter for bytes
        /// </summary>
        public const string FormatBytes = "BYTES";

        /// <summary>
        /// Formatter for bits
        /// </summary>
        public const string FormatBits = "BITS";

        /// <summary>
        /// Formatter for unix time
        /// </summary>
        public const string FormatUptimeUnixTime = "UNIX-UPTIME";

        /// <summary>
        /// FOrmat unix time
        /// </summary>
        public const string FormatUnixTime = "UNIX-TIME";

        /// <summary>
        /// From bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FromBytes(double bytes) => ByteSize.FromBytes(bytes).ToString();

        /// <summary>
        /// From bit
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static string FromBits(long bits) => ByteSize.FromBits(bits).ToString();

        /// <summary>
        /// Memory info
        /// </summary>
        public static string UsageInfo(long usage, long size)
            => $"{Math.Round(CalculatePercentage(usage, size) * 100, 1)}% ({FromBytes(usage)} of {FromBytes(size)})";

        /// <summary>
        /// Calculate percentage
        /// </summary>
        public static double CalculatePercentage(long usage, long size) => (double)usage / ((double)size == 0 ? 1 : size);

        /// <summary>
        /// CPU info
        /// </summary>
        public static string CpuInfo(double cpuUsage, long cpuSize) => $"{cpuUsage:P1} of {cpuSize} CPU(s)";
    }
}