/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm
{
    /// <summary>
    /// Snapshot data
    /// </summary>
    public class VmSnapshot
    {
        private long _time;
        private string _parent;

        /// <summary>
        /// Time
        /// </summary>
        [JsonProperty("snaptime")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatUnixTime + "}")]
        public long Time
        {
            get => _time == 0 ? DateTimeOffset.Now.ToUnixTimeSeconds() : _time;
            set => _time = value;
        }

        /// <summary>
        /// Time
        /// </summary>
        public DateTime Date => DateTimeOffset.FromUnixTimeSeconds(Time).DateTime;

        /// <summary>
        /// Parent
        /// </summary>
        [JsonProperty("parent")]
        public string Parent
        {
            get => string.IsNullOrWhiteSpace(_parent) ? "no-parent" : _parent;
            set => _parent = value;
        }

        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Vm status
        /// </summary>
        [JsonProperty("vmstate")]
        public bool VmStatus { get; set; }
    }
}