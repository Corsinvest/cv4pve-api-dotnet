/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node
{
    /// <summary>
    /// Node network
    /// </summary>
    public class NodeNetwork
    {
        /// <summary>
        /// Method6
        /// </summary>
        /// <value></value>
        [JsonProperty("method6")]
        public string Method6 { get; set; }

        /// <summary>
        /// Priority
        /// </summary>
        /// <value></value>
        [JsonProperty("priority")]
        public int Priority { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        /// <value></value>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// BondMode
        /// </summary>
        /// <value></value>
        [JsonProperty("bond_mode")]
        public string BondMode { get; set; }

        /// <summary>
        /// Cidr
        /// </summary>
        /// <value></value>
        [JsonProperty("cidr")]
        public string Cidr { get; set; }

        /// <summary>
        /// Active
        /// </summary>
        /// <value></value>
        [JsonProperty("active")]
        public bool Active { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        /// <value></value>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Comments
        /// </summary>
        /// <value></value>
        [JsonProperty("comments")]
        public string Comments { get; set; }

        /// <summary>
        /// Families
        /// </summary>
        /// <value></value>
        [JsonProperty("families")]
        public IEnumerable<string> Families { get; set; }

        /// <summary>
        /// Interface
        /// </summary>
        /// <value></value>
        [JsonProperty("iface")]
        public string Interface { get; set; }

        /// <summary>
        /// Bond Miimon
        /// </summary>
        /// <value></value>
        [JsonProperty("bond_miimon")]
        public string BondMiimon { get; set; }

        /// <summary>
        /// Slaves
        /// </summary>
        /// <value></value>
        [JsonProperty("slaves")]
        public string Slaves { get; set; }

        /// <summary>
        /// Auto Start
        /// </summary>
        /// <value></value>
        [JsonProperty("autostart")]
        public int AutoStart { get; set; }

        /// <summary>
        /// BondPrimary
        /// </summary>
        /// <value></value>
        [JsonProperty("bond-primary")]
        public string BondPrimary { get; set; }

        /// <summary>
        /// Method
        /// </summary>
        /// <value></value>
        [JsonProperty("method")]
        public string Method { get; set; }

        /// <summary>
        /// Netmask
        /// </summary>
        /// <value></value>
        [JsonProperty("netmask")]
        public string Netmask { get; set; }

        /// <summary>
        /// Bridge Stp
        /// </summary>
        /// <value></value>
        [JsonProperty("bridge_stp")]
        public string BridgeStp { get; set; }

        /// <summary>
        /// Bridge Vlan Aware
        /// </summary>
        /// <value></value>
        [JsonProperty("bridge_vlan_aware")]
        public int? BridgeVlanAware { get; set; }

        /// <summary>
        /// Bridge Vids
        /// </summary>
        /// <value></value>
        [JsonProperty("bridge_vids")]
        public string BridgeVids { get; set; }

        /// <summary>
        /// Bridge Fd
        /// </summary>
        /// <value></value>
        [JsonProperty("bridge_fd")]
        public string BridgeFd { get; set; }

        /// <summary>
        /// BridgePorts
        /// </summary>
        /// <value></value>
        [JsonProperty("bridge_ports")]
        public string BridgePorts { get; set; }

        /// <summary>
        /// Exists
        /// </summary>
        /// <value></value>
        [JsonProperty("exists")]
        public int? Exists { get; set; }

        /// <summary>
        /// Gateway
        /// </summary>
        /// <value></value>
        [JsonProperty("gateway")]
        public string Gateway { get; set; }
    }
}