/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Resource Vm
/// </summary>
public interface IClusterResourceVm : IVmBase, IClusterResourceHost, IDiskIO, INetIO, IPoolItem
{
    /// <summary>
    /// Vm type
    /// </summary>
    VmType VmType { get; set; }

    /// <summary>
    /// Vm is lock
    /// </summary>
    [JsonProperty("lock")]
    string Lock { get; set; }

    /// <summary>
    /// Is Lock
    /// </summary>
    bool IsLocked { get; set; }

    /// <summary>
    /// Host cpu usage
    /// </summary>
    [Display(Name = "Host Cpu Usage")]
    string HostCpuUsage { get; set; }

    /// <summary>
    /// Host memory usage
    /// </summary>
    [Display(Name = "Host Memory Usage %")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatPercentage)]
    double HostMemoryUsage { get; set; }

    /// <summary>
    /// Tags
    /// </summary>
    string Tags { get; set; }

    /// <summary>
    /// Cgroup Mode
    /// </summary>
    [Display(Name = "Cgroup mode")]
    [JsonProperty("cgroup-mode")]
    int CgroupMode { get; set; }
}