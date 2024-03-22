/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Base resource
/// </summary>
public interface IClusterResourceBase : INodeItem
{
    /// <summary>
    /// Description
    /// </summary>
    string Description { get; set; }

    /// <summary>
    /// Id
    /// </summary>
    /// <value></value>
    [JsonProperty("id")]
    string Id { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    /// <value></value>
    [JsonProperty("type")]
    string Type { get; set; }

    /// <summary>
    /// Resource Type
    /// </summary>
    /// <value></value>
    ClusterResourceType ResourceType { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    /// <value></value>
    [JsonProperty("status")]
    string Status { get; set; }

    /// <summary>
    /// Status Is Unknown
    /// </summary>
    /// <value></value>
    bool IsUnknown { get; set; }
}