/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node subscription
/// </summary>
public class NodeSubscription : ModelBase
{
    /// <summary>
    /// The current subscription status.
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// Next due date of the set subscription.
    /// </summary>
    [JsonProperty("nextduedate")]
    public string NextDuedate { get; set; }

    /// <summary>
    /// Human readable productname of the set subscription.
    /// </summary>
    [JsonProperty("productname")]
    public string ProductName { get; set; }

    /// <summary>
    /// The subscription key, if set and permitted to access.
    /// </summary>
    [JsonProperty("key")]
    public string Key { get; set; }

    /// <summary>
    /// Valid Directory
    /// </summary>
    [JsonProperty("validdirectory")]
    public string ValidDirectory { get; set; }

    /// <summary>
    /// Register date of the set subscription.
    /// </summary>
    [JsonProperty("regdate")]
    public string RegDate { get; set; }

    /// <summary>
    /// Timestamp of the last check done.
    /// </summary>
    [JsonProperty("checktime")]
    public string CheckTime { get; set; }

    /// <summary>
    /// A short code for the subscription level.
    /// </summary>
    [JsonProperty("level")]
    public string Level { get; set; }

    /// <summary>
    /// The server ID, if permitted to access.
    /// </summary>
    [JsonProperty("serverid")]
    public string Serverid { get; set; }

    /// <summary>
    /// URL to the web shop.
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <summary>
    /// The number of sockets for this host.
    /// </summary>
    [JsonProperty("sockets")]
    public int Sockets { get; set; }
    /// <summary>
    /// A more human readable status message.
    /// </summary>
    [JsonProperty("message")]
    public string Message { get; set; }

    /// <summary>
    /// Signature for offline keys
    /// </summary>
    [JsonProperty("signature")]
    public string Signature { get; set; }
}
