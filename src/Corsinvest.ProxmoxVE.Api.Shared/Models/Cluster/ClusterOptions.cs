/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster options
/// </summary>
public class ClusterOptions : ModelBase
{
    /// <summary>
    /// Keyboard
    /// </summary>
    [JsonProperty("keyboard")]
    public string Keyboard { get; set; }

    /// <summary>
    /// Console
    /// </summary>
    [JsonProperty("console")]
    public string Console { get; set; }

    /// <summary>
    /// Allowed Tags
    /// </summary>
    /// <value></value>
    [JsonProperty("allowed-tags")]
    public IEnumerable<string> AllowedTags { get; set; } = [];

    /// <summary>
    /// Migration
    /// </summary>
    [JsonProperty("migration")]
    public MigrationInt Migration { get; set; }

    /// <summary>
    /// Mac Prefix
    /// </summary>
    [JsonProperty("mac_prefix")]
    public string MacPrefix { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; }

    /// <summary>
    /// Tag Style
    /// </summary>
    [JsonProperty("tag-style")]
    public TagStyleInt TagStyle { get; set; }

    /// <summary>
    /// Tag Style
    /// </summary>
    public class TagStyleInt
    {
        /// <summary>
        /// Color Map
        /// </summary>
        [JsonProperty("color-map")]
        public string ColorMap { get; set; }
    }

    /// <summary>
    /// Migration
    /// </summary>
    public class MigrationInt
    {
        /// <summary>
        /// Network
        /// </summary>
        [JsonProperty("network")]
        public string Network { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}