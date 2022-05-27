/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm
{
    /// <summary>
    /// Vm Qemu Agent Os Info
    /// </summary>
    public class VmQemuAgentOsInfo
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
            /// Pretty Name
            /// </summary>
            /// <value></value>
            [JsonProperty("pretty-name")]
            public string PrettyName { get; set; }

            /// <summary>
            /// Version Id
            /// </summary>
            /// <value></value>
            [JsonProperty("version-id")]
            public string VersionId { get; set; }

            /// <summary>
            /// Kernel Version
            /// </summary>
            /// <value></value>
            [JsonProperty("kernel-version")]
            public string KernelVersion { get; set; }

            /// <summary>
            /// Id
            /// </summary>
            /// <value></value>
            [JsonProperty("id")]
            public string Id { get; set; }

            /// <summary>
            /// Name
            /// </summary>
            /// <value></value>
            [JsonProperty("name")]
            public string Name { get; set; }

            /// <summary>
            /// Kernel Release
            /// </summary>
            /// <value></value>
            [JsonProperty("kernel-release")]
            public string KernelRelease { get; set; }

            /// <summary>
            /// Variant Id
            /// </summary>
            /// <value></value>
            [JsonProperty("variant-id")]
            public string VariantId { get; set; }

            /// <summary>
            /// Version
            /// </summary>
            /// <value></value>
            [JsonProperty("version")]
            public string Version { get; set; }

            /// <summary>
            /// Machine
            /// </summary>
            /// <value></value>
            [JsonProperty("machine")]
            public string Machine { get; set; }

            /// <summary>
            /// Variant
            /// </summary>
            /// <value></value>
            [JsonProperty("variant")]
            public string Variant { get; set; }
        }
    }
}