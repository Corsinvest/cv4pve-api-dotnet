/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster
{
    /// <summary>
    /// Resource Vm
    /// </summary>
    public interface IClusterResourceVm : IVmBase, IClusterResourceHost, IDiskIO, INetIO, IPoolItem
    {

        /// <summary>
        /// Vm type
        /// </summary>
        /// <value></value>
        VmType VmType { get; set; }

        /// <summary>
        /// Vm is lock
        /// </summary>
        /// <value></value>
        [JsonProperty("lock")]
        string Lock { get; set; }

        /// <summary>
        /// Is Lock
        /// </summary>
        bool IsLocked { get; set; }

        /// <summary>
        /// Host cpu usage
        /// </summary>
        /// <value></value>
        [Display(Name = "Host Cpu Usage")]
        string HostCpuUsage { get; set; }

        /// <summary>
        /// Host memory usage
        /// </summary>
        /// <value></value>
        [Display(Name = "Host Memory Usage %")]
        [DisplayFormat(DataFormatString = "{0:P1}")]
        double HostMemoryUsage { get; set; }

        /// <summary>
        /// Tags
        /// </summary>
        /// <value></value>
        string Tags { get; set; }
    }
}