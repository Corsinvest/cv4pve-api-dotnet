/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Access;

/// <summary>
/// Two-Factor Authentication (TFA)
/// </summary>
public class AccessTfa : ModelBase
{
    /// <summary>
    /// User this entry belongs to.
    /// </summary>
    [JsonProperty("userid")]
    public string UserId { get; set; }

    /// <summary>
    /// TFA entries
    /// </summary>
    /// <value></value>
    [JsonProperty("entries")]
    public IEnumerable<AccessTfaEntry> Entries { get; set; }

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
    /// TFA Entry
    /// </summary>
    public class AccessTfaEntry
    {
        /// <summary>
        /// TFA type (totp, u2f, webauthn, yubico, recovery)
        /// </summary>
        /// <value></value>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Creation timestamp
        /// </summary>
        /// <value></value>
        [JsonProperty("created")]
        public long Created { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        /// <value></value>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Entry id
        /// </summary>
        /// <value></value>
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}

