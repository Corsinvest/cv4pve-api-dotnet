/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Access
{
    /// <summary>
    /// User
    /// </summary>
    public class AccessUser
    {
        /// <summary>
        /// Enabled
        /// </summary>
        /// <value></value>
        [JsonProperty("enable")]
        public int Enable { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        /// <value></value>
        [JsonProperty("userid")]
        public string Id { get; set; }

        /// <summary>
        /// Expire
        /// </summary>
        /// <value></value>
        [JsonProperty("expire")]
        public int Expire { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        /// <value></value>
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}