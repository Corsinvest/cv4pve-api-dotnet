/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.ComponentModel.DataAnnotations;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// Net I/O
/// </summary>
public interface INetIO
{
    /// <summary>
    /// Net in
    /// </summary>
    [JsonProperty("netin")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    long NetIn { get; set; }

    /// <summary>
    /// Net out
    /// </summary>
    [JsonProperty("netout")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    long NetOut { get; set; }
}
