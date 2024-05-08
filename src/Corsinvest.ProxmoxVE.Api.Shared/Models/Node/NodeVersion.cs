/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node version
/// </summary>
public class NodeVersion : ModelBase
{
    /// <summary>
    /// Repository id
    /// </summary>
    [JsonProperty("repoid")]
    public string RepositoryId { get; set; }

    /// <summary>
    /// Release
    /// </summary>
    [JsonProperty("release")]
    public string Release { get; set; }

    /// <summary>
    /// Version
    /// </summary>
    [JsonProperty("version")]
    public string Version { get; set; }

    /// <summary>
    /// Is equal
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public bool IsEqual(NodeVersion version)
        => Version == version.Version
           && Release == version.Release
           && RepositoryId == version.RepositoryId;
}