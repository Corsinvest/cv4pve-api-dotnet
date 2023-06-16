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
    /// Storage RrdData
    /// </summary>
    public class NodeStorageRrdData
    {
        /// <summary>
        /// Used
        /// </summary>
        [JsonProperty("used")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public long Used { get; set; }

        /// <summary>
        /// Time
        /// </summary>
        [JsonProperty("time")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatUnixTime + "}")]
        public int Time { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        [JsonProperty("total")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public long Size { get; set; }
    }
}