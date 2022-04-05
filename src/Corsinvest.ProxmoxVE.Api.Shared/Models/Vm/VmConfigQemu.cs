/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System.Linq;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm
{
    /// <summary>
    /// Config data qemu
    /// </summary>
    public class VmConfigQemu : VmConfig
    {
        /// <summary>
        /// Agent
        /// </summary>
        [JsonProperty("agent")]
        public string Agent { get; set; }

        /// <summary>
        /// Agent enabled.
        /// </summary>
        public bool AgentEnabled
            => !string.IsNullOrWhiteSpace(Agent) && Agent.Split(',').Select(a => a.Trim()).Any(a => a == "1");

        /// <summary>
        /// Boot Disk
        /// </summary>
        [JsonProperty("bootdisk")]
        public string BootDisk { get; set; }

        /// <summary>
        /// Start Up
        /// </summary>
        [JsonProperty("startup")]
        public string StartUp { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Sockets.
        /// </summary>
        [JsonProperty("sockets")]
        public int Sockets { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Boot
        /// </summary>
        [JsonProperty("boot")]
        public string Boot { get; set; }

        /// <summary>
        /// Numa
        /// </summary>
        [JsonProperty("numa")]
        public string Numa { get; set; }
    }
}