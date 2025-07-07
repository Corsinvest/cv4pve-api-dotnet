/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Access;

/// <summary>
/// User
/// </summary>
public class AccessUser : ModelBase
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

    /// <summary>
    /// Comment
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }

    /// <summary>
    /// Realm Type
    /// </summary>
    [JsonProperty("realm-type")]
    public string RealmType { get; set; }

    /// <summary>
    /// Groups
    /// </summary>
    [JsonProperty("groups")]
    public string Groups { get; set; }

    /// <summary>
    /// Tokens
    /// </summary>
    [JsonProperty("tokens")]
    public IEnumerable<Token> Tokens { get; set; }=[];

    /// <summary>
    /// Token
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Token Id
        /// </summary>
        [JsonProperty("tokenid")]
        public string Id { get; set; }

        /// <summary>
        /// Expire
        /// </summary>
        [JsonProperty("expire")]
        public int Expire { get; set; }
        /// <summary>
        /// Token Name
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Token Privsep
        /// </summary>
        [JsonProperty("privsep")]
        public int Privsep { get; set; }
    }
}