/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Access
{
    /// <summary>
    /// Role
    /// </summary>
    public class AccessRole
    {
        /// <summary>
        /// Privileges
        /// </summary>
        /// <value></value>
        [JsonProperty("privs")]
        public string Privileges { get; set; }

        /// <summary>
        /// Role Id
        /// </summary>
        /// <value></value>
        [JsonProperty("roleid")]
        public string Id { get; set; }

        /// <summary>
        /// Special
        /// </summary>
        /// <value></value>
        [JsonProperty("special")]
        public int Special { get; set; }
    }
}