/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Vm Qemu Agent GetHostName
/// </summary>
public class VmQemuAgentGetHostName : ModelBase
{
    /// <summary>
    /// Result
    /// </summary>
    [JsonProperty("result")]
    public ResultInt Result { get; set; }

    /// <summary>
    /// Result
    /// </summary>
    public class ResultInt
    {
        /// <summary>
        /// Hostname
        /// </summary>
        [JsonProperty("host-name")]
        public string HostName { get; set; }
    }
}