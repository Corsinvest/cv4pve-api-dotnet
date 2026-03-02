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
    /// Enable the account (default). You can set this to '0' to disable the account
    /// </summary>
    [JsonProperty("enable")]
    public bool Enable { get; set; }

    /// <summary>
    /// Full User ID, in the `name@realm` format.
    /// </summary>
    [JsonProperty("userid")]
    public string Id { get; set; }

    /// <summary>
    /// Account expiration date (seconds since epoch). '0' means no expiration date.
    /// </summary>
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
    /// First name.
    /// </summary>
    [JsonProperty("firstname")]
    public string Firstname { get; set; }

    /// <summary>
    /// Last name.
    /// </summary>
    [JsonProperty("lastname")]
    public string Lastname { get; set; }

    /// <summary>
    /// Keys for two factor auth (yubico).
    /// </summary>
    [JsonProperty("keys")]
    public string Keys { get; set; }

    /// <summary>
    /// Contains a timestamp until when a user is locked out of 2nd factors.
    /// </summary>
    [JsonProperty("tfa-locked-until")]
    public long? TfaLockedUntil { get; set; }

    /// <summary>
    /// True if the user is currently locked out of TOTP factors.
    /// </summary>
    [JsonProperty("totp-locked")]
    public bool TotpLocked { get; set; }

    /// <summary>
    /// The type of the users realm
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
        /// Account expiration date (seconds since epoch). '0' means no expiration date.
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