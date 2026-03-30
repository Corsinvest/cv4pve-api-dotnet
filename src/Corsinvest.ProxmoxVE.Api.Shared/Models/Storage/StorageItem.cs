/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Storage;

/// <summary>
/// Storage
/// </summary>
public class StorageItem : ModelBase, IPoolItem
{
    /// <summary>
    /// PruneBackups
    /// </summary>
    [JsonProperty("prune-backups")]
    public string PruneBackups { get; set; }

    /// <summary>
    /// Shared
    /// </summary>
    [JsonProperty("shared")]
    public bool Shared { get; set; }

    /// <summary>
    /// Content
    /// </summary>
    [JsonProperty("content")]
    public string Content { get; set; }

    /// <summary>
    /// Storage
    /// </summary>
    [JsonProperty("storage")]
    public string Storage { get; set; }

    /// <summary>
    /// Server
    /// </summary>
    [JsonProperty("server")]
    public string Server { get; set; }

    /// <summary>
    /// Path
    /// </summary>
    [JsonProperty("path")]
    public string Path { get; set; }

    /// <summary>
    /// Export
    /// </summary>
    [JsonProperty("export")]
    public string Export { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Digest
    /// </summary>
    [JsonProperty("digest")]
    public string Digest { get; set; }

    /// <summary>
    /// Sparse
    /// </summary>
    [JsonProperty("sparse")]
    public int? Sparse { get; set; }

    /// <summary>
    /// Mountpoint
    /// </summary>
    [JsonProperty("mountpoint")]
    public string Mountpoint { get; set; }

    /// <summary>
    /// Pool
    /// </summary>
    [JsonProperty("pool")]
    public string Pool { get; set; }

    /// <summary>
    /// Disable
    /// </summary>
    [JsonProperty("disable")]
    public bool Disable { get; set; }

    /// <summary>
    /// Nodes
    /// </summary>
    [JsonProperty("nodes")]
    public string Nodes { get; set; }

    /// <summary>
    /// Username
    /// </summary>
    [JsonProperty("username")]
    public string Username { get; set; }

    /// <summary>
    /// Fingerprint
    /// </summary>
    [JsonProperty("fingerprint")]
    public string Fingerprint { get; set; }

    /// <summary>
    /// Datastore
    /// /// </summary>

    [JsonProperty("datastore")]
    public string Datastore { get; set; }

    /// <summary>
    /// Krbd
    /// </summary>
    [JsonProperty("krbd")]
    public bool Krbd { get; set; }

    /// <summary>
    /// Mon host
    /// </summary>
    [JsonProperty("monhost")]
    public string Monhost { get; set; }

    /// <summary>
    /// Preallocation
    /// </summary>
    [JsonProperty("preallocation")]
    public string Preallocation { get; set; }
}