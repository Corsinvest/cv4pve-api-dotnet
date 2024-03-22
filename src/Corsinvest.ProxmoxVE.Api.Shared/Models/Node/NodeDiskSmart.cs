/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node Disk Smart
/// </summary>
public class NodeDiskSmart
{
    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Health
    /// </summary>
    [JsonProperty("health")]
    public string Health { get; set; }

    /// <summary>
    /// Attributes
    /// </summary>
    [JsonProperty("attributes")]
    public IEnumerable<NodeDiskSmartAttribute> Attributes { get; set; }

    /// <summary>
    /// Node disk smart attributes
    /// </summary>
    public class NodeDiskSmartAttribute
    {
        /// <summary>
        /// Raw
        /// </summary>
        [JsonProperty("raw")]
        public string Raw { get; set; }

        /// <summary>
        /// Threshold
        /// </summary>
        [JsonProperty("threshold")]
        public int Threshold { get; set; }

        /// <summary>
        /// Worst
        /// </summary>
        [JsonProperty("worst")]
        public int Worst { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Fail
        /// </summary>
        [JsonProperty("fail")]
        public string Fail { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        [JsonProperty("value")]
        public int Value { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Flags
        /// </summary>
        [JsonProperty("flags")]
        public string Flags { get; set; }
    }
}