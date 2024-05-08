/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Vm Qemu Agent GetVCpus
/// </summary>
public class VmQemuAgentGetVCpus : ModelBase
{
    /// <summary>
    /// Data
    /// </summary>
    /// <value></value>
    [JsonProperty("result")]
    public IEnumerable<ResultInt> Result { get; set; }

    /// <summary>
    /// Result
    /// </summary>
    public class ResultInt
    {
        /// <summary>
        /// Can Offline
        /// </summary>
        /// <value></value>
        [JsonProperty("can-offline")]
        public bool CanOffline { get; set; }

        /// <summary>
        /// Logical Id
        /// </summary>
        /// <value></value>
        [JsonProperty("logical-id")]
        public int LogicalId { get; set; }

        /// <summary>
        /// Online
        /// </summary>
        /// <value></value>
        [JsonProperty("online")]
        public bool Online { get; set; }
    }
}