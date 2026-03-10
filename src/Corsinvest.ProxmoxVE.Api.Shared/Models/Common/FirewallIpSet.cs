/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// Firewall IpSet
/// </summary>
public class FirewallIpSet : ModelBase
{
    /// <summary>
    /// Comment
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }

    /// <summary>
    /// Digest
    /// </summary>
    [JsonProperty("digest")]
    public string Digest { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }
}