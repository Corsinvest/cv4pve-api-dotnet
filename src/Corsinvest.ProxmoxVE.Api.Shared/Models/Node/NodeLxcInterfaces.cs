/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node LXC Interfaces
/// </summary>
public class NodeLxcInterfaces : ModelBase
{
    /// <summary>
    /// Interface name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Hardware address (MAC)
    /// </summary>
    [JsonProperty("hardware-address")]
    public string HardwareAddress { get; set; }

    /// <summary>
    /// Hardware address (MAC) - legacy
    /// </summary>
    [JsonProperty("hwaddr")]
    public string HwAddr { get; set; }

    /// <summary>
    /// IPv4 address
    /// </summary>
    [JsonProperty("inet")]
    public string Inet { get; set; }

    /// <summary>
    /// IPv6 address
    /// </summary>
    [JsonProperty("inet6")]
    public string Inet6 { get; set; }

    /// <summary>
    /// IP addresses
    /// </summary>
    [JsonProperty("ip-addresses")]
    public IEnumerable<object> IpAddresses { get; set; } = [];
}
