/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using System.ComponentModel;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

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
        foreach (var node in items.Where(a => a.ResourceType == ClusterResourceType.Node && a.IsOnline))
        {
            var vms = items.Where(a => a.ResourceType == ClusterResourceType.Vm && a.Node == node.Node && a.Uptime > 0);
            foreach (var item in vms)
            {
                var percentage = Math.Round(item.CpuUsagePercentage / node.CpuSize * item.CpuSize * 100.0, 1);
                item.HostCpuUsage = $"{percentage} % of {node.CpuSize} {(node.CpuSize > 1 ? "CPUs" : "CPU")}";
                item.HostMemoryUsage = (double)item.MemoryUsage / node.MemorySize;
            }

            node.NodeCpuAssigned = vms.Sum(a => a.CpuSize);
            node.NodeMemoryAssigned = vms.Sum(a => a.MemorySize);
        }

        return items;
    }

    /// <summary>
    /// Improve data
    /// </summary>
    /// <param name="data"></param>
    public static void EnrichData(this IClusterResourceBase data)
    {
        data.ResourceType = data.Type switch
        {
            var s when s == PveConstants.KeyApiLxc || s == PveConstants.KeyApiQemu => ClusterResourceType.Vm,
            var s when s == PveConstants.KeyApiNode => ClusterResourceType.Node,
            var s when s == PveConstants.KeyApiStorage => ClusterResourceType.Storage,
            var s when s == PveConstants.KeyApiPool => ClusterResourceType.Pool,
            var s when s == PveConstants.KeyApiSdn => ClusterResourceType.Sdn,
            _ => ClusterResourceType.Unknown,
        };

        data.Description = data.ResourceType switch
        {
            ClusterResourceType.Node => data.Node,
            ClusterResourceType.Storage => $"{((IStorageItem)data).Storage} ({data.Node})",
            ClusterResourceType.Vm => $"{((IVmBase)data).VmId} ({((IVmBase)data).Name})",
            ClusterResourceType.Pool => ((IPoolItem)data).Pool,
            ClusterResourceType.Sdn => ((ISdnItem)data).Sdn,
            _ => string.Empty,
        };

        data.IsUnknown = data.Status == PveConstants.StatusUnknown;
    }

    /// <summary>
    /// Improve data
    /// </summary>
    /// <param name="data"></param>
    public static void EnrichData(this IClusterResourceNode data)
    {
        data.IsOnline = data.Status == PveConstants.StatusOnline;

        ((ICpu)data).EnrichData();
        ((IMemory)data).EnrichData();
        ((IDisk)data).EnrichData();
        ((IClusterResourceBase)data).EnrichData();

        data.NodeLevel = NodeHelper.DecodeLevelSupport(data.Level);
    }

    /// <summary>
    /// Improve data
    /// </summary>
    public static void EnrichData(this ClusterResource data)
    {
        ((IClusterResourceBase)data).EnrichData();

        data.IsLocked = !string.IsNullOrWhiteSpace(data.Lock);
        if (data.ResourceType == ClusterResourceType.Vm && data is IClusterResourceVm itemVm)
        {
            itemVm.EnrichData(itemVm.Status);

            itemVm.VmType = (VmType)Enum.Parse(typeof(VmType), itemVm.Type, true);
        }
        else if (data.ResourceType == ClusterResourceType.Node && data is IClusterResourceNode itemNode)
        {
            itemNode.EnrichData();
        }
        else if (data.ResourceType == ClusterResourceType.Storage && data is IClusterResourceStorage itemStorage)
        {
            itemStorage.IsAvailable = data.Status == PveConstants.StatusAvailable;
        }

        if (data is IDisk itemDisk) { itemDisk.EnrichData(); }
        if (data is ICpu itemHostCpu) { itemHostCpu.EnrichData(); }
        if (data is IMemory itemHostMemory) { itemHostMemory.EnrichData(); }
    }

    /// <summary>
    /// Default columns
    /// </summary>
    public static IEnumerable<string> GetDefaultColumns()
        =>
        [
            nameof(ClusterResource.Type),
            nameof(ClusterResource.Description),
            nameof(ClusterResource.DiskUsagePercentage),
            nameof(ClusterResource.MemoryUsagePercentage),
            nameof(ClusterResource.CpuUsagePercentage),
            nameof(ClusterResource.Uptime)
        ];

    /// <summary>
    /// Get web URL path for a cluster resource
    /// </summary>
    public static string GetWebUrl(this ClusterResource resource)
        => resource.ResourceType switch
        {
            ClusterResourceType.Node => PveWebUrlHelper.GetWebUrlNode(resource.Node),
            ClusterResourceType.Storage => PveWebUrlHelper.GetWebUrlStorage(resource.Node, resource.Storage),
            ClusterResourceType.Pool => PveWebUrlHelper.GetWebUrlPool(resource.Pool),
            ClusterResourceType.Vm when resource.VmType == VmType.Qemu => PveWebUrlHelper.GetWebUrlQemu(resource.Node, resource.VmId),
            ClusterResourceType.Vm when resource.VmType == VmType.Lxc => PveWebUrlHelper.GetWebUrlLxc(resource.Node, resource.VmId),
            _ => string.Empty,
        };

    /// <summary>
    /// Column for VM/CT
    /// </summary>
    public static IEnumerable<string> GetVmColumns()
        =>
        [
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
        ];
}