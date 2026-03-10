/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node Ceph FS
/// </summary>
public class NodeCephFs : ModelBase
{
    /// <summary>
    /// Name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Data pool
    /// </summary>
    [JsonProperty("data_pool")]
    public string DataPool { get; set; }

    /// <summary>
    /// Metadata pool
    /// </summary>
    [JsonProperty("metadata_pool")]
    public string MetadataPool { get; set; }
}
