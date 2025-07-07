/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node Apt Update
/// </summary>
public class NodeAptUpdate : ModelBase
{
    /// <summary>
    /// Package name
    /// </summary>
    [JsonProperty("Package")]
    public string Package { get; set; }

    /// <summary>
    /// Update notification status
    /// </summary>
    [JsonProperty("NotifyStatus")]
    public string NotifyStatus { get; set; }

    /// <summary>
    /// Package architecture
    /// </summary>
    [JsonProperty("Arch")]
    public string Arch { get; set; }

    /// <summary>
    /// Package version
    /// </summary>
    [JsonProperty("Version")]
    public string Version { get; set; }

    /// <summary>
    /// Package origin
    /// </summary>
    [JsonProperty("Origin")]
    public string Origin { get; set; }

    /// <summary>
    /// Package priority
    /// </summary>
    [JsonProperty("Priority")]
    public string Priority { get; set; }

    /// <summary>
    /// Package description
    /// </summary>
    [JsonProperty("Description")]
    public string Description { get; set; }

    /// <summary>
    /// Package title
    /// </summary>
    [JsonProperty("Title")]
    public string Title { get; set; }

    /// <summary>
    /// Old package version
    /// </summary>
    [JsonProperty("OldVersion")]
    public string OldVersion { get; set; }

    /// <summary>
    /// Package section
    /// </summary>
    [JsonProperty("Section")]
    public string Section { get; set; }
}
