/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;

namespace Corsinvest.ProxmoxVE.Api.Extension;

/// <summary>
/// Cluster resource health score extensions.
/// </summary>
/// <remarks>
/// The score is the one of the worst resource, not an average: a single saturated
/// resource makes the whole score critical. Each resource maps its usage to a score
/// whose bands match the usage colors (score 80 = warning threshold, score 60 = danger
/// threshold), so 80 and above is good, 60 to 80 is warning, below 60 is critical.
/// </remarks>
public static class ClusterResourceHealthExtensions
{
    private const double UsageWarning = 70;
    private const double UsageDanger = 80;

    // Guest memory includes the page cache, so a healthy guest often sits above 70%.
    private const double GuestMemoryWarning = 85;
    private const double GuestMemoryDanger = 95;

    // CPU is an instant sample: a spike must not turn the score critical.
    private const double CpuMinScore = 60;

    /// <summary>
    /// Maps a usage percentage to a score 0-100: 100 to 80 up to <paramref name="warning"/>,
    /// 80 to 60 up to <paramref name="danger"/>, 60 to 0 up to 100%.
    /// </summary>
    private static double UsageScore(double usage, double warning, double danger)
    {
        usage = Math.Clamp(usage, 0, 100);
        var score = usage <= warning
                        ? 100 - (usage * 20 / warning)
                        : usage <= danger
                            ? 80 - ((usage - warning) * 20 / (danger - warning))
                            : 60 - ((usage - danger) * 60 / (100 - danger));

        return Math.Clamp(score, 0, 100);
    }

    private static double CpuScore(double cpuUsagePercentage)
        => Math.Max(UsageScore(cpuUsagePercentage * 100, UsageWarning, UsageDanger), CpuMinScore);

    private static double DiskScore(ulong diskUsage, ulong diskSize)
        => diskSize > 0
            ? UsageScore((double)diskUsage / diskSize * 100, UsageWarning, UsageDanger)
            : 100;

    extension(IClusterResourceHost resource)
    {
        /// <summary>
        /// Health score 0-100 (higher = healthier) for a Node or running VM: the score of the
        /// worst resource among CPU, memory and (Node only) root disk. Returns <c>null</c> when
        /// not measurable (stopped VM, Node not online, or a resource type that is neither Node nor VM).
        /// </summary>
        public double? HealthScoreHost
        {
            get
            {
                double? score = resource.ResourceType switch
                {
                    ClusterResourceType.Node => resource.Status == PveConstants.StatusOnline
                                                ? Math.Min(CpuScore(resource.CpuUsagePercentage),
                                                           Math.Min(UsageScore(resource.MemoryUsagePercentage * 100, UsageWarning, UsageDanger),
                                                                    DiskScore(resource.DiskUsage, resource.DiskSize)))
                                                : null,
                    ClusterResourceType.Vm => resource.Status == PveConstants.StatusVmRunning
                                                ? Math.Min(CpuScore(resource.CpuUsagePercentage),
                                                           UsageScore(resource.MemoryUsagePercentage * 100, GuestMemoryWarning, GuestMemoryDanger))
                                                : null,
                    _ => null,
                };

                return score is double s ? Math.Round(s, 1) : null;
            }
        }
    }

    extension(IClusterResourceStorage resource)
    {
        /// <summary>
        /// Health score 0-100 (higher = healthier) for a Storage resource, computed from
        /// disk usage. Returns <c>null</c> for a storage that is not available
        /// and for non-storage resource types.
        /// </summary>
        public double? HealthScoreStorage
        {
            get
            {
                if (resource.ResourceType != ClusterResourceType.Storage
                    || resource.Status != PveConstants.StatusAvailable) { return null; }

                return Math.Round(DiskScore(resource.DiskUsage, resource.DiskSize), 1);
            }
        }
    }

    extension(ClusterResource resource)
    {
        /// <summary>
        /// Overall health score 0-100 (higher = healthier) dispatched by resource type.
        /// Returns <c>null</c> when not measurable (stopped VM, Node not online, storage
        /// not available, or an unsupported type);
        /// callers should render this as "not available" rather than a low score.
        /// See the class remarks for how the score maps to the usage bands.
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
