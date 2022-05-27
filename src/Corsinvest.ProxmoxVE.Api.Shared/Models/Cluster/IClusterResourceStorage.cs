/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster
{
    /// <summary>
    /// resource storage
    /// </summary>
    public interface IClusterResourceStorage : IClusterResourceBase, IDisk, IStorageItem
    {
        /// <summary>
        /// Shared storage
        /// </summary>
        [JsonProperty("share")]
        bool Shared { get; set; }

        /// <summary>
        /// Plugin Type
        /// </summary>
        [JsonProperty("plugintype")]
        string PluginType { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        [JsonProperty("content")]
        string Content { get; set; }

        /// <summary>
        /// Is Available
        /// </summary>
        public bool IsAvailable { get; set; }
    }
}