/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node apt version
/// </summary>
public class NodeAptVersion
{
    /// <summary>
    /// Arch
    /// </summary>
    [JsonProperty("Arch")]
    public string Arch { get; set; }

    /// <summary>
    /// Running kernel
    /// </summary>
    [JsonProperty("RunningKernel")]
    public string RunningKernel { get; set; }

    /// <summary>
    /// Old version
    /// </summary>
    [JsonProperty("OldVersion")]
    public string OldVersion { get; set; }

    /// <summary>
    /// Version
    /// </summary>
    [JsonProperty("Version")]
    public string Version { get; set; }

    /// <summary>
    /// Change Log Url
    /// </summary>
    /// <value></value>
    [JsonProperty("ChangeLogUrl")]
    public string ChangeLogUrl { get; set; }

    /// <summary>
    /// Current State
    /// </summary>
    /// <value></value>
    [JsonProperty("CurrentState")]
    public string CurrentState { get; set; }

    /// <summary>
    /// Priority
    /// </summary>
    /// <value></value>
    [JsonProperty("Priority")]
    public string Priority { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    /// <value></value>
    [JsonProperty("Description")]
    public string Description { get; set; }

    /// <summary>
    /// Origin
    /// </summary>
    /// <value></value>
    [JsonProperty("Origin")]
    public string Origin { get; set; }

    /// <summary>
    /// Package
    /// </summary>
    /// <value></value>
    [JsonProperty("Package")]
    public string Package { get; set; }

    /// <summary>
    /// Title
    /// </summary>
    /// <value></value>
    [JsonProperty("Title")]
    public string Title { get; set; }

    /// <summary>
    /// Section
    /// </summary>
    /// <value></value>
    [JsonProperty("Section")]
    public string Section { get; set; }
}