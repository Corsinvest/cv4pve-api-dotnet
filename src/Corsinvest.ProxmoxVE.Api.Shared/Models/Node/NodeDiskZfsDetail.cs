/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node
{
    /// <summary>
    /// Node disk Zfs detail
    /// </summary>
    public class NodeDiskZfsDetail
    {
        /// <summary>
        /// Errors
        /// </summary>
        /// <value></value>
        [JsonProperty("errors")]
        public string Errors { get; set; }

        /// <summary>
        /// State
        /// </summary>
        /// <value></value>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// Scan
        /// </summary>
        /// <value></value>
        [JsonProperty("scan")]
        public string Scan { get; set; }

        /// <summary>
        /// Children
        /// </summary>
        /// <value></value>
        [JsonProperty("children")]
        public IEnumerable<Child> Children { get; set; }

        /// <summary>
        /// Action
        /// </summary>
        /// <value></value>
        [JsonProperty("action")]
        public string Action { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        /// <value></value>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Leaf
        /// </summary>
        /// <value></value>
        [JsonProperty("leaf")]
        public int Leaf { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        /// <value></value>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Child
        /// </summary>
        /// <value></value>
        public class Child
        {
            /// <summary>
            /// Checksum
            /// </summary>
            /// <value></value>
            [JsonProperty("cksum")]
            public int Checksum { get; set; }

            /// <summary>
            /// State
            /// </summary>
            /// <value></value>
            [JsonProperty("state")]
            public string State { get; set; }

            /// <summary>
            /// Write
            /// </summary>
            /// <value></value>
            [JsonProperty("write")]
            public int Write { get; set; }

            /// <summary>
            /// Read
            /// </summary>
            /// <value></value>
            [JsonProperty("read")]
            public int Read { get; set; }

            /// <summary>
            /// Name
            /// </summary>
            /// <value></value>
            [JsonProperty("name")]
            public string Name { get; set; }

            /// <summary>
            /// Msg
            /// </summary>
            /// <value></value>
            [JsonProperty("msg")]
            public string Msg { get; set; }

            /// <summary>
            /// Leaf
            /// </summary>
            /// <value></value>
            [JsonProperty("leaf")]
            public int Leaf { get; set; }

            /// <summary>
            /// Children
            /// </summary>
            [JsonProperty("children")]
            public IEnumerable<Child> Children { get; set; }
        }
    }
}