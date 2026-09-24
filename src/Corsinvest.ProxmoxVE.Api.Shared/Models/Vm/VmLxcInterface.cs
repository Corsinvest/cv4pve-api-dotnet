/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Live network interface of a running container
/// </summary>
public class VmLxcInterface : ModelBase
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
    public IEnumerable<VmQemuAgentNetworkGetInterfaces.Ip> IpAddresses { get; set; } = [];

    /// <summary>
    /// MAC address from whichever field the PVE release fills
    /// </summary>
    public string MacAddress => string.IsNullOrEmpty(HardwareAddress) ? HwAddr : HardwareAddress;
}
