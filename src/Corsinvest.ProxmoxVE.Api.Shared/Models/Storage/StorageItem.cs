/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Storage
{
    /// <summary>
    /// Storage
    /// </summary>
    public class StorageItem : IPoolItem
    {
        /// <summary>
        /// PruneBackups
        /// </summary>
        /// <value></value>
        [JsonProperty("prune-backups")]
        public string PruneBackups { get; set; }

        /// <summary>
        /// Shared
        /// </summary>
        /// <value></value>
        [JsonProperty("shared")]
        public bool Shared { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        /// <value></value>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// Storage
        /// </summary>
        /// <value></value>
        [JsonProperty("storage")]
        public string Storage { get; set; }

        /// <summary>
        /// Server
        /// </summary>
        /// <value></value>
        [JsonProperty("server")]
        public string Server { get; set; }

        /// <summary>
        /// Path
        /// </summary>
        /// <value></value>
        [JsonProperty("path")]
        public string Path { get; set; }

        /// <summary>
        /// Export
        /// </summary>
        /// <value></value>
        [JsonProperty("export")]
        public string Export { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        /// <value></value>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Digest
        /// </summary>
        /// <value></value>
        [JsonProperty("digest")]
        public string Digest { get; set; }

        /// <summary>
        /// Sparse
        /// </summary>
        /// <value></value>
        [JsonProperty("sparse")]
        public int? Sparse { get; set; }

        /// <summary>
        /// Mountpoint
        /// </summary>
        /// <value></value>
        [JsonProperty("mountpoint")]
        public string Mountpoint { get; set; }

        /// <summary>
        /// Pool
        /// </summary>
        /// <value></value>
        [JsonProperty("pool")]
        public string Pool { get; set; }

        /// <summary>
        /// Disable
        /// </summary>
        /// <value></value>
        [JsonProperty("disable")]
        public bool Disable { get; set; }

        /// <summary>
        /// Nodes
        /// </summary>
        /// <value></value>
        [JsonProperty("nodes")]
        public string Nodes { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        /// <value></value>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Fingerprint
        /// </summary>
        /// <value></value>
        [JsonProperty("fingerprint")]
        public string Fingerprint { get; set; }

        /// <summary>
        /// Datastore
        /// /// </summary>
        /// <value></value>
        [JsonProperty("datastore")]
        public string Datastore { get; set; }

        /// <summary>
        /// Krbd
        /// </summary>
        /// <value></value>
        [JsonProperty("krbd")]
        public bool Krbd { get; set; }

        /// <summary>
        /// Mon host
        /// </summary>
        /// <value></value>
        [JsonProperty("monhost")]
        public string Monhost { get; set; }

        /// <summary>
        /// Preallocation
        /// </summary>
        /// <value></value>
        [JsonProperty("preallocation")]
        public string Preallocation { get; set; }
    }
}