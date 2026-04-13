/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Vm Qemu Agent Os Info
/// </summary>
public class VmQemuAgentOsInfo : ModelBase
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
        /// Pretty Name
        /// </summary>
        [JsonProperty("pretty-name")]
        public string PrettyName { get; set; }

        /// <summary>
        /// Version Id
        /// </summary>
        [JsonProperty("version-id")]
        public string VersionId { get; set; }

        /// <summary>
        /// Kernel Version
        /// </summary>
        [JsonProperty("kernel-version")]
        public string KernelVersion { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Kernel Release
        /// </summary>
        [JsonProperty("kernel-release")]
        public string KernelRelease { get; set; }

        /// <summary>
        /// Variant Id
        /// </summary>
        [JsonProperty("variant-id")]
        public string VariantId { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// OS Version
        /// </summary>
        public string OsVersion
        => !string.IsNullOrEmpty(Version) && Version != "N/A"
            ? Version
            : !string.IsNullOrEmpty(KernelVersion)
                ? KernelVersion
                : !string.IsNullOrEmpty(PrettyName)
                    ? PrettyName
                        : string.Empty;

        /// <summary>
        /// Machine
        /// </summary>
        [JsonProperty("machine")]
        public string Machine { get; set; }

        /// <summary>
        /// Variant
        /// </summary>
        [JsonProperty("variant")]
        public string Variant { get; set; }
    }
}