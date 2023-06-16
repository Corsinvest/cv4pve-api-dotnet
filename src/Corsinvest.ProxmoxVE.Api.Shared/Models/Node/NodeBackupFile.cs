/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node
{
    /// <summary>
    /// Backup file
    /// </summary>
    public class NodeBackupFile
    {
        /// <summary>
        /// Leaft
        /// </summary>
        [JsonProperty("leaft")]
        public int Leaft { get; set; }

        /// <summary>
        /// File path
        /// </summary>
        [JsonProperty("filepath")]
        public string FilePath { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Size info
        /// </summary>
        public string SizeInfo => FormatHelper.FromBytes(Size);

        /// <summary>
        /// Text
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Modified Time Unix
        /// </summary>
        [JsonProperty("mtime")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatUnixTime + "}")]
        public long ModifiedTime { get; set; }

        /// <summary>
        /// Modified time
        /// </summary>
        public DateTime ModifiedTimeDate => DateTimeOffset.FromUnixTimeSeconds(ModifiedTime).DateTime;

        /// <summary>
        /// Size
        /// </summary>
        [JsonProperty("size")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public long Size { get; set; }
    }
}