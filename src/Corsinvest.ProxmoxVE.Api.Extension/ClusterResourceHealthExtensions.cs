/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;

namespace Corsinvest.ProxmoxVE.Api.Extension;

/// <summary>
/// Cluster resource health score extensions
/// </summary>
public static class ClusterResourceHealthExtensions
{
    extension(IClusterResourceHost resource)
    {
        /// <summary>
        /// Health score 0–100 (higher = healthier) for a Node or running VM, computed from
        /// CPU, memory and (Node only) disk usage. Returns <c>null</c> when not measurable
        /// (stopped VM, or a resource type that is neither Node nor VM).
        ///   Node         = 100 − (CPU·0.4 + RAM·0.4 + Disk·0.2)
        ///   VM (running) = 100 − (CPU·0.5 + RAM·0.5)
        /// </summary>
        public double? HealthScoreHost
        {
            get
            {
                var cpu = resource.CpuUsagePercentage * 100;
                var ram = resource.MemoryUsagePercentage * 100;
                var disk = resource.DiskSize > 0
                            ? (double)resource.DiskUsage / resource.DiskSize * 100
                            : 0;

                double? score = resource.ResourceType switch
                {
                    ClusterResourceType.Node => 100 - ((cpu * 0.4) + (ram * 0.4) + (disk * 0.2)),
                    ClusterResourceType.Vm => resource.Status == PveConstants.StatusVmRunning
                                                ? 100 - ((cpu * 0.5) + (ram * 0.5))
                                                : null,
                    _ => null,
                };

                return score is double s ? Math.Round(Math.Clamp(s, 0, 100), 1) : null;
            }
        }
    }

    extension(IClusterResourceStorage resource)
    {
        /// <summary>
        /// Health score 0–100 (higher = healthier) for a Storage resource, computed from
        /// disk usage (100 − Disk%). Returns <c>null</c> for non-storage resource types.
        /// </summary>
        public double? HealthScoreStorage
        {
            get
            {
                if (resource.ResourceType != ClusterResourceType.Storage) { return null; }

                var disk = resource.DiskSize > 0
                            ? (double)resource.DiskUsage / resource.DiskSize * 100
                            : 0;

                return Math.Round(Math.Clamp(100 - disk, 0, 100), 1);
            }
        }
    }

    extension(ClusterResource resource)
    {
        /// <summary>
        /// Overall health score 0–100 (higher = healthier) dispatched by resource type.
        /// Returns <c>null</c> when not measurable (stopped VM, or an unsupported type) —
        /// callers should render this as "not available" rather than a low score.
        /// Same formula as cv4pve-admin so the number matches across tools.
        /// </summary>
        public double? HealthScoreCalculated
            => resource.ResourceType switch
            {
                ClusterResourceType.Node or ClusterResourceType.Vm => resource.HealthScoreHost,
                ClusterResourceType.Storage => resource.HealthScoreStorage,
                _ => null,
            };
    }
}