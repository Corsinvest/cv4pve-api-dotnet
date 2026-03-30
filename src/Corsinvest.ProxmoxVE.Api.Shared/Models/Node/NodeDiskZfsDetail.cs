/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node disk Zfs detail
/// </summary>
public class NodeDiskZfsDetail
{
    /// <summary>
    /// Errors
    /// </summary>
    [JsonProperty("errors")]
    public string Errors { get; set; }

    /// <summary>
    /// State
    /// </summary>
    [JsonProperty("state")]
    public string State { get; set; }

    /// <summary>
    /// Scan
    /// </summary>
    [JsonProperty("scan")]
    public string Scan { get; set; }

    /// <summary>
    /// Children
    /// </summary>
    [JsonProperty("children")]
    public IEnumerable<Child> Children { get; set; } = [];

    /// <summary>
    /// Action
    /// </summary>
    [JsonProperty("action")]
    public string Action { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// Leaf
    /// </summary>
    [JsonProperty("leaf")]
    public int Leaf { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Child
    /// </summary>
    public class Child
    {
        /// <summary>
        /// Checksum
        /// </summary>
        [JsonProperty("cksum")]
        public int Checksum { get; set; }

        /// <summary>
        /// State
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// Write
        /// </summary>
        [JsonProperty("write")]
        public int Write { get; set; }

        /// <summary>
        /// Read
        /// </summary>
        [JsonProperty("read")]
        public int Read { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Msg
        /// </summary>
        [JsonProperty("msg")]
        public string Msg { get; set; }

        /// <summary>
        /// Leaf
        /// </summary>
        [JsonProperty("leaf")]
        public int Leaf { get; set; }

        /// <summary>
        /// Children
        /// </summary>
        [JsonProperty("children")]
        public IEnumerable<Child> Children { get; set; } = [];
    }
}