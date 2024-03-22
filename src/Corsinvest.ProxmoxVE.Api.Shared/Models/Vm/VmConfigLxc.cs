/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Config LXC
/// </summary>
public class VmConfigLxc : VmConfig
{
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
    /// The number of cores per socket.
    /// </summary>
    [JsonProperty("cores")]
    public int Cores { get; set; }

    /// <summary>
    /// Unprivileged
    /// </summary>
    [JsonProperty("unprivileged")]
    public bool Unprivileged { get; set; }
}