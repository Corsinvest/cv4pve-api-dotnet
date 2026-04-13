/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
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
    public ResultInfo Result { get; set; }

    /// <summary>
    /// Result
    /// </summary>
    public class ResultInfo
    {
        /// <summary>
        /// Hostname
        /// </summary>
        [JsonProperty("host-name")]
        public string HostName { get; set; }
    }
}