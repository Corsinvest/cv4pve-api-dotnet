/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Vm Qemu Agent Info
/// </summary>
public class VmQemuAgentInfo : ModelBase
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
        /// Supported Commands
        /// </summary>
        [JsonProperty("supported_commands")]
        public IEnumerable<SupportedCommand> SupportedCommands { get; set; } = [];

        /// <summary>
        /// Version
        /// </summary>
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
        [JsonProperty("success-response")]
        public bool SuccessResponse { get; set; }
    }
}