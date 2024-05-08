/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Vm Qemu Agent Get TimeZone
/// </summary>
public class VmQemuAgentGetTimeZone : ModelBase
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
        /// Offset
        /// </summary>
        [JsonProperty("offset")]
        public int Offset { get; set; }

        /// <summary>
        /// ZOne
        /// </summary>
        [JsonProperty("zone")]
        public string Zone { get; set; }
    }
}