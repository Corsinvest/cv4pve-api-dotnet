/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// interface uptime
/// </summary>
public interface IUptimeItem
{
    /// <summary>
    /// Uptime
    /// </summary>
    /// <value></value>
    [JsonProperty("uptime")]
    [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatUptimeUnixTime + "}")]
    long Uptime { get; set; }
}