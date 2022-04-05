/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm
{
    /// <summary>
    /// Config LXC
    /// </summary>
    public class VmConfigLxc : VmConfig
    {
        /// <summary>
        /// Arch
        /// </summary>
        [JsonProperty("rootfs")]
        public string RootFs { get; set; }

        /// <summary>
        /// Hostname
        /// </summary>
        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        /// <summary>
        /// Name server
        /// </summary>
        [JsonProperty("nameserver")]
        public string Nameserver { get; set; }

        /// <summary>
        /// Search domain
        /// </summary>
        [JsonProperty("searchdomain")]
        public string SearchDomain { get; set; }

        /// <summary>
        /// Swap
        /// </summary>
        [JsonProperty("swap")]
        public int Swap { get; set; }

        /// <summary>
        /// Arch
        /// </summary>
        [JsonProperty("arch")]
        public string Arch { get; set; }

        /// <summary>
        /// Unprivileged
        /// </summary>
        [JsonProperty("unprivileged")]
        public bool Unprivileged { get; set; }
    }
}