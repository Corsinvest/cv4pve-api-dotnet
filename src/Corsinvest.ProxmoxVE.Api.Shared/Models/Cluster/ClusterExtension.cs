/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster
{
    /// <summary>
    /// ResourceData extension
    /// </summary>
    public static class ClusterExtension
    {
        /// <summary>
        /// Calculate host usage
        /// </summary>
        /// <param name="items"></param>
        public static IEnumerable<TResource> CalculateHostUsage<TResource>(this IEnumerable<TResource> items)
            where TResource : ClusterResource
        {
            foreach (var item in items.Where(a => a.ResourceType == ClusterResourceType.Vm))
            {
                var node = items.FirstOrDefault(a => a.ResourceType == ClusterResourceType.Node && a.Node == item.Node);
                if (node != null) { item.CalculateHostUsage(node.MemorySize, node.CpuSize); }
            }

            return items;
        }

        /// <summary>
        /// Calculate Host Usage
        /// </summary>
        /// <param name="data"></param>
        /// <param name="hostMemorySize"></param>
        /// <param name="hostCpuSize"></param>
        public static void CalculateHostUsage(this ClusterResource data, long hostMemorySize, long hostCpuSize)
        {
            //calculate host usage
            if (data.ResourceType == ClusterResourceType.Vm && data.Uptime > 0 && hostCpuSize > 0 && hostMemorySize > 0)
            {
                var per = Math.Round(data.CpuUsagePercentage / hostCpuSize * data.CpuSize, 1);
                data.HostCpuUsage = $"{per} % of {hostCpuSize} {(hostCpuSize > 1 ? "CPUs" : "CPU")}";

                data.HostMemoryUsage = (double)data.MemoryUsage / hostMemorySize;
            }
        }

        /// <summary>
        /// Improve data
        /// </summary>
        /// <param name="data"></param>
        public static void ImproveData(this IClusterResourceBase data)
        {
            data.ResourceType = data.Type switch
            {
                var s when s == PveConstants.KeyApiLxc || s == PveConstants.KeyApiQemu => ClusterResourceType.Vm,
                var s when s == PveConstants.KeyApiNode => ClusterResourceType.Node,
                var s when s == PveConstants.KeyApiStorage => ClusterResourceType.Storage,
                var s when s == PveConstants.KeyApiPool => ClusterResourceType.Pool,
                _ => ClusterResourceType.Unknown,
            };

            data.Description = data.Type switch
            {
                var s when s == PveConstants.KeyApiNode => data.Node,
                var s when s == PveConstants.KeyApiStorage => $"{((IStorageItem)data).Storage} ({data.Node})",
                var s when s == PveConstants.KeyApiQemu || s == PveConstants.KeyApiLxc => $"{((IVmBase)data).VmId} ({((IVmBase)data).Name})",
                var s when s == PveConstants.KeyApiPool => ((IPoolItem)data).Pool,
                _ => "",
            };

            data.IsUnknown = data.Status == PveConstants.StatusUnknown;
        }

        /// <summary>
        /// Improve data
        /// </summary>
        /// <param name="data"></param>
        public static void ImproveData(this IClusterResourceNode data)
        {
            data.IsOnline = data.Status == PveConstants.StatusOnline;

            ((ICpu)data).ImproveData();
            ((IMemory)data).ImproveData();
            ((IDisk)data).ImproveData();
            ((IClusterResourceBase)data).ImproveData();

            data.NodeLevel = NodeHelper.DecodeLevelSupport(data.Level);
        }

        /// <summary>
        /// Improve data
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static void ImproveData(this ClusterResource data)
        {
            ((IClusterResourceBase)data).ImproveData();

            data.IsLocked = !string.IsNullOrWhiteSpace(data.Lock);
            if (data.ResourceType == ClusterResourceType.Vm && data is IClusterResourceVm itemVm)
            {
                itemVm.ImproveData(itemVm.Status);

                itemVm.VmType = (VmType)Enum.Parse(typeof(VmType), itemVm.Type, true);
            }
            else if (data.ResourceType == ClusterResourceType.Node && data is IClusterResourceNode itemNode)
            {
                itemNode.ImproveData();
            }
            else if (data.ResourceType == ClusterResourceType.Storage && data is IClusterResourceStorage itemStorage)
            {
                itemStorage.IsAvailable = data.Status == PveConstants.StatusAvailable;
            }

            if (data is IDisk itemDisk) { itemDisk.ImproveData(); }
            if (data is ICpu itemHostCpu) { itemHostCpu.ImproveData(); }
            if (data is IMemory itemHostMemory) { itemHostMemory.ImproveData(); }
        }

        /// <summary>
        /// Default columns
        /// </summary>
        public static IEnumerable<string> GetDefaultColumns()
            => new[]
            {
                nameof(ClusterResource.Type),
                nameof(ClusterResource.Description),
                nameof(ClusterResource.DiskUsagePercentage),
                nameof(ClusterResource.MemoryUsagePercentage),
                nameof(ClusterResource.CpuUsagePercentage),
                nameof(ClusterResource.Uptime)
            };

        /// <summary>
        /// Column for VM/CT
        /// </summary>
        public static IEnumerable<string> GetVmColumns()
            => new[]
            {
                nameof(ClusterResource.Type),
                nameof(ClusterResource.Description),
                nameof(ClusterResource.DiskUsage),
                nameof(ClusterResource.DiskSize),
                nameof(ClusterResource.DiskUsagePercentage),
                nameof(ClusterResource.MemoryUsagePercentage),
                nameof(ClusterResource.CpuUsagePercentage),
                nameof(ClusterResource.Uptime),
                nameof(ClusterResource.HostCpuUsage),
                nameof(ClusterResource.HostMemoryUsage),
            };
    }
}