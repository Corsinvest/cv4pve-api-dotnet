/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
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
    [JsonProperty("id")]
    string Id { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    string Type { get; set; }

    /// <summary>
    /// Resource Type
    /// </summary>
    ClusterResourceType ResourceType { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    [JsonProperty("status")]
    string Status { get; set; }

    /// <summary>
    /// Status Is Unknown
    /// </summary>
    bool IsUnknown { get; set; }
}