/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node Apt Update
/// </summary>
public class NodeAptUpdate
{
    /// <summary>
    /// Package
    /// </summary>
    /// <value></value>
    [JsonProperty("Package")]
    public string Package { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    [JsonProperty("Section")]
    public string Section { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    [JsonProperty("Arch")]
    public string Arch { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    [JsonProperty("Priority")]
    public string Priority { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    [JsonProperty("OldVersion")]
    public string OldVersion { get; set; }

    /// <summary>
    /// Version
    /// </summary>
    /// <value></value>
    [JsonProperty("Version")]
    public string Version { get; set; }

    /// <summary>
    /// Notify Status
    /// </summary>
    /// <value></value>
    [JsonProperty("NotifyStatus")]
    public string NotifyStatus { get; set; }

    /// <summary>
    /// Change Log Url
    /// </summary>
    /// <value></value>
    [JsonProperty("ChangeLogUrl")]
    public string ChangeLogUrl { get; set; }

    /// <summary>
    /// Origin
    /// </summary>
    /// <value></value>
    [JsonProperty("Origin")]
    public string Origin { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    /// <value></value>
    [JsonProperty("Description")]
    public string Description { get; set; }

    /// <summary>
    /// Title
    /// </summary>
    /// <value></value>
    [JsonProperty("Title")]
    public string Title { get; set; }
}