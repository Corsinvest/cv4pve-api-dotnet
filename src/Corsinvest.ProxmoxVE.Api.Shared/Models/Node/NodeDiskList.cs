/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node
{
    /// <summary>
    /// Node disk list
    /// </summary>
    public class NodeDiskList
    {
        /// <summary>
        /// Device path
        /// </summary>
        [JsonProperty("devpath")]
        public string DevPath { get; set; }

        /// <summary>
        /// Parent
        /// </summary>
        [JsonProperty("parent")]
        public string Parent { get; set; }

        /// <summary>
        /// Vendor
        /// </summary>
        [JsonProperty("vendor")]
        public string Vendor { get; set; }

        /// <summary>
        /// Serial
        /// </summary>
        [JsonProperty("serial")]
        public string Serial { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Is ssd
        /// </summary>
        public bool IsSsd => Type == "ssd";

        /// <summary>
        /// Model
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>
        /// Wwn
        /// </summary>
        [JsonProperty("wwn")]
        public string Wwn { get; set; }

        /// <summary>
        /// Health
        /// </summary>
        [JsonProperty("health")]
        public string Health { get; set; }

        /// <summary>
        /// ByIdLink
        /// </summary>
        [JsonProperty("by_id_link")]
        public string ByIdLink { get; set; }

        /// <summary>
        /// Gpt
        /// </summary>
        [JsonProperty("gpt")]
        public int Gpt { get; set; }

        /// <summary>
        /// Wearout
        /// </summary>
        [JsonProperty("wearout")]
        public string Wearout { get; set; }

        /// <summary>
        /// Rpm
        /// </summary>
        [JsonProperty("rpm")]
        public object Rpm { get; set; }

        /// <summary>
        /// OsdId
        /// </summary>
        [JsonProperty("osdid")]
        public int OsdId { get; set; }

        /// <summary>
        /// Used
        /// </summary>
        [JsonProperty("used")]
        public string Used { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        [JsonProperty("size")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public long Size { get; set; }
    }
}