/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node subscription
/// </summary>
public class NodeSubscription
{
    /// <summary>
    /// Status
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// Next Duedate
    /// </summary>
    [JsonProperty("nextduedate")]
    public string NextDuedate { get; set; }

    /// <summary>
    /// Product Name
    /// </summary>
    [JsonProperty("productname")]
    public string ProductName { get; set; }

    /// <summary>
    /// Key
    /// </summary>
    [JsonProperty("key")]
    public string Key { get; set; }

    /// <summary>
    /// Valid Directory
    /// </summary>
    [JsonProperty("validdirectory")]
    public string ValidDirectory { get; set; }

    /// <summary>
    /// Reg date
    /// </summary>
    [JsonProperty("regdate")]
    public string RegDate { get; set; }

    /// <summary>
    /// Check time
    /// </summary>
    [JsonProperty("checktime")]
    public string CheckTime { get; set; }

    /// <summary>
    /// Level
    /// </summary>
    [JsonProperty("level")]
    public string Level { get; set; }

    /// <summary>
    /// Server id
    /// </summary>
    [JsonProperty("serverid")]
    public string Serverid { get; set; }

    /// <summary>
    /// Url
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <summary>
    /// Sockets
    /// </summary>
    [JsonProperty("sockets")]
    public int Sockets { get; set; }
}