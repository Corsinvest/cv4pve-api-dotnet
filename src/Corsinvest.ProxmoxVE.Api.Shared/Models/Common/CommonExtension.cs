/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using System.ComponentModel;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common
{
    /// <summary>
    /// Common extension
    /// </summary>
    public static class CommonExtension
    {
        /// <summary>
        /// Improve data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="status"></param>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static void ImproveData(this IVmBase data, string status)
        {
            data.IsRunning = status == PveConstants.StatusVmRunning;
            data.IsStopped = status == PveConstants.StatusVmStopped;
            data.IsPaused = status == PveConstants.StatusVmPaused;
        }

        /// <summary>
        /// Improve data disk
        /// </summary>
        /// <param name="data"></param>
        public static void ImproveData(this IDisk data)
            => data.DiskUsagePercentage = FormatHelper.CalculatePercentage(data.DiskUsage, data.DiskSize);

        /// <summary>
        /// Improve data host cpu
        /// </summary>
        /// <param name="data"></param>
        public static void ImproveData(this ICpu data)
            => data.CpuInfo = FormatHelper.CpuInfo(data.CpuUsagePercentage, data.CpuSize);

        /// <summary>
        /// Improve data host memory
        /// </summary>
        /// <param name="data"></param>
        public static void ImproveData(this IMemory data)
        {
            data.MemoryInfo = FormatHelper.UsageInfo(data.MemoryUsage, data.MemorySize);
            data.MemoryUsagePercentage = FormatHelper.CalculatePercentage(data.MemoryUsage, data.MemorySize);
        }
    }
}