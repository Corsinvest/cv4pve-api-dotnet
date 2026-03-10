/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node Ceph Pool
/// </summary>
public class NodeCephPool : ModelBase
{
    /// <summary>
    /// Pool id
    /// </summary>
    [JsonProperty("pool")]
    public int Pool { get; set; }

    /// <summary>
    /// Pool name
    /// </summary>
    [JsonProperty("pool_name")]
    public string PoolName { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Size
    /// </summary>
    [JsonProperty("size")]
    public int Size { get; set; }

    /// <summary>
    /// Min size
    /// </summary>
    [JsonProperty("min_size")]
    public int MinSize { get; set; }

    /// <summary>
    /// PG num
    /// </summary>
    [JsonProperty("pg_num")]
    public int PgNum { get; set; }

    /// <summary>
    /// Crush rule
    /// </summary>
    [JsonProperty("crush_rule")]
    public int CrushRule { get; set; }

    /// <summary>
    /// Crush rule name
    /// </summary>
    [JsonProperty("crush_rule_name")]
    public string CrushRuleName { get; set; }

    /// <summary>
    /// Bytes used
    /// </summary>
    [JsonProperty("bytes_used")]
    public long BytesUsed { get; set; }

    /// <summary>
    /// Percent used
    /// </summary>
    [JsonProperty("percent_used")]
    public double PercentUsed { get; set; }

    /// <summary>
    /// PG autoscale mode
    /// </summary>
    [JsonProperty("pg_autoscale_mode")]
    public string PgAutoscaleMode { get; set; }

    /// <summary>
    /// PG num final
    /// </summary>
    [JsonProperty("pg_num_final")]
    public int? PgNumFinal { get; set; }

    /// <summary>
    /// PG num min
    /// </summary>
    [JsonProperty("pg_num_min")]
    public int? PgNumMin { get; set; }

    /// <summary>
    /// Target size
    /// </summary>
    [JsonProperty("target_size")]
    public long? TargetSize { get; set; }

    /// <summary>
    /// Target size ratio
    /// </summary>
    [JsonProperty("target_size_ratio")]
    public double? TargetSizeRatio { get; set; }
}
