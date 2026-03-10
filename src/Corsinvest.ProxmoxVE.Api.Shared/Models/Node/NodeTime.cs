/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node Time
/// </summary>
public class NodeTime : ModelBase
{
    /// <summary>
    /// Seconds since 1970-01-01 00:00:00 UTC (Unix epoch)
    /// </summary>
    [JsonProperty("time")]
    public long Time { get; set; }

    /// <summary>
    /// Seconds since 1970-01-01 00:00:00 (local time)
    /// </summary>
    [JsonProperty("localtime")]
    public long LocalTime { get; set; }

    /// <summary>
    /// Time zone
    /// </summary>
    [JsonProperty("timezone")]
    public string Timezone { get; set; }
}
