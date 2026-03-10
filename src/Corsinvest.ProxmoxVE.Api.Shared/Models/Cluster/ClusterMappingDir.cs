/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster Mapping Dir
/// </summary>
public class ClusterMappingDir : ModelBase
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; }

    /// <summary>
    /// Map
    /// </summary>
    [JsonProperty("map")]
    public IEnumerable<string> Map { get; set; } = [];

    /// <summary>
    /// Checks
    /// </summary>
    [JsonProperty("checks")]
    public IEnumerable<object> Checks { get; set; } = [];
}
