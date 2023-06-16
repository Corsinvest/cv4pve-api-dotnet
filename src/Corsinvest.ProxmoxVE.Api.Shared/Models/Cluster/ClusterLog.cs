/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster
{
    /// <summary>
    /// Cluster log
    /// </summary>
    public class ClusterLog
    {
        /// <summary>
        /// Message
        /// </summary>
        /// <value></value>
        [JsonProperty("msg")]
        public string Message { get; set; }

        /// <summary>
        /// Uid
        /// </summary>
        /// <value></value>
        [JsonProperty("uid")]
        public string Uid { get; set; }

        /// <summary>
        /// User
        /// </summary>
        /// <value></value>
        [JsonProperty("user")]
        public string User { get; set; }

        /// <summary>
        /// Pid
        /// </summary>
        /// <value></value>
        [JsonProperty("pid")]
        public int Pid { get; set; }

        /// <summary>
        /// Severity
        /// </summary>
        /// <value></value>
        [JsonProperty("pri")]
        public int Severity { get; set; }

        /// <summary>
        /// Severity
        /// </summary>
        [Display(Name = "Severity")]
        public ClusterLogSeverity SeverityEnum => (ClusterLogSeverity)Severity;

        /// <summary>
        /// Id
        /// </summary>
        /// <value></value>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Service
        /// </summary>
        /// <value></value>
        [JsonProperty("tag")]
        public string Service { get; set; }

        /// <summary>
        /// Node
        /// </summary>
        /// <value></value>
        [JsonProperty("node")]
        public string Node { get; set; }

        /// <summary>
        /// Time unix format
        /// </summary>
        /// <value></value>
        [JsonProperty("time")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatUnixTime + "}")]
        public int Time { get; set; }

        /// <summary>
        /// Time
        /// </summary>
        public DateTime TimeDate => DateTimeOffset.FromUnixTimeSeconds(Time).DateTime;
    }
}