/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Vm Qemu Agent Info
/// </summary>
public class VmQemuAgentInfo
{
    /// <summary>
    /// Result
    /// </summary>
    /// <value></value>
    [JsonProperty("result")]
    public ResultInt Result { get; set; }

    /// <summary>
    /// Result
    /// </summary>
    public class ResultInt
    {
        /// <summary>
        /// Supported Commands
        /// </summary>
        /// <value></value>
        [JsonProperty("supported_commands")]
        public IEnumerable<SupportedCommand> SupportedCommands { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        /// <value></value>
        [JsonProperty("version")]
        public string Version { get; set; }
    }

    /// <summary>
    /// Supported Command
    /// </summary>
    public class SupportedCommand
    {
        /// <summary>
        /// Enabled
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Success Response
        /// </summary>
        /// <value></value>
        [JsonProperty("success-response")]
        public bool SuccessResponse { get; set; }
    }
}